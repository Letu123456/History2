using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.DTO;
using Business.Model;

namespace Business.Mapping
{
    public class HistoricalMapping:Profile
    {
        public HistoricalMapping()
        {
            CreateMap<Historical, HistoricalDto>()

            .ForMember(dest => dest.Image, opt => opt.MapFrom(
                src => src.Image))
            .ForMember(dest => dest.Video, opt => opt.MapFrom(
                src => src.Video))
            .ForMember(dest => dest.Podcast, opt => opt.MapFrom(
                src => src.Podcast));







        }
    }
}
