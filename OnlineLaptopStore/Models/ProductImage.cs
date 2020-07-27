using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.Models
{
    [ElasticsearchType(RelationName = "productImage", IdProperty = nameof(ImageId))]
    public class ProductImage
    {
        [Key]
        [Index]
        public long ImageId { get; set; }
        public string Image { get; set; }
        public Product Product { get; set; }
    }
}
