namespace P05_ChangeTownNamesCase
{
    using P01_InitialSetup;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            string countryName = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string updateTownsQuery = @"UPDATE Towns
                                               SET Name = UPPER(Name)
                                             WHERE CountryCode = (SELECT c.Id 
                                                                    FROM Countries AS c 
                                                                   WHERE c.Name = @countryName)";

                using (SqlCommand command = new SqlCommand(updateTownsQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", countryName);
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                    }

                    else
                    {
                        Console.WriteLine($"{affectedRows} town names were affected.");
                        PrintTownNames(connection, countryName);
                    }
                }
            }
        }

        private static void PrintTownNames(SqlConnection connection, string countryName)
        {
            string getTownsNamesQuery = @"SELECT t.Name 
                                       FROM Towns as t
                                       JOIN Countries AS c 
                                         ON c.Id = t.CountryCode
                                      WHERE c.Name = @countryName";

            using (SqlCommand command = new SqlCommand(getTownsNamesQuery, connection))
            {
                command.Parameters.AddWithValue("@countryName", countryName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var towns = new List<string>();

                    while (reader.Read())
                    {
                        towns.Add((string)reader[0]);
                    }

                    Console.WriteLine($"[{string.Join(", ", towns)}]");
                }
            }
        }
    }
}
