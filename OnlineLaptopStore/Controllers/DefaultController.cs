using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using OnlineLaptopStore.Data;
using OnlineLaptopStore.DTOs;
using OnlineLaptopStore.Models;
using Microsoft.Extensions.Configuration;

namespace OnlineLaptopStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<DefaultController> _logger;
        private readonly IMapper _mapper;

        public DefaultController(IElasticClient elasticClient, ILogger<DefaultController> logger, IMapper mapper)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<long> Get()
        {
            var productsData = GetProducts();

            List<ProductDTO> products = _mapper.Map<List<ProductDTO>>(productsData);


            foreach (var product in products)
            {

                var existsResponse = await _elasticClient.DocumentExistsAsync<ProductDTO>(product);

                // If the document already exists, we're going to update it; otherwise insert it
                if (existsResponse.IsValid && existsResponse.Exists)
                {
                    var updateResponse = await _elasticClient.UpdateAsync<ProductDTO>(product, u => u.Doc(product));

                    if (!updateResponse.IsValid)
                    {
                        var errorMsg = "Problem updating document in Elasticsearch.";
                        _logger.LogError(updateResponse.OriginalException, errorMsg);
                        throw new Exception(errorMsg);
                    }
                }
                else
                {
                    var insertResponse = await _elasticClient.IndexDocumentAsync(product);

                    if (!insertResponse.IsValid)
                    {
                        var errorMsg = "Problem inserting document to Elasticsearch.";
                        _logger.LogError(insertResponse.OriginalException, errorMsg);
                        throw new Exception(errorMsg);
                    }

                }
            }

            var countResponse = _elasticClient.Count<ProductDTO>(p => p
                .Index("products")
            );

            return countResponse.Count;
        }

        public List<Product> GetProducts()
        {
            using (var db = new ApplicationDbContext())
            {
                var data = db.Products.ToList();
                return data;
            }
        }
    }
}