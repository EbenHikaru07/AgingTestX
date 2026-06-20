using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AgingTest_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgingParamController : ControllerBase
    {
        [HttpPost("AgingParam")]
        public IActionResult AgingParam([FromBody] string value)
        {

            return Ok(new { message = "Test" });
        }

        // GET: api/<AgingParamController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AgingParamController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgingParamController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgingParamController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AgingParamController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
