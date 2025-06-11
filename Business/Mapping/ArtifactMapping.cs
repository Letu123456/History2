using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Business.Mapping
{
    public class ArtifactMapping : Profile
    {
        public ArtifactMapping()
        {
            CreateMap<Artifact, ArtifactDetailDto>()

            .ForMember(dest => dest.Images, opt => opt.MapFrom(
                src => src.Images));
        }


    }
    }

