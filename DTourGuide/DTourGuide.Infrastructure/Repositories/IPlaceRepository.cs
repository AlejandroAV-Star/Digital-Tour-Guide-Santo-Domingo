using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTourGuide.Domain.Entities;

namespace DTourGuide.Infrastructure.Repositories
{
    public interface IPlaceRepository
    {
        Task<IEnumerable<Place>> GetAsync(Category? category = null, string? search = null);
        Task<Place?> GetByIdAsync(int id);
        Task<Place> AddAsync(Place place);
        Task UpdateAsync(Place place);
        Task DeleteAsync(Place place);
        Task SaveAsync();
    }
}
