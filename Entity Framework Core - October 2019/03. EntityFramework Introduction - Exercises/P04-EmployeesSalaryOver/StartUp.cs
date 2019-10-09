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
                string result = GetEmployeesWithSalaryOver50000(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName);

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
