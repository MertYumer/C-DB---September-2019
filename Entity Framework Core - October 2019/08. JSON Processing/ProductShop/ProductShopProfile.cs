namespace ProductShop
{
    using AutoMapper;
    using Dtos;
    using Models;
    using System.Collections.Generic;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<Product, ExportProductDto>()
               .ForMember(x => x.Seller, y => y.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            CreateMap<User, UserWithSalesDto>()
               .ForMember(x => x.SoldProducts, y => y.MapFrom(s => s.ProductsSold));
        }
    }
}
