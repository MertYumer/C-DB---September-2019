namespace CarDealer
{
    using AutoMapper;
    using Dtos.Import;
    using Models;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierDto, Supplier>();

            CreateMap<PartDto, Part>();

            CreateMap<CarDto, Car>();

            CreateMap<CustomerDto, Customer>();

            CreateMap<SaleDto, Sale>();
        }
    }
}
