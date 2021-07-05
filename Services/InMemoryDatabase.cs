using System;
using System.Collections.Generic;

namespace agent_api.Services
{
    public class InMemoryDatabase
    {
        public IList<User> Users = new List<User>()
        {
            new User {Id = "1", Username = "Sirwan", Token = "1**Sirwan"}
        };
    }

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}