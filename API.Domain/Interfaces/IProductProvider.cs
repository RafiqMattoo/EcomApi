using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain
{
    public interface IProductProvider
    {
        public BaseResponse<List<ProductsResponse>> GetProductList(); 
        public BaseResponse<int> AddProduct(AddProduct agencyRequest);
        public BaseResponse<int> UpdateProduct(UpdateProduct agencyRequest);
        public BaseResponse<int> DeleteProduct(int productId);
    }
}
