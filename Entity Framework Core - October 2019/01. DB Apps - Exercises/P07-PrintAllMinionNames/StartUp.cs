namespace P07_PrintAllMinionNames
{
    using P01_InitialSetup;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            List<string> minions = GetMinionNames();

            for (int i = 0; i < minions.Count / 2; i++)
            {
                Console.WriteLine(minions[i]);
                Console.WriteLine(minions[minions.Count - 1 - i]);
            }

            if (minions.Count % 2 != 0)
            {
                Console.WriteLine(minions[minions.Count / 2]);
            }
        }

        private static List<string> GetMinionNames()
        {
            List<string> minions = new List<string>();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string getMinionNamesSql = @"SELECT Name FROM Minions";

                using (SqlCommand command = new SqlCommand(getMinionNamesSql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minions.Add((string)reader[0]);
                        }
                    }
                }
            }

            return minions;
        }
    }
}
