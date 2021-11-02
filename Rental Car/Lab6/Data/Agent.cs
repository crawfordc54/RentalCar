using System;

namespace Lab6.Data
{
    public class Agent
    {
        public int AgentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime DateOfHire { get; set; }
        public DateTime? DateOfTermination { get; set; }
        public decimal Commission { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
