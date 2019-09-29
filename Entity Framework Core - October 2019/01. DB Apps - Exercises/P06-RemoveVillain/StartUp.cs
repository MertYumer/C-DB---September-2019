namespace P06_RemoveVillain
{
    using P01_InitialSetup;
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            int villainId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainName = GetVillainNameById(connection, villainId);

                if (villainName == null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }

                int minionsReleased = ReleaseMinions(connection, villainId);
                DeleteVillain(connection, villainId, villainName);
                Console.WriteLine($"{minionsReleased} minions were released.");
            }
        }

        private static string GetVillainNameById(SqlConnection connection, int villainId)
        {
            string getVillainNameQuery = @"SELECT Name 
                                             FROM Villains 
                                            WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(getVillainNameQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                return (string)command.ExecuteScalar();
            }
        }

        private static int ReleaseMinions(SqlConnection connection, int villainId)
        {
            string deleteFromMinionsVillainsQuery = @"DELETE FROM MinionsVillains 
                                                       WHERE VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(deleteFromMinionsVillainsQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                return command.ExecuteNonQuery();
            }
        }

        private static void DeleteVillain(SqlConnection connection, int villainId, string villainName)
        {
            string deleteVillainQuery = @"DELETE FROM Villains
                                           WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("Deleting villain failed.");
                }

                Console.WriteLine($"{villainName} was deleted.");
            }
        }
    }
}
