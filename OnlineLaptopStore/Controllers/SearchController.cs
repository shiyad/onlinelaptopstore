using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using OnlineLaptopStore.Data;
using OnlineLaptopStore.DataManagers;
using OnlineLaptopStore.DTOs;
using OnlineLaptopStore.Models;

namespace OnlineLaptopStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IElasticClient _elasticClient;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IMapper mapper, IElasticClient elasticClient, ILogger<SearchController> logger)
        {
            _mapper = mapper;

            _elasticClient = elasticClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<SearchDTO> Get(string q, int from, int size)
        {
            if (string.IsNullOrEmpty(q))
            {
                var noResults = new SearchDTO { Term = "[No Search]" };
                return noResults;
            }

            var response = await _elasticClient.SearchAsync<ProductDTO>(s =>
                s.Query(sq =>
                    sq.MultiMatch(mm => mm
                        .Query(q)
                        .Fuzziness(Fuzziness.Auto)
                    )
                ).Size(10000).Scroll("5m")
            );

            var dispDTO = new SearchDTO {
                Term = q,
                Total = response.Total
            };

            if (response.IsValid)
                dispDTO.Results = response.Documents?.Skip(from).Take(size).ToList();
            else
                _logger.LogError(response.OriginalException, "Problem searching Elasticsearch for term {0}", q);

            return dispDTO;
        }
    }
}