using Lab6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab6.Services
{
    public class AgentService
    {
        private readonly ReservationSystemContext _dbContext;

        private static readonly int MAX_LOGIN_ATTEMPTS = 3;

        private Agent _authenticatedAgent;

        public AgentService(ReservationSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Authenticate()
        {
            string username;
            string password;

            int loginAttempts = 0;
            do
            {
                loginAttempts++;

                Console.Write("Username: ");
                username = Console.ReadLine();

                Console.Write("Password: or enter x to change password.");
                password = GetPassword();
                if (password.Equals("x"))
                {
                    Console.WriteLine("Please enter new password:");
                    password = GetPassword();
                    updatePassword(username, password);
                }
                else
                {
                    _authenticatedAgent = RetrieveAgentByCredentials(username, password);
                    if (_authenticatedAgent == null)
                    {
                        Console.WriteLine("Invalid username or password. Please try again.");
                        Console.WriteLine();
                    }
                }
            } while (_authenticatedAgent == null && loginAttempts < MAX_LOGIN_ATTEMPTS);

            return _authenticatedAgent != null;
        }

        public string GetLoggedINDisplayName()
        {
            return $"{_authenticatedAgent.FirstName} {_authenticatedAgent.LastName}";
        }

        public int GetLoggedInAgentID()
        {
            return _authenticatedAgent.AgentID;
        }

        public bool GetLoggedInAdminStatus()
        {
            return _authenticatedAgent.IsAdmin;
        }

        public void DisplayManageAgentsMenu()
        {
            DisplaySubmenuHeading();

            bool returnToMenu;
            do
            {
                returnToMenu = DisplaySubmenuOptions();
                Console.Clear();
            } while (!returnToMenu);
        }

        public void DisplayCommissions(string startingDate, string endingDate)
        {

            DateTime startDate = Convert.ToDateTime(startingDate);
            DateTime endDate = Convert.ToDateTime(endingDate);

            Console.Clear();
            Console.WriteLine("Please wait...");

            var reservationsForDateRange = _dbContext.Reservations
                .Where(res => res.ActualReturnDate.HasValue);

            Dictionary<int, decimal> commissions = new Dictionary<int, decimal>();
            foreach (var agent in reservationsForDateRange.AsEnumerable().Where(w => w.ActualReturnDate >= startDate && w.ActualReturnDate <= endDate).GroupBy(g => g.AgentID))
            {
                decimal commission = 0M;
                foreach(var agentReservations in reservationsForDateRange.Where(res => res.AgentID == agent.Key))
                {
                    if (agentReservations.ActualReturnDate >= startDate && agentReservations.ActualReturnDate <= endDate)
                    {
                        commission += (decimal)agentReservations.BilledSubtotal * agentReservations.Agent.Commission;
                    }
                }

                commissions.Add(agent.Key, commission);
            }

            Console.WriteLine("{0,-15} {1,15}","Agent","Commission");
            Console.WriteLine("------------------------------");
            foreach(var commission in commissions)
            {
                var agent = _dbContext.Agents.First(a => a.AgentID == commission.Key);

                Console.WriteLine("{0,-15} {1,15}",$"{agent.FirstName} {agent.LastName}",$"{commission.Value:C}");
            }

            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        private static string GetPassword()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        private bool DisplaySubmenuOptions()
        {
            Console.WriteLine(" AA - Add an agent");
            Console.WriteLine(" LA - List agents");
            Console.WriteLine(" UA - Update an agent");
            Console.WriteLine();

            Console.WriteLine(" RT - Return to the previous menu");
            Console.WriteLine();

            string userInput;
            bool validInput = false;
            do
            {
                Console.WriteLine("Enter the desired menu option and press <ENTER>");
                userInput = Console.ReadLine();

                switch (userInput.ToUpperInvariant())
                {
                    case "AA":
                        validInput = true;
                        AddAgent();
                        break;
                    case "LA":
                        validInput = true;
                        ListAgents();
                        break;
                    case "UA":
                        validInput = true;
                        UpdateAgent();
                        break;
                    case "RT":
                        return true;
                    default:
                        Console.WriteLine($"{userInput} is not a valid menu option");
                        break;
                }
            } while (!validInput);

            return false;
        }

        private void ListAgents()
        {
            var foundAgents = _dbContext.Agents.AsEnumerable();

            Console.WriteLine(" ID First Name      Last Name       Hire Dt         Term Dt         Comm  Username");
            Console.WriteLine("----------------------------------------------------------------------------------");
            if (foundAgents != null && foundAgents.Any())
            {
                foreach (var agent in foundAgents)
                {
                    Console.Write(agent.AgentID.ToString().PadLeft(3));
                    Console.Write(' ');
                    Console.Write(agent.FirstName.PadRight(15));
                    Console.Write(' ');
                    Console.Write(agent.LastName.PadRight(15));
                    Console.Write(' ');
                    Console.Write(agent.DateOfHire.ToString("d").PadRight(15));
                    Console.Write(' ');
                    Console.Write((agent.DateOfTermination != null ? agent.DateOfTermination.Value.ToString("d") : "Still active").PadRight(15));
                    Console.Write(' ');
                    Console.Write(agent.Commission.ToString().PadRight(5));
                    Console.Write(' ');
                    Console.Write(agent.Username.PadRight(12));
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No agents were found with the specified search criteria");
            }

            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        public void UpdateAgent()
        {
            bool validSubInput = false;
            string agentInput = null;
            do
            {
                Console.WriteLine("Specify the agent ID number to edit, or enter RT to return");
                agentInput = Console.ReadLine();

                if (agentInput.ToUpperInvariant() == "RT")
                {
                    validSubInput = true;
                    return;
                }

                if (int.TryParse(agentInput, out int parsedAgentID))
                {
                    if (_dbContext.Customers.Any(a => a.CustomerID == parsedAgentID))
                    {
                        validSubInput = true;
                        UpdateAgent(parsedAgentID);
                    }
                }
            } while (!validSubInput);
        }

        private void updatePassword(string username, string password)
        {
            string input = null;

            var agentToUpdate = _dbContext.Agents.FirstOrDefault(v => v.Username == username);

            if (agentToUpdate != null)
            {
                agentToUpdate.Password = password;
                _dbContext.SaveChanges();

                Console.WriteLine("Agent has been updated");
            }
            else
            {
                Console.WriteLine("Invalid user");
            }
        }

        private void UpdateAgent(int agentID)
        {
            Console.Clear();
            Console.WriteLine("Enter updated agent information:");
            Console.WriteLine();

            string input = null;

            var agentToUpdate = _dbContext.Agents.FirstOrDefault(v => v.AgentID == agentID);

            if (agentToUpdate != null)
            {

                Console.Write($"Enter agent first name, or press <ENTER> to leave {agentToUpdate.FirstName}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.FirstName = input;
                }

                Console.Write($"Enter agent last name, or press <ENTER> to leave {agentToUpdate.LastName}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.LastName = input;
                }

                Console.Write($"Enter agent admin status as Y or N, or press <ENTER> to leave {agentToUpdate.IsAdmin}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.IsAdmin = input.ToLower() == "y";
                }

                Console.Write($"Enter agent date of hire in YYYY-MM-DD format, or press <ENTER> to leave {agentToUpdate.DateOfHire:d}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.DateOfHire = DateTime.Parse(input);
                }

                Console.Write($"Enter agent date of termination in YYYY-MM-DD format, or press <ENTER> to leave {agentToUpdate.DateOfTermination:d}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.DateOfTermination = DateTime.Parse(input);
                }


                Console.Write($"Enter agent commission in 0.xx format, or press <ENTER> to leave {agentToUpdate.Commission}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.Commission = decimal.Parse(input);
                }

                Console.Write($"Enter agent username, or press <ENTER> to leave {agentToUpdate.Username}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.Username = input;
                }

                Console.Write($"Enter agent password, or press <ENTER> to leave {agentToUpdate.Password}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    agentToUpdate.Password = input;
                }

                _dbContext.SaveChanges();

                Console.WriteLine("Agent has been updated");
            }
            else
            {
                Console.WriteLine($"ID number {agentID} is not a valid customer");
            }

            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        private void AddAgent()
        {
            Console.Clear();
            Console.WriteLine("Enter new agent information:");
            Console.WriteLine();

            Console.Write("Enter customer first name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter customer last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Set if the agent is a system admin (enter Y or N): ");
            string isAdmin = Console.ReadLine();

            Console.Write("Enter date of hire in YYYY-MM-DD format: ");
            string dateOfHire = Console.ReadLine();

            Console.Write("Enter commissions in 0.xx format (percentage): ");
            string commission = Console.ReadLine();

            Console.Write("Enter login username: ");
            string username = Console.ReadLine();

            Console.Write("Enter login password: ");
            string password = Console.ReadLine();

            var newVehicle = _dbContext.Agents.Add(new Agent
            {
                FirstName = firstName,
                LastName = lastName,
                IsAdmin = (isAdmin.ToLower() == "y"),
                DateOfHire = DateTime.Parse(dateOfHire),
                Commission = decimal.Parse(commission),
                Username = username,
                Password = password
            }
            );
            _dbContext.SaveChanges();

            Console.WriteLine($"Agent {newVehicle.Entity.AgentID} added");
        }

        private Agent RetrieveAgentByCredentials(string username, string password)
        {
            // NOTE TO STUDENTS (breaking the fourth wall): If this were a real system, YOU WOULD NOT STORE PASSWORDS IN PLAIN TEXT
            // More info at: https://medium.com/dealeron-dev/storing-passwords-in-net-core-3de29a3da4d2 
            return _dbContext.Agents.Where(a => a.Username == username && a.Password == password && a.DateOfTermination == null).FirstOrDefault();
        }

        private void DisplaySubmenuHeading()
        {
            Console.Clear();
            Console.WriteLine("Enter a menu option to continue:");
            Console.WriteLine();
        }
    }
}
