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
                string result = IncreaseSalaries(context);
                Console.WriteLine(result);
            }
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees
                .Where(e => 
                e.Department.Name == "Engineering" || 
                e.Department.Name == "Tool Design" || 
                e.Department.Name == "Marketing" || 
                e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new
                {
                    FullName = $"{e.FirstName} {e.LastName}",
                    Salary = e.Salary * 1.12m
                });

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FullName} (${employee.Salary:f2})");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
