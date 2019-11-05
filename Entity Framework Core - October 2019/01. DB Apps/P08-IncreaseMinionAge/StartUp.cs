namespace P08_IncreaseMinionAge
{
    using P01_InitialSetup;
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            var minionsIds = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                foreach (var id in minionsIds)
                {
                    UpdateMinion(connection, id);
                }

                GetAllMinions(connection);
            }
        }

        private static void UpdateMinion(SqlConnection connection, int id)
        {
            string updateMinionQuery = @"UPDATE Minions
                                            SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), 
                                                Age += 1
                                          WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(updateMinionQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        private static void GetAllMinions(SqlConnection connection)
        {
            string getAllMinionsQuery = @"SELECT Name, 
                                                 Age 
                                            FROM Minions";

            using (SqlCommand command = new SqlCommand(getAllMinionsQuery, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = (string)reader[0];
                        int age = (int)reader[1];

                        Console.WriteLine($"{name} - {age}");
                    }
                }
            }
        }
    }
}
