using Microsoft.EntityFrameworkCore;
using OnlineLaptopStore.Data;
using OnlineLaptopStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.DataManagers
{
    // Created but not use this we can implement if we want sorting also.
    public class ProductManger
    {
        DbContextOptionsBuilder<ApplicationDbContext> _optionsBuilder;

        public ProductManger()
        {
            _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        }
        public List<Product> GetProducts(string SortColumn, string SearchText, int? Page)
        {
            using (var db = new ApplicationDbContext())
            {
                var data = db.Products.Where(x =>
                    x.Category.Contains(SearchText) ||
                    x.Color.Contains(SearchText) ||
                    x.CompanyName.Contains(SearchText) ||
                    x.Description.Contains(SearchText) ||
                    x.ModelNumber.Contains(SearchText) ||
                    x.Name.Contains(SearchText) ||
                    x.Video.Contains(SearchText));

                switch (SortColumn)
                {
                    case "Category":
                        data = data.OrderBy(s => s.Category);
                        break;
                    case "Color":
                        data = data.OrderBy(s => s.Color);
                        break;
                    case "CompanyName":
                        data = data.OrderBy(s => s.CompanyName);
                        break;
                    case "Description":
                        data = data.OrderBy(s => s.Description);
                        break;
                    case "ModelNumber":
                        data = data.OrderBy(s => s.ModelNumber);
                        break;
                    case "Name":
                        data = data.OrderBy(s => s.Name);
                        break;
                    case "Video":
                        data = data.OrderBy(s => s.Video);
                        break;
                    case "CreateDate":
                        data = data.OrderBy(s => s.CreateDate);
                        break;
                    default:  // Name ascending 
                        data = data.OrderBy(s => s.Name);
                        break;
                }

                return data.ToList();
            }
        }
    }
}
