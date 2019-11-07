namespace ProductShop
{
    using System;
    using System.IO;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;
    using Data;
    using Dtos;
    using Models;
    using AutoMapper;
    using System.Collections.Generic;
    using Newtonsoft.Json.Serialization;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            string jsonUsers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\ProductShop\Datasets\users.json");
            string jsonProducts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\ProductShop\Datasets\products.json");
            string jsonCategories = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\ProductShop\Datasets\categories.json");
            string jsonCategoryProducts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\ProductShop\Datasets\categories-products.json");

            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var context = new ProductShopContext())
            {
                var result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }

        //Problem 1 - Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        //Problem 2 - Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.Products.AddRange(products);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        //Problem 3 - Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson);
            var validCategories = categories
                .Where(x => x.Name != null
                && x.Name.Length >= 3
                && x.Name.Length <= 13)
                .ToArray();

            context.Categories.AddRange(validCategories);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        //Problem 4 - Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            var categoriesIds = context.Categories.Select(c => c.Id).ToList();
            var productsIds = context.Products.Select(p => p.Id).ToList();

            var validCategoriesProducts = categoriesProducts
                .Where(cp => categoriesIds.Contains(cp.CategoryId)
                 && productsIds.Contains(cp.ProductId));

            context.CategoryProducts.AddRange(validCategoriesProducts);
            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 5 - Query 5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductDto>()
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(productsInRange, Formatting.Indented);

            return jsonResult;
        }

        //Problem 6 - Query 6. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSales = context
                .Users
                .Where(u => u.ProductsSold.Count > 0
                && u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Include(u => u.ProductsSold)
                .ToList();

            var jsonExport = Mapper.Map<IEnumerable<User>,
                IEnumerable<UserWithSalesDto>>(usersWithSales);

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(jsonExport, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return jsonResult;
        }

        //Problem 7 - Query 7. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = $"{c.CategoryProducts.Average(cp => cp.Product.Price):f2}",
                    TotalRevenue = $"{c.CategoryProducts.Sum(cp => cp.Product.Price):f2}"
                });

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(categories, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return jsonResult;
        }

        //Problem 8 - Query 8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,

                    soldProducts = new
                    {
                        Count = u.ProductsSold
                        .Count(p => p.Buyer != null),

                        Products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            p.Name,
                            p.Price
                        })
                    }
                })
                .ToList();

            var resultUsers = new
            {
                UsersCount = users.Count,
                Users = users
            };


            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(resultUsers, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return jsonResult;
        }
    }
}