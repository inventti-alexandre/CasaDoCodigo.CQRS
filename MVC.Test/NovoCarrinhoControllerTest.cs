﻿using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class NovoCarrinhoControllerTest : BaseControllerTest
    {
        private readonly Mock<IHttpContextAccessor> contextAccessorMock;
        private readonly Mock<IIdentityParser<ApplicationUser>> appUserParserMock;
        private readonly Mock<ILogger<CarrinhoController>> loggerMock;
        private readonly Mock<ICatalogoService> catalogoServiceMock;
        private readonly Mock<ICarrinhoService> carrinhoServiceMock;

        public NovoCarrinhoControllerTest()
        {
            contextAccessorMock = new Mock<IHttpContextAccessor>();
            appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            loggerMock = new Mock<ILogger<CarrinhoController>>();
            catalogoServiceMock = new Mock<ICatalogoService>();
            carrinhoServiceMock = new Mock<ICarrinhoService>();
        }

       [Fact]
        public async Task CarrinhoController_Index()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            catalogoServiceMock
                .Setup(c => c.GetProduto(testProduct.Codigo))
                .ReturnsAsync(testProduct);

            var itemCarrinho = new ItemCarrinho(testProduct.Codigo, testProduct.Codigo, testProduct.Nome, testProduct.Preco, 1, testProduct.UrlImagem);
            carrinhoServiceMock
                .Setup(c => c.AddItem(clienteId, It.IsAny<ItemCarrinho>()))
                .ReturnsAsync(
                new CarrinhoCliente(clienteId, 
                    new List<ItemCarrinho>
                    {
                        itemCarrinho
                    }));
                
            //act
            var controller = new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object);
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim("sub", clienteId) }
                ));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.Index(testProduct.Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult.Model);
        }       
    }
}