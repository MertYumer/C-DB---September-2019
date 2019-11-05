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
                string result = DeleteProjectById(context);
                Console.WriteLine(result);
            }
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            int projectId = 2;

            var employeesProjects = context.EmployeesProjects
                .Where(ep => ep.ProjectId == projectId);

            foreach (var employeeProject in employeesProjects)
            {
                context.EmployeesProjects.Remove(employeeProject);
            }

            var project = context.Projects.Find(projectId);
            context.Projects.Remove(project);
            context.SaveChanges();

            var projectsNames = context.Projects
                .Take(10)
                .Select(p => p.Name);

            foreach (var projectName in projectsNames)
            {
                stringBuilder.AppendLine(projectName);
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
