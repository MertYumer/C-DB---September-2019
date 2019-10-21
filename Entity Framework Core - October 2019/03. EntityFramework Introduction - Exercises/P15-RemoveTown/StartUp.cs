namespace SoftUni
{
    using Data;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                string result = RemoveTown(context);
                Console.WriteLine(result);
            }
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToList();

            context.Employees
                .Where(e => addresses
                    .Any(a => a.AddressId == e.AddressId))
                .ToList()
                .ForEach(e => e.AddressId = null);

            context.Addresses.RemoveRange(addresses);

            var town = context.Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            context.Towns.Remove(town);
            context.SaveChanges();

            var result = $"{addresses.Count} addresses in Seattle were deleted";

            return result;
        }
    }
}
