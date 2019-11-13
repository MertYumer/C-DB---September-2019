namespace CarDealer
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Dtos.Export;
    using Dtos.Import;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            //string xmlCars = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\cars.xml");
            //string xmlCustomers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\customers.xml");
            //string xmlParts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\parts.xml");
            //string xmlSales = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\sales.xml");
            //string xmlSuppliers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\suppliers.xml");

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var context = new CarDealerContext())
            {
                var result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
        }

        //Problem 9 - Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SupplierDto[]),
                    new XmlRootAttribute("Suppliers"));

            var supplierDtos = (SupplierDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var suppliers = Mapper.Map<IEnumerable<Supplier>>(supplierDtos);

            context.Suppliers.AddRange(suppliers);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 10 - Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(PartDto[]),
                        new XmlRootAttribute("Parts"));

            var partDtos = (PartDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var parts = Mapper.Map<IEnumerable<Part>>(partDtos);

            var supplierIds = context.Suppliers.Select(x => x.Id).ToHashSet();
            var validParts = parts.Where(p => supplierIds.Contains(p.SupplierId));

            context.Parts.AddRange(validParts);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 11 - Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CarDto[]),
                            new XmlRootAttribute("Cars"));

            var carDtos = (CarDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var cars = new List<Car>();

            foreach (var carDto in carDtos)
            {
                var car = Mapper.Map<Car>(carDto);

                foreach (var part in carDto.Parts)
                {
                    var partCarExists = car
                        .PartCars
                        .FirstOrDefault(p => p.PartId == part.PartId) != null;

                    if (!partCarExists && context.Parts.Any(p => p.Id == part.PartId))
                    {
                        var partCar = new PartCar
                        {
                            CarId = car.Id,
                            PartId = part.PartId
                        };

                        car.PartCars.Add(partCar);
                    }
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {context.Cars.ToList().Count}";
        }

        //Problem 12 - Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CustomerDto[]),
                            new XmlRootAttribute("Customers"));

            var customerDtos = (CustomerDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var customers = Mapper.Map<IEnumerable<Customer>>(customerDtos);

            context.Customers.AddRange(customers);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 13 - Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SaleDto[]),
                            new XmlRootAttribute("Sales"));

            var saleDtos = (SaleDto[])(xmlSerializer.Deserialize(new StringReader(inputXml)));
            var carsIds = context.Cars.Select(c => c.Id);
            var validSales = new List<SaleDto>();

            foreach (var sale in saleDtos)
            {
                if (carsIds.Contains(sale.CarId))
                {
                    validSales.Add(sale);
                }
            }

            var sales = Mapper.Map<IEnumerable<Sale>>(validSales);

            context.Sales.AddRange(sales);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 14 - Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carDtos = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<CarWithDistanceDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarWithDistanceDto[]),
                            new XmlRootAttribute("cars"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), carDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 15 - Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var carDtos = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<CarFromMakeBMWDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarFromMakeBMWDto[]),
                            new XmlRootAttribute("cars"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), carDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 16 - Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplierDtos = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<LocalSupplierDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(LocalSupplierDto[]),
                            new XmlRootAttribute("suppliers"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), supplierDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 17 - Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carDtos = context
                    .Cars
                    .OrderByDescending(c => c.TravelledDistance)
                    .ThenBy(c => c.Model)
                    .Take(5)
                    .Select(c => new CarWithPartsDto
                    { 
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance,
                        Parts = c.PartCars.Select(pc => new ExportPartDto
                        { 
                            Name = pc.Part.Name,
                            Price = pc.Part.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    })
                    .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarWithPartsDto[]),
                            new XmlRootAttribute("cars"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), carDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 18 - Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerDtos = context
                .Customers
                .Where(c => c.Sales.Any())
                .ProjectTo<CustomerWithSpentMoneyDto>()
                .OrderByDescending(cdto => cdto.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CustomerWithSpentMoneyDto[]),
                            new XmlRootAttribute("customers"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), customerDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        //Problem 19 - Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var saleDtos = context
                .Sales
                .ProjectTo<SaleWithDiscountDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SaleWithDiscountDto[]),
                            new XmlRootAttribute("sales"));

            var stringBuilder = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(stringBuilder), saleDtos, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }
    }
}