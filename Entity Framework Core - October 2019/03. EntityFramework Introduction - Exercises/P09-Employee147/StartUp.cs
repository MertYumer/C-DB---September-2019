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
                string result = GetEmployee147(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employee = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(e => new
                {
                    FullName = e.FirstName + ' ' + e.LastName,
                    e.JobTitle,
                    ProjectsNames = e.EmployeesProjects
                        .Select(p => p.Project.Name)
                        .OrderBy(p => p)
                })
                .FirstOrDefault();

            stringBuilder.AppendLine($"{employee.FullName} - {employee.JobTitle}");

            foreach (var projectName in employee.ProjectsNames)
            {
                stringBuilder.AppendLine(projectName);
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
