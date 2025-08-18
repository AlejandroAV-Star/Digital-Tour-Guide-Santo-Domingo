using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTourGuide.Application.DTOs;
using DTourGuide.Domain.Entities;

namespace DTourGuide.Application.Services
{
    public interface IPlaceService
    {
        Task<IEnumerable<PlaceListDto>> GetAsync(Category? category = null, string? search = null);
        Task<PlaceDetailDto?> GetByIdAsync(int id);
        Task<PlaceDetailDto> CreateAsync(PlaceDetailDto dto);
        Task<PlaceDetailDto?> UpdateAsync(int id, PlaceDetailDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IReadOnlyList<PlaceListDto>> BuildItineraryAsync(IEnumerable<int> placeIds, double? startLat, double? startLng);
    }
}
