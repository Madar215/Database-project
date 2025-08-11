using Npgsql;
using System.Numerics;

namespace TriviaServer
{
    public class DatabaseManager
    {
        private static DatabaseManager instance = null;
        private static readonly object padlock = new object();

        private string _connectionString = "Host=aws-0-eu-north-1.pooler.supabase.com;Database=postgres;Username=postgres.iydtculxsldvaswpgeev;Password=Gmadar25!010;Port=5432;SSL Mode=Require;Trust Server Certificate=true";
        public IConfigurationRoot Configuration { get; private set; }

        public static DatabaseManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DatabaseManager();
                    }
                    return instance;
                }
            }
        }

        private DatabaseManager()
        {
            // Load config from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }


        public async Task<Question> GetQuestion(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            try
            {
                Question question = new Question();
                await connection.OpenAsync();

                // Query Players table
                string query = "SELECT * FROM \"public\".\"Questions\" WHERE id = " + id;
                using var command = new NpgsqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    question.QuestionText = reader.GetString(reader.GetOrdinal("question_text"));
                    question.Answer1Text = reader.GetString(reader.GetOrdinal("answer1_text"));
                    question.Answer2Text = reader.GetString(reader.GetOrdinal("answer2_text"));
                    question.Answer3Text = reader.GetString(reader.GetOrdinal("answer3_text"));
                    question.Answer4Text = reader.GetString(reader.GetOrdinal("answer4_text"));
                    question.CorrectAnswer = reader.GetInt16(reader.GetOrdinal("correct_answer"));
                }
                connection.Close();
                return question;
            }
            catch (Exception ex)
            {
                connection.Close();
                return null;
            }
        }

        public async Task<List<Question>> GetQuestions()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            try
            {
                List<Question> questionList = new List<Question>();
                await connection.OpenAsync();

                // Query Players table
                string query = "SELECT * FROM \"public\".\"Questions\"";
                using var command = new NpgsqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Question question = new Question();
                    question.QuestionText = reader.GetString(reader.GetOrdinal("question_text"));
                    question.Answer1Text = reader.GetString(reader.GetOrdinal("answer1_text"));
                    question.Answer2Text = reader.GetString(reader.GetOrdinal("answer2_text"));
                    question.Answer3Text = reader.GetString(reader.GetOrdinal("answer3_text"));
                    question.Answer4Text = reader.GetString(reader.GetOrdinal("answer4_text"));
                    question.CorrectAnswer = reader.GetInt16(reader.GetOrdinal("correct_answer"));
                    questionList.Add(question);
                }
                connection.Close();
                return questionList;
            }
            catch (Exception ex)
            {
                connection.Close();
                return null;
            }
        }

        public async Task<int> AddPlayer(string name, bool isActive)
        {
            // insert into players (name) values('Gal');

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "INSERT INTO players (name, isactive) VALUES (@name, @isActive) RETURNING id;";
                await using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("isactive", isActive);

                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex)
            {
                // You can log `ex` here if needed
                Console.WriteLine("DB Error: " + ex.Message);
                return -1;
            }
        }

        public async Task<int> GetActivePlayers()
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "SELECT COUNT(*) FROM players WHERE isactive = TRUE;";
                await using var cmd = new NpgsqlCommand(query, connection);

                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                // Optionally log: ex
                return 0;
            }
        }

        public async Task<bool> UpdatePlayer(int id, int score, float time)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "UPDATE players SET score = @score, timeaccumulated = @timeaccumulated WHERE id = @id;";
                await using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("score", score);
                cmd.Parameters.AddWithValue("timeaccumulated", time);

                var affectedRows = await cmd.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex.Message);
                return false;
            }
        }

        public async Task<int?> GetTopPlayerId()
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "SELECT id FROM players ORDER BY score DESC, timeaccumulated ASC LIMIT 1;";
                await using var cmd = new NpgsqlCommand(query, connection);

                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex.Message);
                return null;
            }
        }


    }
}
