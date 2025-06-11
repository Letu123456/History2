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
    public class EventMapping:Profile
    {
        public EventMapping()
        {
            CreateMap<Event, EventDto>()

            .ForMember(dest => dest.Image, opt => opt.MapFrom(
                src => src.Image));
           
        }
    }
}
