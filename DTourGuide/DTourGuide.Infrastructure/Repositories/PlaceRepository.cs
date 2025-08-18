using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTourGuide.Domain.Entities;
using DTourGuide.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DTourGuide.Infrastructure.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly AppDbContext _db;
        public PlaceRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<Place>> GetAsync(Category? category = null, string? search = null)
        {
            var q = _db.Places.Include(p => p.Photos).AsQueryable();
            if (category.HasValue) q = q.Where(p => p.Category == category);
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => p.Name.Contains(search));
            return await q.AsNoTracking().ToListAsync();
        }

        public Task<Place?> GetByIdAsync(int id) =>
            _db.Places.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id)!;

        public async Task<Place> AddAsync(Place place)
        {
            _db.Places.Add(place);
            await _db.SaveChangesAsync();
            return place;
        }

        public async Task UpdateAsync(Place place)
        {
            _db.Places.Update(place);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Place place)
        {
            _db.Places.Remove(place);
            await _db.SaveChangesAsync();
        }

        public Task SaveAsync() => _db.SaveChangesAsync();
    }
}
