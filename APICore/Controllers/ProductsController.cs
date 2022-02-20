using  API.Core;
using API.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductProvider _productProvider;
        public ProductsController(IProductProvider productProvider)
        {
            _productProvider = productProvider;
        }
        [HttpGet("GetProductsList")]
        public BaseResponse<List<ProductsResponse>> GetProductsList()
        {
            return _productProvider.GetProductList();
        }

        [HttpPost("AddProduct")]
        public BaseResponse<int> AddProduct([FromBody] AddProduct  product)
        {
            return _productProvider.AddProduct(product);
        }

        [HttpDelete("DeleteProduct")]
        public BaseResponse<int> DeleteProduct([FromQuery] int productId)
        {
            return _productProvider.DeleteProduct(productId);
        }

        [HttpPut("UpdateProduct")]
        public BaseResponse<int> UpdateProduct([FromBody] UpdateProduct product)
        {
            return _productProvider.UpdateProduct(product);
        }
    }
}
