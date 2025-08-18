using AutoMapper;
using DTourGuide.Application.DTOs;
using DTourGuide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DTourGuide.Application.MappingProfiles
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            CreateMap<Place, PlaceListDto>();
            CreateMap<Place, PlaceDetailDto>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<PlaceDetailDto, Place>()
                .ForMember(d => d.Photos, opt => opt.Ignore());
        }
    }
}

