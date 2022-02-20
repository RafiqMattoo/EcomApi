using API.Data.DBEntities;
using API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data.Repositories
{
    public class ProductRepositoy : IProductRepository
    {
        public int AddProduct(AddProduct request)
        {
            Guid guid = Guid.NewGuid(); Product product = new Product();
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Image = guid.ToString() + request.Image;
            product.CreatedDate = DateTime.Now;
            using (APIDbContext dbContext = new APIDbContext())
            {
                dbContext.Add(product);
               
                 dbContext.SaveChanges(); return product.ProductId;
            }
            
        }

        public int DeleteProduct(int productId)
        {
            using (APIDbContext dbContext = new APIDbContext())
            {
                var prod = dbContext.Products.FirstOrDefault(x => x.ProductId == productId);
                dbContext.Remove(prod);
                return dbContext.SaveChanges();
                
            }
        }

        public List<ProductsResponse> GetProducts()
        {
            using (APIDbContext dbContext = new APIDbContext())
            {
                return dbContext.Products.Select(x => new ProductsResponse()
                {
                     ProductId= x.ProductId,
                     Name=x.Name,
                     Price=x.Price,
                     Stock=x.Stock,
                     Image=x.Image,
                     CreatedDate= (DateTime)x.CreatedDate == null ? DateTime.Now: (DateTime)x.CreatedDate,
                     UpdatedDate= (DateTime)x.UpdatedDate == null ? DateTime.Now : (DateTime)x.UpdatedDate,
                }).ToList();
            }
        }

        public int UpdateProduct(UpdateProduct request)
        {
            Guid guid = Guid.NewGuid();
            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Image = guid.ToString() +  request.Image;
            product.UpdatedDate = DateTime.Now;
            using (APIDbContext dbContext = new APIDbContext())
            {
                dbContext.Update(product);
               dbContext.SaveChanges();
                return product.ProductId;
            }
            
        }

        public Product GetProduct(int productId)
        {

            using (APIDbContext dbContext = new APIDbContext())
            {

                return dbContext.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            }
        }
    }
}
