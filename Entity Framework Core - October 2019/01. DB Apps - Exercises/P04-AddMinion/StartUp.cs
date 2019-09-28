namespace P04_AddMinion
{
    using P01_InitialSetup;
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            string[] minionInfo = Console.ReadLine().Split();
            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string townName = minionInfo[3];

            string[] villainInfo = Console.ReadLine().Split();
            string villainName = villainInfo[1];

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                int? townId = GetTownId(connection, townName);

                if (townId == null)
                {
                    AddTown(connection, townName);
                    townId = GetTownId(connection, townName);
                }

                int? villainId = GetVillainId(connection, villainName);

                if (villainId == null)
                {
                    AddVillain(connection, villainName);
                    villainId = GetVillainId(connection, villainName);
                }

                AddMinion(connection, minionName, minionAge, (int)townId);
                int minionId = GetMinionId(connection, minionName);

                MakeMinionAServentOfVillain(connection, minionId, minionName, villainId, villainName);
            }
        }

        private static int? GetTownId(SqlConnection connection, string townName)
        {
            string getTownIdQuery = @"SELECT Id
                                       FROM Towns 
                                      WHERE Name = @townName";

            using (SqlCommand command = new SqlCommand(getTownIdQuery, connection))
            {
                command.Parameters.AddWithValue("@townName", townName);

                int? townId = (int?)command.ExecuteScalar();

                return townId;
            }
        }

        private static void AddTown(SqlConnection connection, string townName)
        {
            string insertTownQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";

            using (SqlCommand command = new SqlCommand(insertTownQuery, connection))
            {
                command.Parameters.AddWithValue("@townName", townName);

                int rowsAffected = (int)command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Town {townName} was added to the database.");
                }

                else
                {
                    throw new InvalidOperationException($"Something went wrong while attempting to add town {townName} to the database");
                }
            }
        }

        private static int? GetVillainId(SqlConnection connection, string villainName)
        {
            string getVillainIdQuery = @"SELECT Id 
                                           FROM Villains    
                                          WHERE Name = @villainName";

            using (SqlCommand command = new SqlCommand(getVillainIdQuery, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);
                int? villainId = (int?)command.ExecuteScalar();

                return villainId;
            }
        }

        private static void AddVillain(SqlConnection connection, string villainName)
        {
            string insertVillainQuery = @"INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@villainName, 4)";

            using (SqlCommand command = new SqlCommand(insertVillainQuery, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);

                int rowsAffected = (int)command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                else
                {
                    throw new InvalidOperationException($"Something went wrong while attempting to add villain {villainName} to the database");
                }
            }
        }

        private static void AddMinion(SqlConnection connection, string minionName, int minionAge,
            int townId)
        {
            string insertMinionQuery = @"INSERT INTO Minions(Name, Age, TownId) VALUES(@name, @age, @townId)";

            using (SqlCommand command = new SqlCommand(insertMinionQuery, connection))
            {
                command.Parameters.AddWithValue("@name", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townId);

                command.ExecuteNonQuery();
            }
        }

        private static int GetMinionId(SqlConnection connection, string minionName)
        {
            string getVillainIdQuery = @"SELECT Id 
                                           FROM Minions    
                                          WHERE Name = @minionName";

            using (SqlCommand command = new SqlCommand(getVillainIdQuery, connection))
            {
                command.Parameters.AddWithValue("@minionName", minionName);
                int minionId = (int)command.ExecuteScalar();

                return minionId;
            }
        }

        private static void MakeMinionAServentOfVillain(SqlConnection connection, int minionId,
            string minionName, int? villainId, string villainName)
        {
            string makeMinionAServent = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            using (SqlCommand command = new SqlCommand(makeMinionAServent, connection))
            {
                command.Parameters.AddWithValue("@minionId", minionId);
                command.Parameters.AddWithValue("@villainId", villainId);

                int rowsAffected = 0;

                try
                {
                    rowsAffected = (int)command.ExecuteNonQuery();
                }

                catch (Exception)
                {
                    Console.WriteLine($"{minionName} is already a slave of {villainName}");
                }

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                }
            }
        }
    }
}
