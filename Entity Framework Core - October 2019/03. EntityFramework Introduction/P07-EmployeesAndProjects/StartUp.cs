namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                string result = GetEmployeesInPeriod(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                    .Any(p => p.Project.StartDate.Year >= 2001 
                    && p.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    ManagerFullName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Projects = e.EmployeesProjects
                    .Select(p => new
                    {
                        p.Project.Name,
                        p.Project.StartDate,
                        p.Project.EndDate
                    })
                });

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FullName} - Manager: {employee.ManagerFullName}");

                foreach (var project in employee.Projects)
                {
                    string format = "M/d/yyyy h:mm:ss tt";
                    string startDate = project.StartDate
                        .ToString(format, CultureInfo.InvariantCulture);

                    string endDate = project.EndDate != null 
                        ? project.EndDate.Value.ToString(format, CultureInfo.InvariantCulture) 
                        : "not finished";

                    stringBuilder.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
