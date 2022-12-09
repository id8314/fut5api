using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace fut5.Data
{
    public class ClubesDatabaseService
    {
        private readonly dbPathService _dependency;
        public ClubesDatabaseService(dbPathService dbps)
        {
            _dependency = dbps;
        }
        public Task<Clube[]> GetClubes()
        {
            var clubes = new List<Clube>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                clubes.Add(new Clube("Database not found!"));
            }
            else
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
                        selectCommand.CommandText = "SELECT nome FROM Clubes ORDER BY nome";
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clubes.Add(new Clube(reader.GetString(0)));
                            }
                        }
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(clubes.ToArray<Clube>());
        }
        public Task<Clube[]> AddClube(Clube clube)
        {
            var clubes = new List<Clube>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                clubes.Add(new Clube("Database not found!"));
            }
            else
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
                        deleteCommand.CommandText = "DELETE FROM Clubes WHERE nome = $nome";
                        deleteCommand.Parameters.AddWithValue("$nome", clube.Nome);
                        deleteCommand.ExecuteNonQuery();

                        var insertCommand = connection.CreateCommand();
                        insertCommand.Transaction = transaction;
                        insertCommand.CommandText = "INSERT INTO Clubes (nome) VALUES ($nome)";
                        insertCommand.Parameters.AddWithValue("$nome", clube.Nome);
                        insertCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = $"SELECT nome FROM Clubes WHERE nome='{clube.Nome}'";
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clubes.Add(new Clube(reader.GetString(0)));
                        }
                    }
                }
            }

            return Task.FromResult(clubes.ToArray<Clube>());

        }
        public Task<Clube[]> GetClubesFor(string email)
        {
            var clubes = new List<Clube>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                clubes.Add(new Clube("Database not found!"));
            }
            else
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
                        selectCommand.CommandText = $"SELECT clube FROM atletaClube WHERE email='{email}' ORDER BY clube";
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clubes.Add(new Clube(
                                    reader.GetString(0)
                                ));
                            }
                        }
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(clubes.ToArray<Clube>());
        }
        public Task<Clube[]> SetClubesFor(string email, ClubeMember[] membership)
        {
            var clubes = new List<Clube>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                clubes.Add(new Clube("Database not found!"));
            }
            else
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
                        deleteCommand.CommandText = "DELETE FROM atletaClube WHERE email = $email";
                        deleteCommand.Parameters.AddWithValue("$email", email);
                        deleteCommand.ExecuteNonQuery();

                        foreach (var clube in membership)
                        {
                            if (clube.isMember)
                            {
                                var insertCommand = connection.CreateCommand();
                                insertCommand.Transaction = transaction;
                                insertCommand.CommandText = "INSERT INTO atletaClube (clube,email) VALUES ($clube,$email)";
                                insertCommand.Parameters.AddWithValue("$clube", clube.clube);
                                insertCommand.Parameters.AddWithValue("$email", email);
                                insertCommand.ExecuteNonQuery();
                            }
                    }

                        transaction.Commit();
                    }
                }
            }
            return Task.FromResult(clubes.ToArray<Clube>());
        }
        public Task<Atleta[]> GetMembersFor(string clubename)
        {
            var atletas = new List<Atleta>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                atletas.Add(new Atleta("Database not found!","","",false,false));
            }
            else
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
                        selectCommand.CommandText = 
                            "SELECT atletas.nome, atletas.email, atletas.ativo, atletaClube.clube FROM atletaClube " +
                            "INNER JOIN atletas ON atletas.email=atletaClube.email " +
                            $"WHERE atletaClube.clube = '{clubename}' ";
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                atletas.Add(new Atleta(
                                   reader.GetString(0),reader.GetString(1),
                                   "",reader.GetBoolean(2),false));
                            }
                        }
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(atletas.ToArray<Atleta>());
        }

        public Task<Clube[]> DELETEClube(String clube)
        {
            var clubes = new List<Clube>();
            var db_file = _dependency.databasePath;

            if (db_file == null)
            {
                clubes.Add(new Clube("Database not found!"));
            }
            else
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
                        d1.CommandText = "DELETE FROM atletaClube WHERE clube = $nome";
                        d1.Parameters.AddWithValue("$nome", clube);
                        d1.ExecuteNonQuery();

                        var d2 = connection.CreateCommand();
                        d2.Transaction = transaction;
                        d2.CommandText = "DELETE FROM jogosAtletas WHERE clube = $nome";
                        d2.Parameters.AddWithValue("$nome", clube);
                        d2.ExecuteNonQuery();

                        var d3 = connection.CreateCommand();
                        d3.Transaction = transaction;
                        d3.CommandText = "DELETE FROM jogos WHERE clube = $nome";
                        d3.Parameters.AddWithValue("$nome", clube);
                        d3.ExecuteNonQuery();

                        var d4 = connection.CreateCommand();
                        d4.Transaction = transaction;
                        d4.CommandText = "DELETE FROM clubes WHERE nome = $nome";
                        d4.Parameters.AddWithValue("$nome", clube);
                        d4.ExecuteNonQuery();

                        transaction.Commit();
                    }

                }
            }
            return GetClubes();
        }

    }
}