namespace P03_MinionNames
{
    using P01_InitialSetup;
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            Console.Write("Enter Villain ID: ");
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainNameQuery = @"SELECT Name 
                                              FROM Villains 
                                             WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(villainNameQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {id} exists in the database.");
                        return;
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                string minionsQuery = @"  SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                                 m.Name, 
                                                 m.Age
                                            FROM MinionsVillains AS mv
                                            JOIN Minions As m ON mv.MinionId = m.Id
                                           WHERE mv.VillainId = @id
                                        ORDER BY m.Name";

                using (SqlCommand command = new SqlCommand(minionsQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                            return;
                        }

                        while (reader.Read())
                        {
                            long rowNumber = (long)reader["RowNum"];
                            string name = (string)reader["Name"];
                            int minionsCount = (int)reader["Age"];

                            Console.WriteLine($"{rowNumber}. {name} {minionsCount}");
                        }
                    }
                }
            }
        }
    }
}