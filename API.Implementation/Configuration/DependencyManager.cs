using API.Data;
using API.Data.Repositories;
using API.Domain;
using API.Implementation.Products;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Implementation.Configuration
{
    public class DependencyManager
    {
        public void InjectDependencies(IServiceCollection services)
        {


            #region Provider

            services.AddTransient<IProductProvider, ProductsProvider>();
            

            #endregion

            #region Repositories

            services.AddTransient<IProductRepository, ProductRepositoy>();
             

            #endregion
        }
    }
}
