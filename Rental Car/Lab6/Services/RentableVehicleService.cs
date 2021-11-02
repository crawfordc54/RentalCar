using Lab6.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6.Services
{
    public class RentableVehicleService
    {
        private readonly ReservationSystemContext _dbContext;

        public RentableVehicleService(ReservationSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DisplayManageVehiclesMenu()
        {
            DisplaySubmenuHeading();

            bool returnToMenu;
            do
            {
                returnToMenu = DisplaySubmenuOptions();
                Console.Clear();
            } while (!returnToMenu);
        }

        private bool DisplaySubmenuOptions()
        {
            Console.WriteLine(" AV - Add a rentable vehicle");
            Console.WriteLine(" UV - Update a rentable vehicle");
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
                    case "AV":
                        validInput = true;
                        AddVehicle();
                        break;
                    case "UV":
                        validInput = true;
                        DisplayRentableVehicles();
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

        private void DisplaySubmenuHeading()
        {
            Console.Clear();
            Console.WriteLine("Enter a menu option to continue:");
            Console.WriteLine();
        }

        private void AddVehicle()
        {
            Console.Clear();
            Console.WriteLine("Enter new vehicle information:");
            Console.WriteLine();

            Console.Write("Enter vehicle VIN: ");
            string vin = Console.ReadLine();

            Console.Write("Enter vehicle year: ");
            string year = Console.ReadLine();

            Console.Write("Enter vehicle make: ");
            string make = Console.ReadLine();

            Console.Write("Enter vehicle model: ");
            string model = Console.ReadLine();

            Console.Write("Enter vehicle submodel: ");
            string submodel = Console.ReadLine();

            Console.Write("Enter vehicle colour: ");
            string colour = Console.ReadLine();

            Console.Write("Enter vehicle acquisition date in YYYY-MM-DD format: ");
            string dateAcquired = Console.ReadLine();

            Console.Write("Enter vehicle current odometer rating: ");
            string odometer = Console.ReadLine();

            Console.Write("Enter vehicle daily rate in xx.yy format (no $): ");
            string dailyRate = Console.ReadLine();

            Console.Write("Enter vehicle fuel capacity in xx.y format (no UOM): ");
            string fuelCapacity = Console.ReadLine();

            var newVehicle = _dbContext.RentableVehicles.Add(new RentableVehicle
                {
                    VehicleIdentificationNumber = vin,
                    Year = year,
                    Make = make,
                    Model = model,
                    Submodel = submodel,
                    ExteriorColor = colour,
                    DateAcquired = DateTime.Parse(dateAcquired),
                    InitialOdometerReading = Decimal.Parse(odometer),
                    CurrentOdometerReading = Decimal.Parse(odometer),
                    StandardDailyRate = Decimal.Parse(dailyRate),
                    FuelCapacity = Decimal.Parse(fuelCapacity)
                }
            );
            _dbContext.SaveChanges();

            Console.WriteLine($"Vehicle {newVehicle.Entity.RentableVehicleID} added");
        }

        private void DisplayRentableVehicles()
        {
            Console.Clear();
            Console.WriteLine("The following vehicles are in the system:");
            var availableVehicles = _dbContext.RentableVehicles.ToList();
            if (availableVehicles.Any())
            {
                DisplayRentableVehicleList(availableVehicles);

                bool validSubInput = false;
                string vehicleInput = null;
                do
                {
                    Console.WriteLine("Enter the vehicle ID number to edit, or enter RT to return");
                    vehicleInput = Console.ReadLine();

                    if (vehicleInput.ToUpperInvariant() == "RT")
                    {
                        validSubInput = true;
                        return;
                    }

                    if (int.TryParse(vehicleInput, out int parsedVehicleInput))
                    {
                        if (availableVehicles.Any(a => a.RentableVehicleID == parsedVehicleInput))
                        {
                            validSubInput = true;
                            UpdateVehicle(parsedVehicleInput);
                        }
                    }
                } while (!validSubInput);
            }
            else
            {
                Console.WriteLine("No vehicles are currently available to rent.");
                Console.WriteLine("Press <ENTER> to return to the main menu");
                Console.ReadLine();
            }
        }

        public void DisplayRentableVehicleList(IEnumerable<RentableVehicle> availableVehicles)
        {
            Console.WriteLine("ID    VIN               Year Make       Model      Colour      Date Disposed");
            Console.WriteLine("------------------------------------------------------------------------------------");
            foreach (var vehicle in availableVehicles)
            {
                Console.Write(vehicle.RentableVehicleID.ToString().PadLeft(5));
                Console.Write(' ');
                Console.Write(vehicle.VehicleIdentificationNumber.PadRight(17));
                Console.Write(' ');
                Console.Write(vehicle.Year.PadRight(4));
                Console.Write(' ');
                Console.Write(vehicle.Make.PadRight(10));
                Console.Write(' ');
                Console.Write(vehicle.Model.PadRight(10));
                Console.Write(' ');
                Console.Write(vehicle.ExteriorColor.PadRight(10));
                Console.Write(' ');
                Console.Write(vehicle.DateDisposed.ToString().PadRight(10));
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void UpdateVehicle(int vehicleID)
        {
            Console.Clear();
            Console.WriteLine("Enter updated vehicle information:");
            Console.WriteLine();

            var vehicleToUpdate = _dbContext.RentableVehicles.First(v => v.RentableVehicleID == vehicleID);

            string input = null;

            Console.Write($"Enter vehicle VIN, or press <ENTER> to leave {vehicleToUpdate.VehicleIdentificationNumber}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.VehicleIdentificationNumber = input;
            }

            Console.Write($"Enter vehicle year, or press <ENTER> to leave {vehicleToUpdate.Year}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.Year = input;
            }

            Console.Write($"Enter vehicle make, or press <ENTER> to leave {vehicleToUpdate.Make}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.Make = input;
            }

            Console.Write($"Enter vehicle model, or press <ENTER> to leave {vehicleToUpdate.Model}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.Model = input;
            }

            Console.Write($"Enter vehicle submodel, or press <ENTER> to leave {vehicleToUpdate.Submodel}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.Submodel = input;
            }

            Console.Write($"Enter vehicle colour, or press <ENTER> to leave {vehicleToUpdate.ExteriorColor}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.ExteriorColor = input;
            }

            Console.Write($"Enter vehicle acquisition date in YYYY-MM-DD format, or press <ENTER> to leave {vehicleToUpdate.DateAcquired:yyyy-MM-dd}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.DateAcquired = DateTime.Parse(input);
            }

            Console.Write($"Enter vehicle current odometer, or press <ENTER> to leave {vehicleToUpdate.CurrentOdometerReading}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.CurrentOdometerReading = decimal.Parse(input);
            }

            Console.Write($"Enter vehicle current daily rate, or press <ENTER> to leave {vehicleToUpdate.StandardDailyRate}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.StandardDailyRate = decimal.Parse(input);
            }

            Console.Write($"Enter vehicle current fuel capacity, or press <ENTER> to leave {vehicleToUpdate.FuelCapacity}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.FuelCapacity = decimal.Parse(input);
            }

            Console.Write($"Enter vehicle disposition date, or press <ENTER> to leave {vehicleToUpdate.DateDisposed}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.DateDisposed = DateTime.Parse(input);
            }

            Console.Write($"Enter vehicle disposition price, or press <ENTER> to leave {vehicleToUpdate.DisposalPrice:C}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.DisposalPrice = decimal.Parse(input);
            }

            Console.Write($"Enter vehicle disposition reason, or press <ENTER> to leave {vehicleToUpdate.DisposalReason}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                vehicleToUpdate.DisposalReason = input;
            }

            _dbContext.SaveChanges();

            Console.WriteLine("Vehicle has been updated");
        }
    }
}
