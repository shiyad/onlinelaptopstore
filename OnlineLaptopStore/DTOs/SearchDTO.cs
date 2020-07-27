using OnlineLaptopStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.DTOs
{
    public class SearchDTO
    {
        public string Term { get; set; }
        public long Total { get; set; }
        public List<ProductDTO> Results { get; set; }
    }
}
