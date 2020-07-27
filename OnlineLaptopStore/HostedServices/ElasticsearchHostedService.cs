using Microsoft.Extensions.Hosting;
using Nest;
using OnlineLaptopStore.DTOs;
using OnlineLaptopStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLaptopStore.HostedServices
{
    public class ElasticsearchHostedService: IHostedService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticsearchHostedService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var productsIndexName = "products";

            if ((await _elasticClient.Indices.ExistsAsync(productsIndexName)).Exists)
                await _elasticClient.Indices.DeleteAsync(productsIndexName);

            var createMoviesIndexResponse = await _elasticClient.Indices.CreateAsync(productsIndexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            .Stop("english_stop", st => st
                                .StopWords("_english_")
                            )
                            .Stemmer("english_stemmer", st => st
                                .Language("english")
                            )
                            .Stemmer("light_english_stemmer", st => st
                                .Language("light_english")
                            )
                            .Stemmer("english_possessive_stemmer", st => st
                                .Language("possessive_english")
                            )
                            .Synonym("product_synonyms", st => st
                                .Synonyms(
                                    "haphazard,indiscriminate,erratic",
                                    "incredulity,amazement,skepticism")
                            )
                        )
                        .Analyzers(aa => aa
                            .Custom("light_english", ca => ca
                                .Tokenizer("standard")
                                .Filters("light_english_stemmer", "english_possessive_stemmer", "lowercase", "asciifolding")
                            )
                            .Custom("full_english", ca => ca
                                .Tokenizer("standard")
                                .Filters("english_possessive_stemmer",
                                        "lowercase",
                                        "english_stop",
                                        "english_stemmer",
                                        "asciifolding")
                            )
                            .Custom("full_english_synopsis", ca => ca
                                .Tokenizer("standard")
                                .Filters("product_synonyms",
                                        "english_possessive_stemmer",
                                        "lowercase",
                                        "english_stop",
                                        "english_stemmer",
                                        "asciifolding")
                            )
                        )
                    )
                ) // Only activated for name and description because of best performance
                .Map<ProductDTO>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t
                            .Name(n => n.Name)
                            .Analyzer("light_english")
                        )
                        .Text(t => t
                            .Name(n => n.Description)
                            .Analyzer("full_english_synopsis")
                        )
                    )
                )
            );
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
