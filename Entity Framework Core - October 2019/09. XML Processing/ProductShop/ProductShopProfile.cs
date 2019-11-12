namespace ProductShop
{
    using AutoMapper;
    using Dtos.Import;
    using Models;
    using Dtos.Export;
    using System.Linq;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDto, User>();

            CreateMap<ImportProductDto, Product>();

            CreateMap<ImportCategoryDto, Category>();

            CreateMap<ImportCategoryProductsDto, CategoryProduct>();

            CreateMap<Product, ProductInRangeDto>()
                .ForMember(dest => dest.Buyer, 
                opt => opt.MapFrom(src => $"{src.Buyer.FirstName} {src.Buyer.LastName}"));

            CreateMap<Category, CategoryWithProductsDto>()
                .ForMember(dest => dest.Count,
                opt => opt.MapFrom(src => src.CategoryProducts.Count))
                .ForMember(dest => dest.AveragePrice,
                opt => opt.MapFrom(src => src.CategoryProducts.Average(cp => cp.Product.Price)))
                .ForMember(dest => dest.TotalRevenue,
                opt => opt.MapFrom(src => src.CategoryProducts.Sum(cp => cp.Product.Price)));
        }
    }
}
