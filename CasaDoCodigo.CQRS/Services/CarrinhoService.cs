﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.Services
{
    public class CarrinhoService : BaseHttpService, ICarrinhoService
    {
        class CarrinhoUris
        {
            public static string GetCarrinho => "api/carrinho";
            public static string PostItem => "api/carrinho";
        }

        private readonly ILogger<CarrinhoService> _logger;

        public CarrinhoService(
            IConfiguration configuration
            , IHttpClientFactory httpClientFactory
            , HttpClient httpClient
            , IHttpContextAccessor contextAccessor
            , ILogger<CarrinhoService> logger)
            : base(configuration, httpClientFactory, httpClient, contextAccessor)
        {
            _logger = logger;
            _baseUri = _configuration["CarrinhoUrl"];
        }

        public async Task<CarrinhoViewModel> GetCarrinho(string userId)
        {
            return await GetAsync<CarrinhoViewModel>(CarrinhoUris.GetCarrinho, userId);
        }

        public async Task<CarrinhoViewModel> UpdateItem(string clienteId, ItemCarrinho input)
        {
            var uri = $"{CarrinhoUris.PostItem}/{clienteId}";
            return await PostAsync<CarrinhoViewModel>(uri, input);
        }
    }
}