using API.Data.DBEntities;
using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data
{
    public interface IProductRepository
    {
        public List<ProductsResponse> GetProducts();
        public int AddProduct(AddProduct request);
        public int UpdateProduct(UpdateProduct request);
        public int DeleteProduct(int productId);
        public Product GetProduct(int productId);
    }
}
