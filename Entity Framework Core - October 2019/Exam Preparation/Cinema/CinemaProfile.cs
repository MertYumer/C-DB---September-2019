namespace Cinema
{
    using System;
    using System.Linq;

    using AutoMapper;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ExportDto;
    using Cinema.DataProcessor.ImportDto;

    public class CinemaProfile : Profile
    {
        public CinemaProfile()
        {
            this.CreateMap<ImportMovieDto, Movie>()
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => (Genre)Enum.Parse(typeof(Genre), src.Genre)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.Parse(src.Duration)));

            this.CreateMap<ImportHallWithSeatsDto, Hall>();

            this.CreateMap<ImportProjectionDto, Projection>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => DateTime.Parse(src.DateTime)));

            this.CreateMap<ImportTicketDto, Ticket>();

            this.CreateMap<ImportCustomerWithTicketsDto, Customer>();

            this.CreateMap<Customer, ExportCustomerWithSpentMoneyDto>()
                .ForMember(dest => dest.SpentMoney, 
                opt => opt.MapFrom(src => src.Tickets.Sum(t => t.Price).ToString("f2")))
                .ForMember(dest => dest.SpentTime, 
                opt => opt.MapFrom(src => TimeSpan.FromMilliseconds(
                    src.Tickets.Sum(t => t.Projection.Movie.Duration.TotalMilliseconds)).ToString(@"hh\:mm\:ss")));
        }
    }
}
