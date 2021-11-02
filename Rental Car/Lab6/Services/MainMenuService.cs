using System;

namespace Lab6.Services
{
    public class MainMenuService
    {
        private readonly AgentService _agentService;
        private readonly ReservationService _reservationService;
        private readonly CustomerService _customerService;
        private readonly RentableVehicleService _rentableVehicleService;

        public MainMenuService(AgentService agentService, RentableVehicleService rentableVehicleService, CustomerService customerService, ReservationService reservationService)
        {
            _agentService = agentService;
            _rentableVehicleService = rentableVehicleService;
            _customerService = customerService;
            _reservationService = reservationService;
        }

        public void StartMenu()
        {
            DisplayMenuHeading();

            bool closeProgram;
            do
            {
                closeProgram = DisplayMenuOptions();
                Console.Clear();
            } while (!closeProgram);
        }

        private bool DisplayMenuOptions()
        {
            Console.WriteLine("Enter a menu option to continue:");
            Console.WriteLine();

            Console.WriteLine("Rental functions");
            Console.WriteLine(" RE - Reserve a vehicle");
            Console.WriteLine(" RT - Return a vehicle");
            Console.WriteLine(" AR - Display active reservations");
            Console.WriteLine(" DR - Display reservation by ID");
            Console.WriteLine();

            Console.WriteLine("People functions");
            Console.WriteLine(" MC - Manage customer records");
            if (_agentService.GetLoggedInAdminStatus())
            {
                Console.WriteLine(" MA - Manage agent records");
            }
            Console.WriteLine();

            if (_agentService.GetLoggedInAdminStatus())
            {
                Console.WriteLine("Financial functions");
                Console.WriteLine(" CF - Display current financial status");
                Console.WriteLine(" CM - Display agent commissions");
                Console.WriteLine();
            }

            Console.WriteLine("System functions");
            if (_agentService.GetLoggedInAdminStatus())
            {
                Console.WriteLine(" MV - Manage vehicles");
            }
            Console.WriteLine(" EX - Exit");
            Console.WriteLine();

            string userInput;
            bool validInput = false;
            do
            {
                Console.WriteLine("Enter the desired menu option and press <ENTER>");
                userInput = Console.ReadLine();

                switch (userInput.ToUpperInvariant())
                {
                    case "RE":
                        validInput = true;
                        _reservationService.DislayRentVehicleMenu();
                        break;
                    case "RT":
                        validInput = true;
                        _reservationService.DisplayReturnVehicleMenu();
                        break;
                    case "AR":
                        validInput = true;
                        _reservationService.DisplayActiveReservations();
                        break;
                    case "DR":
                        validInput = true;
                        Console.WriteLine("You must be 25 years or older to make a resrvation.");
                        Console.WriteLine("Enter \"Y\" to verify you are 25 years or older or enter \"N\" to return to main menu.");
                        if (Console.ReadLine().ToUpper().Equals("Y")){
                            _reservationService.DisplayReservationByID();
                        }
                        break;
                    case "MC":
                        validInput = true;
                        _customerService.DisplayManageCustomersMenu();
                        break;
                    case "MA":
                        if (_agentService.GetLoggedInAdminStatus())
                        {
                            validInput = true;
                            _agentService.DisplayManageAgentsMenu();
                        }
                        break;
                    case "MV":
                        if (_agentService.GetLoggedInAdminStatus())
                        {
                            validInput = true;
                            _rentableVehicleService.DisplayManageVehiclesMenu();
                        }
                        break;
                    case "CF":
                        if (_agentService.GetLoggedInAdminStatus())
                        {
                            validInput = true;
                            Console.WriteLine("No finanacial reports currently exist");
                            Console.WriteLine("Press <ENTER> to continue");
                            Console.ReadLine();
                        }
                        break;
                    case "CM":
                        if (_agentService.GetLoggedInAdminStatus())
                        {
                            validInput = true;
                            Console.WriteLine("Please enter start date:");
                            String startDate = Console.ReadLine();
                            Console.WriteLine("Please enter an end date:");
                            String endDate = Console.ReadLine();
                            _agentService.DisplayCommissions(startDate, endDate);

                        }
                        break;
                    case "EX":
                        return true;
                    default:
                        Console.WriteLine($"{userInput} is not a valid menu option");
                        break;
                }
            } while (!validInput);
            return false;
        }

        private void DisplayMenuHeading()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Car Rental Reservation Service");
            Console.WriteLine($"You are logged in as {_agentService.GetLoggedINDisplayName()}");
            Console.WriteLine();
        }
    }
}
