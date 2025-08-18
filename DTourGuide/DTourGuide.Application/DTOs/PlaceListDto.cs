using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTourGuide.Domain.Entities;

namespace DTourGuide.Application.DTOs
{
    public record PlaceListDto(
        int Id, string Name, double Latitude, double Longitude, Category Category
    );
}

