namespace CarDealer
{
    using AutoMapper;
    using Dtos.Import;
    using Data;
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            string xmlCars = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\cars.xml");
            string xmlCustomers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\customers.xml");
            string xmlParts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\parts.xml");
            string xmlSales = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\sales.xml");
            string xmlSuppliers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\09. XML Processing\CarDealer\Datasets\suppliers.xml");

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var context = new CarDealerContext())
            {
                var result = ImportSales(context, xmlSales);
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
        //Problem 15 - Cars from make BMW
        //Problem 16 - Local Suppliers
        //Problem 17 - Cars with Their List of Parts
        //Problem 18 - Total Sales by Customer
        //Problem 19 - Sales with Applied Discount
    }
}