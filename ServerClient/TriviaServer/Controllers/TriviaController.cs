using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Numerics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TriviaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TriviaController : ControllerBase
    {
        private readonly MongoScoreboard _mongo;

        public TriviaController(MongoScoreboard mongo)
        {
            _mongo = mongo;
        }

        // GET: api/<TriviaController>
        [HttpGet]
        public async Task<IEnumerable<Question>> Get()
        {
            List<Question> QuestionList = await DatabaseManager.Instance.GetQuestions();
            return QuestionList;
        }

        // GET api/<TriviaController>/5
        [HttpGet("{id}")]
        public async Task<Question> Get(int id)
        {
            Question question = await DatabaseManager.Instance.GetQuestion(id);
            return question;
        }

        [HttpGet("active-count")]
        public async Task<int> GetActivePlayers()
        {
            int activePlayers = await DatabaseManager.Instance.GetActivePlayers();
            return activePlayers;
        }

        // POST api/<TriviaController>
        [HttpPost]
        public async Task<int> Post([FromBody] PlayerDto player)
        {
            int result = await DatabaseManager.Instance.AddPlayer(player.name, player.isActive);
            return result;
        }

        [HttpPost("update-player")]
        public async Task<IActionResult> UpdatePlayer(
        [FromForm] int id,
        [FromForm] int score,
        [FromForm] float time,
        [FromServices] MongoScoreboard mongo,
        ILogger<TriviaController> logger)
        {
            try
            {
                var ok = await DatabaseManager.Instance.UpdatePlayer(id, score, time);
                if (!ok) return BadRequest(false);

                try
                {
                    mongo.InsertScore(id, score, time); // mirror to NoSQL
                }
                catch (Exception mex)
                {
                    logger.LogError(mex, "Mongo insert failed for id {Id}", id);
                    // choose: either still Ok, or fail hard:
                    // return StatusCode(500, "Mongo insert failed");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdatePlayer failed for id {Id}", id);
                return StatusCode(500, "SQL update failed");
            }
        }

        [HttpGet("top-player")]
        public async Task<int?> GetTopPlayer()
        {
            return await DatabaseManager.Instance.GetTopPlayerId();
        }


        // PUT api/<TriviaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TriviaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
