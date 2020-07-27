﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using OnlineLaptopStore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLaptopStore.Extensions
{
    public static class ElasticsearchExtensions
    {
        
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

            var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];

            if (!string.IsNullOrEmpty(defaultIndex))
                settings = settings.DefaultIndex(defaultIndex);

            // The authentication options below are set if you have non-null/empty
            // settings in the configuration.
            var apiKeyId = configuration["ElasticsearchSettings:apiKeyId"];
            var apiKey = configuration["ElasticsearchSettings:apiKey"];

            if (!string.IsNullOrEmpty(apiKeyId) && !string.IsNullOrEmpty(apiKey))
            {
                settings = settings.ApiKeyAuthentication(apiKeyId, apiKey);
            }
            else
            {
                var basicAuthUser = configuration["ElasticsearchSettings:basicAuthUser"];
                var basicAuthPassword = configuration["ElasticsearchSettings:basicAuthPassword"];

                if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
                    settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);
            }

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client); // It's Thread safe
        }
    }
}
