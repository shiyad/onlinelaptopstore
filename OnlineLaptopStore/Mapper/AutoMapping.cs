using AutoMapper;
using OnlineLaptopStore.DTOs;
using OnlineLaptopStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.Mapper
{
    public class AutoMapping: Profile
    {
        public AutoMapping()
        {
            CreateMap<Product, ProductDTO>();
        }
    }
}
