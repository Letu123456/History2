using AutoMapper;
using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mapping
{
    public class MuseumMapping:Profile
    {
        public MuseumMapping()
        {
            CreateMap<Museum, MuseumDto>()

            .ForMember(dest => dest.Image, opt => opt.MapFrom(
                src => src.Image))
            .ForMember(dest => dest.Video, opt => opt.MapFrom(
                src => src.Video));
        }
    }
}
