using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication8.Models
{
    public interface IProduct
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetPreferred(int count);
        IEnumerable<Product> GetProductByCategoryId(int categoryId);
        IEnumerable<Product> GetFilteredProduct(int id, string searchQuery);
        IEnumerable<Product> GetFilteredProduct(string searchQuery);
        Product GetById(int id);
        void NewProduct(Product food);
        void EditProduct(Product food);
        void DeleteProduct(int id);
    }
}
