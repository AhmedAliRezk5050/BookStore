using AutoMapper;
using BookStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, UpsertProductViewModel>()
                .ForMember(dest => dest.CategoryId, src => src.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.CoverTypeId, src => src.MapFrom(src => src.CoverType.Id))
                .ReverseMap();
        }
    }
}
