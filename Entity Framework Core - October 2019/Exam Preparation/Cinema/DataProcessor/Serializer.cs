namespace Cinema.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using AutoMapper;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context
                .Movies
                .Where(m => m.Rating >= rating
                    && m.Projections.Any(p => p.Tickets.Any()))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Take(10)
                .ToArray();

            var movieDtos = movies
                .Select(m => new ExportMovieDto
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("f2"),

                    Customers = m.Projections.SelectMany(p => p.Tickets)
                    .Select(t => new ExportCustomerDto
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = t.Customer.Balance.ToString("f2")
                    })
                    .OrderByDescending(c => c.Balance)
                    .ThenBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToArray()
                })
                .ToArray();

            var jsonResult = JsonConvert.SerializeObject(movieDtos, Newtonsoft.Json.Formatting.Indented);

            return jsonResult;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context
                .Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Take(10)
                .ToArray();

            var customerDtos = Mapper.Map<ExportCustomerWithSpentMoneyDto[]>(customers);

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomerWithSpentMoneyDto[]),
                           new XmlRootAttribute("Customers"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), customerDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }
    }
}