using Microsoft.Data.Sqlite;

namespace fut5.Data
{
    public class JogosDatabaseService
    {
        private readonly dbPathService _dependency;
        public JogosDatabaseService(dbPathService dbps)
        {
            _dependency = dbps;
        }

        public async Task<List<Jogo>>JogoSave(String clube, String dataJogo, String dataInscricoes)
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
            }
            else
            {

                var jogo = new Jogo(clube, dataJogo, dataInscricoes);

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
                            deleteCommand.CommandText = "DELETE FROM jogos WHERE clube = $clube AND dia = $dia";
                            deleteCommand.Parameters.AddWithValue("$clube", jogo.Clube);
                            deleteCommand.Parameters.AddWithValue("$dia", jogo.DataJogo);
                            deleteCommand.ExecuteNonQuery();

                            var deleteCommand2 = connection.CreateCommand();
                            deleteCommand2.Transaction = transaction;
                            deleteCommand2.CommandText = "DELETE FROM jogosAtletas WHERE clube = $clube AND dia = $dia";
                            deleteCommand2.Parameters.AddWithValue("$clube", jogo.Clube);
                            deleteCommand2.Parameters.AddWithValue("$dia", jogo.DataJogo);
                            deleteCommand2.ExecuteNonQuery();
                            
                            var insertCommand = connection.CreateCommand();
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = "INSERT INTO jogos (dia,clube,horaRegistoInicio,horaRegistoFim) VALUES ($dia,$clube,$horaRegistoInicio,$horaRegistoFim)";
                            insertCommand.Parameters.AddWithValue("$dia", jogo.DataJogo);
                            insertCommand.Parameters.AddWithValue("$clube", jogo.Clube);
                            insertCommand.Parameters.AddWithValue("$horaRegistoInicio", jogo.DataInscricoes);
                            insertCommand.Parameters.AddWithValue("$horaRegistoFim", jogo.DataInscricoesFim);
                            insertCommand.ExecuteNonQuery();

                            transaction.Commit();
                        }

                        var selectCommand = connection.CreateCommand();
                        selectCommand.CommandText = "SELECT clube,dia,horaRegistoInicio,horaRegistoFim FROM jogos WHERE clube = $clube AND dia = $dia";
                        selectCommand.Parameters.AddWithValue("$clube", jogo.Clube);
                        selectCommand.Parameters.AddWithValue("$dia", jogo.DataJogo);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                jogos.Add(new Jogo(
                                    reader.GetString(0),reader.GetString(1), reader.GetString(2)
                                ));
                            }
                        }
                    }                    
                });
            }
            
            return jogos.ToList<Jogo>();
        }

        public async Task<List<Jogo>>JogoCancel(String clube, String dataJogo)
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;
            
            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
            }
            else
            {

                var jogo = new Jogo(clube, dataJogo, "2021-12-12");

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
                            deleteCommand.CommandText = "DELETE FROM jogos WHERE clube = $clube AND dia = $dia";
                            deleteCommand.Parameters.AddWithValue("$clube", jogo.Clube);
                            deleteCommand.Parameters.AddWithValue("$dia", jogo.DataJogo);
                            deleteCommand.ExecuteNonQuery();

                            var deleteCommand2 = connection.CreateCommand();
                            deleteCommand2.Transaction = transaction;
                            deleteCommand2.CommandText = "DELETE FROM jogosAtletas WHERE clube = $clube AND dia = $dia";
                            deleteCommand2.Parameters.AddWithValue("$clube", jogo.Clube);
                            deleteCommand2.Parameters.AddWithValue("$dia", jogo.DataJogo);
                            deleteCommand2.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }                    
                });
            }
            return await GetJogos();
        }

        public async Task<List<Jogo>>GetJogos()
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;

            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
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
                                selectCommand.CommandText = 
                                "SELECT clube,dia,horaRegistoInicio,horaRegistoFim FROM jogos " +
                                "WHERE JULIANDAY(date('now'))-JULIANDAY(dia) <= 0 ORDER BY dia";
                                using (var reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        jogos.Add(new Jogo(
                                            reader.GetString(0),reader.GetString(1), reader.GetString(2)
                                        ));
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                });
            }

            // return Task.FromResult(atletas.ToArray<Atleta>());
            return jogos.ToList<Jogo>();
        }
        
        public async Task<List<Jogo>>GetJogo(string clube, string dia)
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;

            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
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
                            var jogo = new Jogo(clube, dia, dia);
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {
                                var selectCommand = connection.CreateCommand();
                                selectCommand.CommandText = "SELECT clube,dia,horaRegistoInicio,horaRegistoFim FROM jogos WHERE clube = $clube AND dia = $dia";
                                selectCommand.Parameters.AddWithValue("$clube", jogo.Clube);
                                selectCommand.Parameters.AddWithValue("$dia", jogo.DataJogo);
                                using (var reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        jogos.Add(new Jogo(
                                            reader.GetString(0),reader.GetString(1), reader.GetString(2)
                                        ));
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                });
            }

            // return Task.FromResult(atletas.ToArray<Atleta>());
            return jogos.ToList<Jogo>();
        }

        public async Task<List<Jogo>>GetMyGames(string email)
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;

            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
            }
            else
            {
                await Task.Run(() =>
                {

                    var clubeService = new ClubesDatabaseService(_dependency);
                    var myClubes = clubeService.GetClubesFor(email).Result;
                    foreach (var clube in myClubes)
                    {
                        using (var connection = new SqliteConnection("" +
                        new SqliteConnectionStringBuilder
                        {
                            DataSource = db_file
                        }))
                        {
                            // var jogo = new Jogo(clube.Nome, dia, dia);
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {
                                var selectCommand = connection.CreateCommand();
                                selectCommand.CommandText = 
                                "SELECT clube,dia,horaRegistoInicio,horaRegistoFim FROM jogos WHERE " +
                                "clube = $clube AND (JULIANDAY(date('now'))-JULIANDAY(dia) <= 0) ORDER BY dia";
                                selectCommand.Parameters.AddWithValue("$clube", clube.Nome);
                                using (var reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        jogos.Add(new Jogo(
                                            reader.GetString(0),reader.GetString(1), reader.GetString(2)
                                        ));
                                    }
                                }
                                transaction.Commit();
                            }
                        }

                    }

                });
            }

            // return Task.FromResult(atletas.ToArray<Atleta>());
            return jogos.ToList<Jogo>();
        }
        
        public async Task<List<JogoPresenca>>GetPresencas(string clube, string dataJogo)
        {
            var presencas = new List<JogoPresenca>();
            var db_file = _dependency.databasePath;
            string[] data1;
            if (dataJogo.Contains("T"))
            {
                data1 = dataJogo.Split("T")[0].Split("-");     // "2021-12-20T00:00:00+00:00"
            }
            else
            {
                data1 = dataJogo.Split(" ")[0].Split("-");     // "2021-12-09 00:00:00+00:00"
            }
            var DataJogo = new DateTimeOffset(
                year : int.Parse(data1[0]),
                month : int.Parse(data1[1]),
                day : int.Parse(data1[2]),
                hour:0,minute:0,second:0, new TimeSpan(0)
                );

            if(db_file == null)
            {
                presencas.Add(new JogoPresenca("Database not found!","","",0,DateTime.Today));
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
                                selectCommand.CommandText =
                                "SELECT nome,table1.email,coalesce(resposta,0) AS resposta, coalesce(dia,$dia) " +
                                "FROM (SELECT nome,email FROM atletas WHERE ativo=1 AND email in " +
                                "(SELECT email FROM atletaClube nome WHERE clube=$clube)) table1 " +
                                "LEFT JOIN jogosAtletas " +
                                "ON table1.email = jogosAtletas.email " + 
                                "AND (JULIANDAY(jogosAtletas.dia) - JULIANDAY($dia)) = 0 " +
                                "ORDER BY nome;";
                                selectCommand.Parameters.AddWithValue("$dia", DataJogo);
                                selectCommand.Parameters.AddWithValue("$clube", clube);
                                using (var reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                    presencas.Add(new JogoPresenca(
                                        reader.GetString(0),
                                        reader.GetString(1),
                                        clube,
                                        reader.GetInt32(2),
                                        reader.GetDateTimeOffset(3)));

                                    // // filtrar pela data correta
                                    // var timespan = reader.GetDateTimeOffset(3) - DataJogo;
                                    // if (timespan.Days == 0)
                                    // {
                                    //     presencas.Add(new JogoPresenca(
                                    //     reader.GetString(0),
                                    //     reader.GetString(1),
                                    //     clube, reader.GetInt32(2),
                                    //     reader.GetDateTimeOffset(3)
                                    // ));
                                    // }
                                }
                                }
                                transaction.Commit();
                            }
                        }
                });
            }

            return presencas.ToList<JogoPresenca>();
        }
        
        public async Task<List<JogoPresenca>>SavePresenca(string clube, string dataJogo, string email, int resposta)
        {
            var presencas = new List<JogoPresenca>();
            var db_file = _dependency.databasePath;
            string[] data1;
            if (dataJogo.Contains("T"))
            {
                data1 = dataJogo.Split("T")[0].Split("-");     // "2021-12-20T00:00:00+00:00"
            }
            else
            {
                data1 = dataJogo.Split(" ")[0].Split("-");     // "2021-12-09 00:00:00+00:00"
            }
            var DataJogo = new DateTimeOffset(
                year : int.Parse(data1[0]),
                month : int.Parse(data1[1]),
                day : int.Parse(data1[2]),
                hour:0,minute:0,second:0, new TimeSpan(0)
                );

            if(db_file == null)
            {
                presencas.Add(new JogoPresenca("Database not found!","","",0,DateTime.Today));
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
                                deleteCommand.CommandText = "DELETE FROM jogosAtletas WHERE clube=$clube AND dia=$dia AND email=$email";
                                deleteCommand.Parameters.AddWithValue("$clube", clube);
                                deleteCommand.Parameters.AddWithValue("$dia", DataJogo);
                                deleteCommand.Parameters.AddWithValue("$email", email);
                                deleteCommand.ExecuteNonQuery();

                                if (resposta != 0)
                                {
                                    var insertCommand = connection.CreateCommand();
                                    insertCommand.Transaction = transaction;
                                    insertCommand.CommandText = 
                                    "INSERT INTO jogosAtletas (dia,clube,email,resposta) VALUES ($dia,$clube,$email,$resposta)";
                                    insertCommand.Parameters.AddWithValue("$dia", DataJogo);
                                    insertCommand.Parameters.AddWithValue("$clube", clube);
                                    insertCommand.Parameters.AddWithValue("$email", email);
                                    insertCommand.Parameters.AddWithValue("$resposta", resposta);
                                    insertCommand.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                    });
                }
                return await GetPresencas(clube, dataJogo);
            }
        
        public async Task<List<Jogo>>Purge()
        {
            var jogos = new List<Jogo>();
            var db_file = _dependency.databasePath;

            if(db_file == null)
            {
                jogos.Add(new Jogo("Database not found!","2021-12-12","2021-12-12"));
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
                                deleteCommand.CommandText = "DELETE FROM jogos WHERE JULIANDAY(date('now'))-JULIANDAY(dia) > 0";
                                deleteCommand.ExecuteNonQuery();

                                var deleteCommand2 = connection.CreateCommand();
                                deleteCommand2.Transaction = transaction;
                                deleteCommand2.CommandText = "DELETE FROM jogosAtletas WHERE JULIANDAY(date('now'))-JULIANDAY(dia) > 0";
                                deleteCommand2.ExecuteNonQuery();

                                transaction.Commit();
                            }
                        }
                });
            }

            // return Task.FromResult(atletas.ToArray<Atleta>());
            return jogos.ToList<Jogo>();
        }
    }
}