namespace ProductShop
{
    using AutoMapper;
    using Data;
    using Dtos.Import;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            string xmlUsers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\ProductShop\Datasets\users.xml");
            string xmlProducts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\ProductShop\Datasets\products.xml");
            string xmlCategories = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\ProductShop\Datasets\categories.xml");
            string xmlCategoryProducts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\ProductShop\Datasets\categories-products.xml");

            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var context = new ProductShopContext())
            {
                var result = ImportCategories(context, xmlCategories);
                Console.WriteLine(result);
            }
        }

        //Problem 1 - Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]),
                new XmlRootAttribute("Users"));

            var userDtos = (ImportUserDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var users = Mapper.Map<IEnumerable<ImportUserDto>, IEnumerable<User>>(userDtos);

            context.Users.AddRange(users);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 2 - Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]),
                new XmlRootAttribute("Products"));

            var productDtos = (ImportProductDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var products = Mapper.Map<IEnumerable<ImportProductDto>, IEnumerable<Product>>(productDtos);

            context.Products.AddRange(products);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 3 - Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]),
                new XmlRootAttribute("Categories"));

            var categoryDtos = (ImportCategoryDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var categories = Mapper.Map<IEnumerable<ImportCategoryDto>, IEnumerable<Category>>(categoryDtos);

            context.Categories.AddRange(categories);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 4 - Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDto[]),
                new XmlRootAttribute("CategoryProducts"));

            var categoryProductsDtos = (ImportCategoryProductsDto[])(xmlSerializer
                .Deserialize(new StringReader(inputXml)));

            var categoryProducts = Mapper.Map<IEnumerable<ImportCategoryProductsDto>, 
                IEnumerable<CategoryProduct>>(categoryProductsDtos);

            var categories = context.Categories.Select(c => c.Id);
            var products = context.Products.Select(p => p.Id);

            var validCategoryProducts = categoryProducts
                .Where(cp => categories.Contains(cp.CategoryId)
                 && products.Contains(cp.ProductId));

            context.CategoryProducts.AddRange(categoryProducts);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }
    }
}