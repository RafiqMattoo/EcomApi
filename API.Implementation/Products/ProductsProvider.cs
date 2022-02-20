using API.Data;
using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.Implementation.Products
{
    public class ProductsProvider : IProductProvider
    {
        private readonly IProductRepository _productRepository;
        public ProductsProvider(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public BaseResponse<int> AddProduct(AddProduct addRequest)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            try
            {

                var data = _productRepository.AddProduct(addRequest);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Sucess";
                response.Data = data;
                return response;
            }
            catch (Exception exp)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";

                return response;
            }
        }

        public  BaseResponse<int> DeleteProduct(int productId)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            try
            {
                var dataProduct = _productRepository.GetProduct(productId);
                if (dataProduct != null)
                {
                    var data = _productRepository.DeleteProduct(productId);
                
                    response.Code = (int)HttpStatusCode.OK;
                    response.Message = "Sucess";
                    response.Data = data;
                    return response;
                }
                else
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Not Found";                   
                    return response;
                }

            }
            catch (Exception exp)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";

                return response;
            }
        }

        public BaseResponse<List<ProductsResponse>> GetProductList()
        {
            BaseResponse<List<ProductsResponse>> response = new BaseResponse<List<ProductsResponse>>();
            try
            {

                var data = _productRepository.GetProducts();
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Sucess";
                response.Data = data;
                return response;
            }
            catch (Exception exp)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";

                return response;
            }
        }

       public  BaseResponse<int> UpdateProduct(UpdateProduct updateRequest)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            try
            {
                var dataProduct = _productRepository.GetProduct(updateRequest.ProductId);
                if (dataProduct != null)
                {
                    var data = _productRepository.UpdateProduct(updateRequest);
                    response.Code = (int)HttpStatusCode.OK;
                    response.Message = "Sucess";
                    response.Data = data;
                    return response;
                }
                else
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Not Found";
                    return response;
                }
            }
            catch (Exception exp)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";

                return response;
            }
        }
    }
}
