namespace SoftUni
{
    using Data;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                string result = GetEmployeesByFirstNameStartingWithSa(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new
                {
                    FullName = $"{e.FirstName} {e.LastName}",
                    e.JobTitle,
                    e.Salary
                });

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FullName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
