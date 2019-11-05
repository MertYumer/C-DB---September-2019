namespace SoftUni
{
    using Data;
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
                string result = GetLatestProjects(context);
                Console.WriteLine(result);
            }
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var projects = context.Projects
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name);

            foreach (var project in projects)
            {
                stringBuilder.AppendLine(project.Name);
                stringBuilder.AppendLine(project.Description);

                string format = "M/d/yyyy h:mm:ss tt";
                string startDate = project.StartDate
                    .ToString(format, CultureInfo.InvariantCulture);

                stringBuilder.AppendLine(startDate);
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
