namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DTO;
    using Data;
    using Models;
    using Newtonsoft.Json;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            string jsonSuppliers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\CarDealer\Datasets\suppliers.json");
            string jsonParts = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\CarDealer\Datasets\parts.json");
            string jsonCars = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\CarDealer\Datasets\cars.json");
            string jsonCustomers = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\CarDealer\Datasets\customers.json");
            string jsonSales = File.ReadAllText(@"D:\Projects\C# DB - September 2019\Entity Framework Core - October 2019\08. JSON Processing\CarDealer\Datasets\sales.json");

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var context = new CarDealerContext())
            {
                var result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
        }

        //Problem 9 - Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.Suppliers.AddRange(suppliers);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}.";
        }

        //Problem 10 - Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);
            var supplierIds = context
                .Suppliers
                .Select(s => s.Id)
                .ToList();

            var validParts = new List<Part>();

            foreach (var part in parts)
            {
                if (supplierIds.Contains(part.SupplierId))
                {
                    validParts.Add(part);
                }
            }

            context.Parts.AddRange(validParts);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}.";
        }

        //Problem 11 - Import Cars 
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsImport = JsonConvert.DeserializeObject<CarImportDto[]>(inputJson);
            var carsToAdd = Mapper.Map<CarImportDto[], Car[]>(carsImport);

            context.AddRange(carsToAdd);
            context.SaveChanges();

            HashSet<int> partIds = context.Parts.Select(x => x.Id).ToHashSet();

            HashSet<PartCar> carPartsToAdd = new HashSet<PartCar>();

            foreach (var car in carsImport)
            {
                car.PartsId = car
                    .PartsId
                    .Distinct()
                    .ToList();

                Car currentCar = context.
                    Cars
                    .Where(x => x.Make == car.Make
                    && x.Model == car.Model
                    && x.TravelledDistance == car.TravelledDistance)
                    .FirstOrDefault();

                if (currentCar == null)
                {
                    continue;
                }

                foreach (var id in car.PartsId)
                {
                    if (!partIds.Contains(id))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar
                    {
                        CarId = currentCar.Id,
                        PartId = id
                    };

                    if (!carPartsToAdd.Contains(partCar))
                    {
                        carPartsToAdd.Add(partCar);
                    }
                }

                if (carPartsToAdd.Count > 0)
                {
                    currentCar.PartCars = carPartsToAdd;
                    context.PartCars.AddRange(carPartsToAdd);
                    carPartsToAdd.Clear();
                }
            }

            context.SaveChanges();

            return $"Successfully imported {context.Cars.ToList().Count}.";
        }

        //Problem 12 - Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.Customers.AddRange(customers);
            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }

        //Problem 13 - Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.Sales.AddRange(sales);
            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }

        //Problem 14 - Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .ToList();

            var customerDtos = Mapper.Map<IEnumerable<Customer>,
                IEnumerable<CustomerDto>>(customers);

            var jsonResult = JsonConvert.SerializeObject(customerDtos, Formatting.Indented);

            return jsonResult;
        }

        //Problem 15 - Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();

            var carDtos = Mapper.Map<IEnumerable<Car>,
                IEnumerable<CarExportDto>>(cars);

            var jsonResult = JsonConvert.SerializeObject(carDtos, Formatting.Indented);

            return jsonResult;
        }

        //Problem 16 - Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .ToList();

            var supplierDtos = Mapper.Map<IEnumerable<Supplier>,
                IEnumerable<SupplierDto>>(suppliers);

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(supplierDtos, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return jsonResult;
        }

        //Problem 17 - Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context
                .Cars
                .Select(x => new
                {
                    car = new
                    {
                        x.Make,
                        x.Model,
                        x.TravelledDistance
                    },

                    parts = x.PartCars
                    .Select(pc => new
                    {
                        pc.Part.Name,
                        Price = $"{pc.Part.Price:F2}"
                    })
                })
                .ToList();

            string jsonOutput = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);

            return jsonOutput;
        }

        //Problem 18 - Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersWithSales = context
                .Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerWithSalesDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(customersWithSales, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return jsonResult;
        }

        //Problem 19 - Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance
                    },

                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    priceWithDiscount =
                    $"{s.Car.PartCars.Sum(pc => pc.Part.Price) - s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100m):f2}"
                })
                .Take(10)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return jsonResult;
        }
    }
}