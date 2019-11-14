namespace CarDealer
{
    using AutoMapper;
    using CarDealer.Dtos.Export;
    using Dtos.Import;
    using Models;
    using System.Linq;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierDto, Supplier>();

            CreateMap<PartDto, Part>();

            CreateMap<CarDto, Car>();

            CreateMap<CustomerDto, Customer>();

            CreateMap<SaleDto, Sale>();

            CreateMap<Customer, CustomerWithSpentMoneyDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BoughtCars, opt => opt.MapFrom(src => src.Sales.Count))
                .ForMember(dest => dest.SpentMoney,
                opt => opt.MapFrom(src => src.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))));

            CreateMap<Sale, SaleWithDiscountDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Car.PartCars.Sum(pc => pc.Part.Price)))
                .ForMember(dest => dest.PriceWithDiscount,
                opt => opt.MapFrom(src =>
                    src.Car.PartCars.Sum(pc => pc.Part.Price)
                    - (src.Car.PartCars.Sum(pc => pc.Part.Price) * src.Discount / 100)));
        }
    }
}
