namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                string result = GetAddressesByTown(context);
                Console.WriteLine(result);
            }
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new
                {
                    Text = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                });

            foreach (var address in addresses)
            {
                stringBuilder.AppendLine($"{address.Text}, {address.TownName}" +
                    $" - {address.EmployeesCount} employees");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
