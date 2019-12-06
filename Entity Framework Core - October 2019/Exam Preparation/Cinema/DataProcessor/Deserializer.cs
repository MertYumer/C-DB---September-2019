namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using AutoMapper;
    using Cinema.Data.Models;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movieDtos = JsonConvert.DeserializeObject<List<ImportMovieDto>>(jsonString);
            var movies = new List<Movie>();

            var sb = new StringBuilder();

            foreach (var movieDto in movieDtos)
            {
                if (!IsValid(movieDto) || movies.Any(m => m.Title == movieDto.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = Mapper.Map<Movie>(movieDto);
                movies.Add(movie);

                sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, $"{movie.Rating:f2}"));
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallDtos = JsonConvert.DeserializeObject<ImportHallWithSeatsDto[]>(jsonString);
            var halls = new List<Hall>();

            var sb = new StringBuilder();
            string projectionType = string.Empty;

            foreach (var hallDto in hallDtos)
            {
                if (!IsValid(hallDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = Mapper.Map<Hall>(hallDto);

                for (int i = 0; i < hallDto.SeatsCount; i++)
                {
                    hall.Seats.Add(new Seat());
                }

                halls.Add(hall);

                if (hall.Is4Dx && hall.Is3D)
                {
                    projectionType = "4Dx/3D";
                }

                else if (hall.Is4Dx && !hall.Is3D)
                {
                    projectionType = "4Dx";
                }

                else if (!hall.Is4Dx && hall.Is3D)
                {
                    projectionType = "3D";
                }

                else
                {
                    projectionType = "Normal";
                }

                halls.Add(hall);
                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hallDto.SeatsCount));
            }

            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]),
                    new XmlRootAttribute("Projections"));

            var projectionDtos = (ImportProjectionDto[])(xmlSerializer.Deserialize(new StringReader(xmlString)));
            var projections = new List<Projection>();

            StringBuilder sb = new StringBuilder();

            foreach (var projectionDto in projectionDtos)
            {
                var isValidProjection = IsValid(projectionDto);
                var movie = context.Movies.FirstOrDefault(a => a.Id == projectionDto.MovieId);
                var hall = context.Halls.FirstOrDefault(w => w.Id == projectionDto.HallId);

                if (!isValidProjection || movie == null || hall == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projection = Mapper.Map<Projection>(projectionDto);
                projection.Movie = movie;
                projection.Hall = hall;
                projections.Add(projection);

                sb.AppendLine(string.Format(
                    SuccessfulImportProjection,
                    projection.Movie.Title,
                    projection.DateTime.ToString("MM/dd/yyyy")));
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerWithTicketsDto[]),
                    new XmlRootAttribute("Customers"));

            var customerDtos = (ImportCustomerWithTicketsDto[])(xmlSerializer.Deserialize(new StringReader(xmlString)));
            var customers = new List<Customer>();

            StringBuilder sb = new StringBuilder();

            foreach (var customerDto in customerDtos)
            {
                if (!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = Mapper.Map<Customer>(customerDto);

                customers.Add(customer);

                sb.AppendLine(string.Format(
                    SuccessfulImportCustomerTicket,
                    customer.FirstName,
                    customer.LastName,
                    customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return isValid;
        }
    }
}