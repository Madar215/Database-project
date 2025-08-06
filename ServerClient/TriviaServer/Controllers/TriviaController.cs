using Microsoft.AspNetCore.Mvc;
using System.Collections;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TriviaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TriviaController : ControllerBase
    {
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
        public async Task<int> Post([FromBody] string name, bool isActive)
        {
            int result =  await DatabaseManager.Instance.AddPlayer(name, isActive);
            return result;
        }

        [HttpPost("update-player")]
        public async Task<bool> Post(int id, int score, float time)
        {
            bool result = await DatabaseManager.Instance.UpdatePlayer(id, score, time);
            return result;
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
