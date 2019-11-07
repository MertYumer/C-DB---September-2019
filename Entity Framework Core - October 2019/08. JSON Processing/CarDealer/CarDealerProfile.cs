namespace CarDealer
{
    using AutoMapper;
    using DTO;
    using Models;
    using System.Globalization;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<CarImportDto, Car>();

            CreateMap<Customer, CustomerDto>()
                .ForMember(x => x.BirthDate, y => y.MapFrom(c => c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Car, CarExportDto>();

            CreateMap<Supplier, SupplierDto>();

            CreateMap<Customer, CustomerWithSalesDto>();
        }
    }
}
