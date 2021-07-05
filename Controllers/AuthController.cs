using System;
using agent_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace agent_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private InMemoryDatabase database;

        public AuthController(InMemoryDatabase database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult ProductList()
        {
            return Ok(database.Users);
        }

        [HttpPost]
        [Route("Signin")]
        public IActionResult Signin(string username, string password)
        {
            var generatedId = Guid.NewGuid().ToString();
            database.Users.Add(new User { Id = generatedId, Username = username, Token = $"{generatedId}**{username}" });
            return Ok(new { Ok = true });
        }
    }
}