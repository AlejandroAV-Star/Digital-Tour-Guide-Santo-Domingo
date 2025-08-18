using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DTourGuide.Application.DTOs;
using DTourGuide.Domain.Entities;
using DTourGuide.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace DTourGuide.Application.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceRepository _repo;
        private readonly IMapper _mapper;
        public PlaceService(IPlaceRepository repo, IMapper mapper)
        {
            _repo = repo; _mapper = mapper;
        }

        public async Task<IEnumerable<PlaceListDto>> GetAsync(Category? category = null, string? search = null)
        {
            var data = await _repo.GetAsync(category, search);
            return data.AsQueryable().ProjectTo<PlaceListDto>(_mapper.ConfigurationProvider).ToList();
        }

        public async Task<PlaceDetailDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<PlaceDetailDto>(entity);
        }

        public async Task<PlaceDetailDto> CreateAsync(PlaceDetailDto dto)
        {
            var entity = _mapper.Map<Domain.Entities.Place>(dto);
            entity.Id = 0;
            var saved = await _repo.AddAsync(entity);
            return _mapper.Map<PlaceDetailDto>(saved);
        }

        public async Task<PlaceDetailDto?> UpdateAsync(int id, PlaceDetailDto dto)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current == null) return null;

            current.Name = dto.Name;
            current.Description = dto.Description;
            current.Address = dto.Address;
            current.OpeningHours = dto.OpeningHours;
            current.Latitude = dto.Latitude;
            current.Longitude = dto.Longitude;
            current.Category = dto.Category;

            await _repo.UpdateAsync(current);
            return _mapper.Map<PlaceDetailDto>(current);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current == null) return false;
            await _repo.DeleteAsync(current);
            return true;
        }

        public async Task<IReadOnlyList<PlaceListDto>> BuildItineraryAsync(IEnumerable<int> placeIds, double? startLat, double? startLng)
        {
            var set = new HashSet<int>(placeIds);
            var all = await _repo.GetAsync(null, null);
            var selected = all.Where(p => set.Contains(p.Id)).Select(p => new PlaceListDto(p.Id, p.Name, p.Latitude, p.Longitude, p.Category)).ToList();
            if (selected.Count <= 1) return selected;

            double curLat = startLat ?? selected[0].Latitude;
            double curLng = startLng ?? selected[0].Longitude;

            var remaining = new List<PlaceListDto>(selected);
            var route = new List<PlaceListDto>();

            while (remaining.Count > 0)
            {
                var next = remaining.OrderBy(p => HaversineKm(curLat, curLng, p.Latitude, p.Longitude)).First();
                route.Add(next);
                remaining.Remove(next);
                curLat = next.Latitude; curLng = next.Longitude;
            }
            return route;

            static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
            {
                const double R = 6371;
                double dLat = ToRad(lat2 - lat1), dLon = ToRad(lon2 - lon1);
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                           Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                return 2 * R * Math.Asin(Math.Min(1, Math.Sqrt(a)));
                static double ToRad(double d) => d * Math.PI / 180.0;
            }
        }
    }
}
