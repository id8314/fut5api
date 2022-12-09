using Microsoft.Data.Sqlite;

namespace fut5.Data
{
    public class AtletasDatabaseService
    {
        private readonly dbPathService _dependency;
        public AtletasDatabaseService(dbPathService dbps)
        {
            _dependency = dbps;
        }
        public async Task<List<Atleta>>GetAtletas()
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;

            if(db_file == "")
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection("" +
                        new SqliteConnectionStringBuilder
                        {
                            DataSource = db_file
                        }))
                        {
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {

                                var selectCommand = connection.CreateCommand();
                                selectCommand.Transaction = transaction;
                                selectCommand.CommandText = "SELECT nome,email,pass,ativo,admin FROM Atletas ORDER BY nome";
                                using (var reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        atletas.Add(new Atleta(
                                            reader.GetString(0),reader.GetString(1),
                                            reader.GetString(2),reader.GetBoolean(3),
                                            reader.GetBoolean(4)
                                        ));
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                });
            }

            // return Task.FromResult(atletas.ToArray<Atleta>());
            return atletas.ToList<Atleta>();
        }
        public async Task<List<Atleta>>GetAtleta(String email)
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection("" +
                    new SqliteConnectionStringBuilder
                    {
                        DataSource = db_file
                    }))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            var selectCommand = connection.CreateCommand();
                            selectCommand.Transaction = transaction;
                            selectCommand.CommandText = $"SELECT nome,email,pass,ativo,admin FROM Atletas WHERE email='{email}'";
                            using (var reader = selectCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    atletas.Add(new Atleta(
                                        reader.GetString(0),reader.GetString(1),
                                        reader.GetString(2),reader.GetBoolean(3),
                                        reader.GetBoolean(4)
                                    ));
                                }
                            }
                            transaction.Commit();
                        }
                    }
                });
            }

            return atletas.ToList<Atleta>();

        }
        public async Task<List<Atleta>>AtletaSave(Atleta atleta)
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection("" +
                    new SqliteConnectionStringBuilder
                    {
                        DataSource = db_file
                    }))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {

                            var deleteCommand = connection.CreateCommand();
                            deleteCommand.Transaction = transaction;
                            deleteCommand.CommandText = "DELETE FROM Atletas WHERE email = $email";
                            deleteCommand.Parameters.AddWithValue("$email", atleta.Email);
                            deleteCommand.ExecuteNonQuery();
                            
                            var insertCommand = connection.CreateCommand();
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = "INSERT INTO Atletas (nome,email,pass,ativo,admin) VALUES ($nome,$email,$pass,$ativo,$admin)";
                            insertCommand.Parameters.AddWithValue("$nome", atleta.Nome);
                            insertCommand.Parameters.AddWithValue("$email", atleta.Email);
                            insertCommand.Parameters.AddWithValue("$pass", atleta.Pass);
                            insertCommand.Parameters.AddWithValue("$ativo", atleta.Ativo);
                            insertCommand.Parameters.AddWithValue("$admin", atleta.IsAdmin);
                            insertCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }

                        var selectCommand = connection.CreateCommand();
                        selectCommand.CommandText = $"SELECT nome,email,pass,ativo,admin FROM Atletas WHERE email='{atleta.Email}'";
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                atletas.Add(new Atleta(
                                    reader.GetString(0),reader.GetString(1),
                                    reader.GetString(2),reader.GetBoolean(3),
                                    reader.GetBoolean(4)
                                ));
                            }
                        }
                    }                    
                });
            }
            
            return atletas.ToList<Atleta>();
        }
        public async Task<List<Atleta>>AtletaSavePassword(Atleta atleta)
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection("" +
                    new SqliteConnectionStringBuilder
                    {
                        DataSource = db_file
                    }))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {

                            var cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;
                            cmd.CommandText = "UPDATE Atletas SET pass = $pass WHERE email = $email";
                            cmd.Parameters.AddWithValue("$email", atleta.Email);
                            cmd.Parameters.AddWithValue("$pass", atleta.Pass);
                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                        }

                        var selectCommand = connection.CreateCommand();
                        selectCommand.CommandText = $"SELECT nome,email,pass,ativo,admin FROM Atletas WHERE email='{atleta.Email}'";
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                atletas.Add(new Atleta(
                                    reader.GetString(0),reader.GetString(1),
                                    reader.GetString(2),reader.GetBoolean(3),
                                    reader.GetBoolean(4)
                                ));
                            }
                        }
                    }                    
                });
            }
            
            return atletas.ToList<Atleta>();
        }
        public async Task<List<Atleta>>AtletaDELETE(String email)
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection("" +
                    new SqliteConnectionStringBuilder
                    {
                        DataSource = db_file
                    }))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            var d1 = connection.CreateCommand();
                            d1.Transaction = transaction;
                            d1.CommandText = "DELETE FROM atletaClube WHERE email = $email";
                            d1.Parameters.AddWithValue("$email", email);
                            d1.ExecuteNonQuery();

                            var d2 = connection.CreateCommand();
                            d2.Transaction = transaction;
                            d2.CommandText = "DELETE FROM jogosAtletas WHERE clube = $nome";
                            d2.Parameters.AddWithValue("$nome", email);
                            d2.ExecuteNonQuery();

                            var deleteCommand = connection.CreateCommand();
                            deleteCommand.Transaction = transaction;
                            deleteCommand.CommandText = "DELETE FROM Atletas WHERE email = $email";
                            deleteCommand.Parameters.AddWithValue("$email", email);
                            deleteCommand.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }                    
                });
            }
            
            return atletas.ToList<Atleta>();
        }
    }
}