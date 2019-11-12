namespace ProductShop
{
    using AutoMapper;
    using Data;
    using Dtos.Import;
    using Models;
    using Dtos.Export;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using AutoMapper.QueryableExtensions;
    using System.Text;
    using System.Xml;

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
                var result = GetUsersWithProducts(context);
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

        //Problem 5 - Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productDtos = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ProductInRangeDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ProductInRangeDto[]),
                new XmlRootAttribute("Products"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), productDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 6 - Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var userDtos = context
                .Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<GetSoldProductsDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(GetSoldProductsDto[]),
                new XmlRootAttribute("Users"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), userDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 7 - Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoryDtos = context
                .Categories
                .ProjectTo<CategoryWithProductsDto>()
                .OrderByDescending(cwp => cwp.Count)
                .ThenBy(cwp => cwp.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CategoryWithProductsDto[]),
                new XmlRootAttribute("Categories"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), categoryDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 8 - Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var userDtos = context
                    .Users
                    .Where(u => u.ProductsSold.Any())
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Select(u => new UsersWithProductsDto
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        ProductsSold = new SoldProductsWithCountDto
                        { 
                            Count = u.ProductsSold.Count,
                            Products = u.ProductsSold
                            .OrderByDescending(ps => ps.Price)
                            .Select(ps => new SoldProductDto
                            { 
                                Name = ps.Name,
                                Price = ps.Price
                            })
                            .ToArray()
                        }
                    })
                    .Take(10)
                    .ToArray();

            var result = new GetCountAndUsersWithProductsDto
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = userDtos
            };

            var xmlSerializer = new XmlSerializer(typeof(GetCountAndUsersWithProductsDto),
                new XmlRootAttribute("Users"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), result, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }
    }
}