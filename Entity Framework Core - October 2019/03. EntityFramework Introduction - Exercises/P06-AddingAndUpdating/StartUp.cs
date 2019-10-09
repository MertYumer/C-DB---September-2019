namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                string result = AddNewAddressToEmployee(context);
                Console.WriteLine(result);
            }
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .FirstOrDefault(x => x.LastName == "Nakov");

            employee.Address = address;
            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText);

            foreach (var currentAddress in addresses)
            {
                stringBuilder.AppendLine(currentAddress);
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
