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
                .ReverseMap();

            CreateMap<Company, UpsertCompanyViewModel>()
                .ReverseMap();
        }
    }
}
