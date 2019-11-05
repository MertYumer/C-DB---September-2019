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
                string result = GetDepartmentsWithMoreThan5Employees(context);
                Console.WriteLine(result);
            }
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    ManagerFullName = d.Manager.FirstName + " " + d.Manager.LastName,
                    Employees = d.Employees
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Select(e => new
                    {
                        FullName = e.FirstName + " " + e.LastName,
                        e.JobTitle
                    })
                });

            foreach (var department in departments)
            {
                stringBuilder.AppendLine($"{department.Name} - {department.ManagerFullName}");

                foreach (var employee in department.Employees)
                {
                    stringBuilder.AppendLine($"{employee.FullName} - {employee.JobTitle}");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
