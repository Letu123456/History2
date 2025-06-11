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
    public class BlogMapping:Profile
    {
        public BlogMapping()
        {
            CreateMap<Blog, BlogDto>()

            .ForMember(dest => dest.Image, opt => opt.MapFrom(
                src => src.Image));
            
        }
    }
}
