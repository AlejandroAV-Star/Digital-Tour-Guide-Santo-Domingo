using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTourGuide.Domain.Entities;

namespace DTourGuide.Application.DTOs
{
    public record PlaceDetailDto(
        int Id, string Name, string? Description, string? Address, string? OpeningHours,
        double Latitude, double Longitude, Category Category, IEnumerable<PhotoDto> Photos
    );
}
