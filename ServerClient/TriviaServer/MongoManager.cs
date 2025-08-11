using MongoDB.Driver;
using System;

public class MongoScoreboard
{
    private readonly IMongoCollection<ScoreRecord> _col;

    public MongoScoreboard(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var db = client.GetDatabase("trivia");
        _col = db.GetCollection<ScoreRecord>("scoreboard");
    }

    public void InsertScore(int playerId, int score, float time, string name = null)
    {
        var doc = new ScoreRecord
        {
            PlayerId = playerId,
            Name = name,
            Score = score,
            TimeAccumulated = time,
            UpdatedAt = DateTime.UtcNow
        };
        _col.InsertOne(doc);
    }
}

public class ScoreRecord
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public float TimeAccumulated { get; set; }
    public DateTime UpdatedAt { get; set; }
}
