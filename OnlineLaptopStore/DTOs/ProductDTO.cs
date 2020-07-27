﻿using OnlineLaptopStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ModelNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string Video { get; set; }
        public string CompanyName { get; set; }
        public string Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
