using Lab6.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6.Services
{
    public class ReservationService
    {        
        private readonly ReservationSystemContext _dbContext;
        private readonly AgentService _agentService;
        private readonly RentableVehicleService _rentableVehicleService;

        private static readonly decimal FUEL_SURCHARGE_DOLLARS_PER_UOM = 5.00M;
        private static readonly decimal LATE_CHARGE_PERCENTAGE = 0.10M;
        private static readonly decimal SALES_TAX_PERCENTAGE = 0.085M;

        public ReservationService(ReservationSystemContext dbContext, AgentService agentService, RentableVehicleService rentableVehicleService)
        {
            _dbContext = dbContext;
            _agentService = agentService;
            _rentableVehicleService = rentableVehicleService;
        }

        public void DislayRentVehicleMenu()
        {
            Console.Clear();

            string input = null;

            DateTime rentalDate = DateTime.Now;
            Console.Write($"What is the first date of the rental? (Press <ENTER> to use today's date of {rentalDate:d}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                rentalDate = DateTime.Parse(input);
            }

            Console.WriteLine("The following vehicles are available to rent:");

            var activeReservations = _dbContext.Reservations.Where(res => res.ActualReturnDate == null || res.ExpectedReturnDate.Date > rentalDate.Date);
            
            var availableVehicles = 
                _dbContext.RentableVehicles
                    .Where(rent => rent.DateDisposed == null && !activeReservations.Any(res => rent.RentableVehicleID == res.RentableVehicleID));

            if (availableVehicles.Any())
            {
                _rentableVehicleService.DisplayRentableVehicleList(availableVehicles);

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
                            CreateReservation(parsedVehicleInput, rentalDate);
                        }
                    }
                } while (!validSubInput);
            }
            else
            {
                Console.WriteLine("No vehicles are currently available to rent. Press <ENTER> to return to the main menu");
                Console.ReadLine();
            }
        }

        public void DisplayReservationByID()
        {
            Console.Clear();
            bool validInput = false;
            string reservationInput;
            do
            {
                Console.WriteLine("Specify the reservation ID number to view, or enter RT to return");
                reservationInput = Console.ReadLine();

                if (reservationInput.ToUpperInvariant() == "RT")
                {
                    return;
                }
                if (int.TryParse(reservationInput, out int reservationID))
                {
                    validInput = true;
                    DisplayReservationByID(reservationID);
                    Console.WriteLine("Press <ENTER> to continue");
                    Console.ReadLine();
                }
            } while (!validInput);
        }

        public void DisplayReservationByID(int reservationID)
        {
            var reservation = _dbContext.Reservations.FirstOrDefault(res => res.ReservationID == reservationID);
            if (reservation != null)
            {
                Console.Clear();
                Console.WriteLine($"Billing statement");
                Console.WriteLine($"------------------------------------------------------------------------------");
                Console.WriteLine("{0,-25} {1,20}", "Reservation:", $"{reservation.ReservationID}");
                Console.WriteLine();
                Console.WriteLine("{0,-25} {1,20}", "Customer ID:", $"{reservation.CustomerID}");
                Console.WriteLine("{0,-25} {1,20}", "Customer Name:", $"{reservation.Customer.FirstName} {reservation.Customer.LastName}");
                Console.WriteLine("{0,-25} {1,20}", "Customer Address:", $"{reservation.Customer.Address}");
                Console.WriteLine("{0,-25} {1,20}", "Customer City/St/ZIP:", $"{reservation.Customer.City}, {reservation.Customer.State} {reservation.Customer.PostalCode.Substring(0, 5)}");
                Console.WriteLine();
                Console.WriteLine("{0,-25} {1,20}", "Vehicle:", $"{reservation.RentableVehicle.Year} {reservation.RentableVehicle.Make} {reservation.RentableVehicle.Model} ({reservation.RentableVehicleID})");
                Console.WriteLine("{0,-25} {1,20}", "Rented date:", $"{reservation.RentalDate:d}");
                Console.WriteLine("{0,-25} {1,20}", "Expected return date:.", $"{reservation.ExpectedReturnDate:d}");

                if (reservation.ActualReturnDate != null)
                {

                    decimal totalDays = (decimal)(reservation.ActualReturnDate.Value - reservation.RentalDate).TotalDays;
                    decimal fuelRemaining = reservation.FuelRemaining.Value;
                    decimal fuelSurcharge = 0M;
                    if (reservation.FuelRemaining < 0.90M)
                    {
                        fuelSurcharge = ((1.00M - fuelRemaining) * reservation.RentableVehicle.FuelCapacity) * FUEL_SURCHARGE_DOLLARS_PER_UOM;
                    }
                    decimal lateCharge = 0M;
                    decimal daysLate = (decimal)(reservation.ActualReturnDate.Value.Date - reservation.ExpectedReturnDate.Date).TotalDays;
                    if (daysLate > 0)
                    {
                        lateCharge = daysLate * reservation.DailyRate * LATE_CHARGE_PERCENTAGE;
                    }


                    Console.WriteLine("{0,-25} {1,20}", "Actual return date:", $"{reservation.ActualReturnDate:d}");
                    Console.WriteLine();
                    Console.WriteLine("{0,-30} {1,30}", "Daily rate:", $"{reservation.DailyRate:C}");
                    Console.WriteLine("{0,-20} {1,20}", "Number of days:", $"{totalDays:0}");
                    Console.WriteLine("{0,-30} {1,30}", "Base total:", $"{totalDays * reservation.DailyRate:C}");

                    if (fuelSurcharge > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("{0,-20} {1,20}", "Fuel owed @ cost:", $"{1.00M - fuelRemaining} @ {FUEL_SURCHARGE_DOLLARS_PER_UOM:C}");
                        Console.WriteLine("{0,-30} {1,30}", "Fuel surcharges:", $"{fuelSurcharge:C}");
                    }

                    if (lateCharge > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("{0,-20} {1,20}", "Day(s) late @ charge/day:", $"{daysLate} @ {LATE_CHARGE_PERCENTAGE:P}");
                        Console.WriteLine("{0,-30} {1,30}", "Late charges:", $"{lateCharge:C}");
                    }


                    Console.WriteLine();
                    if (reservation.SelfPay)
                    {
                        Console.WriteLine("With Self Pay Discount: ");
                    }
                    Console.WriteLine("{0,-30} {1,30}", "Subtotal:", $"{reservation.BilledSubtotal:C}");

                    Console.WriteLine("{0,-30} {1,30}", $"Tax @ {SALES_TAX_PERCENTAGE:P}:", $"{reservation.BilledTax:C}");
                    Console.WriteLine();
                    Console.WriteLine("{0,-30} {1,30}", "Total:", $"{reservation.BilledTotal:C}");
                    Console.WriteLine($"------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("****VEHICLE NOT YET RETURNED****");
                }
            }
            else
            {
                Console.WriteLine($"Reservation {reservationID} is invalid");
            }
        }

        public void DisplayActiveReservations()
        {
            Console.Clear();

            var activeReservations =
                _dbContext.Reservations
                    .Where(r => r.ActualReturnDate == null);

            DisplayReservationList(activeReservations);
            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        public void DisplayReturnVehicleMenu()
        {
            Console.Clear();

            var activeReservations =
                _dbContext.Reservations
                    .Where(r => r.ActualReturnDate == null);

            if (activeReservations.Any())
            {
                DisplayReservationList(activeReservations);

                bool validSubInput = false;
                string reservationInput = null;
                do
                {
                    Console.WriteLine("Specify the reservation ID number to edit, or enter RT to return");
                    reservationInput = Console.ReadLine();

                    if (reservationInput.ToUpperInvariant() == "RT")
                    {
                        validSubInput = true;
                        return;
                    }

                    if (int.TryParse(reservationInput, out int parsedReservationID))
                    {
                        if (activeReservations.Any(a => a.ReservationID == parsedReservationID))
                        {
                            validSubInput = true;
                            EndReservation(parsedReservationID);
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

        private void EndReservation(int parsedReservationID)
        {
            var reservation = _dbContext.Reservations.First(r => r.ReservationID == parsedReservationID);

            Console.Clear();
            string input = null;

            DateTime actualReturnDate = DateTime.Now;
            Console.WriteLine($"Specify the return date in YYYY-MM-DD format, or press <ENTER> to use today's date {DateTime.Now:d}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                actualReturnDate = DateTime.Parse(input);
            }

            decimal newMileage = reservation.RentableVehicle.CurrentOdometerReading;
            Console.WriteLine($"Specify the current odometer reading, or press <ENTER> to use current reading of {newMileage}: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                newMileage = decimal.Parse(input);
            }

            decimal fuelRemaining = 1.00M;
            Console.WriteLine("Specify the current percentage of fuel remaining in 0.XX format, or press <ENTER> for a full tank/battery: ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                fuelRemaining = decimal.Parse(input);
            }

            reservation.ActualReturnDate = actualReturnDate;
            reservation.TotalTripMileage = reservation.RentableVehicle.CurrentOdometerReading + newMileage;
            reservation.RentableVehicle.CurrentOdometerReading = newMileage;
            reservation.FuelRemaining = fuelRemaining;

            decimal totalDays = (decimal)((actualReturnDate - reservation.RentalDate).TotalDays);
            decimal baseTotal = totalDays * reservation.DailyRate;

            // If tank is returned with less than 90 percent of the fuel remaining, apply a surcharge
            decimal fuelSurcharge = 0M;
            if (reservation.FuelRemaining < 0.90M)
            {
                fuelSurcharge = ((1.00M - fuelRemaining) * reservation.RentableVehicle.FuelCapacity) * FUEL_SURCHARGE_DOLLARS_PER_UOM;
            }

            // If vehicle was returned after expected date, bill a surcharge
            decimal lateCharge = 0M;
            decimal daysLate = (decimal)(reservation.ActualReturnDate.Value.Date - reservation.ExpectedReturnDate.Date).TotalDays;
            if (daysLate > 0)
            {
                lateCharge = daysLate * reservation.DailyRate * LATE_CHARGE_PERCENTAGE;
            }

            reservation.BilledSubtotal = baseTotal + fuelSurcharge + lateCharge;
            if (reservation.SelfPay)
            {
                reservation.BilledSubtotal -= (Decimal).15 * reservation.BilledSubtotal;
            }
            reservation.BilledTax = (baseTotal * SALES_TAX_PERCENTAGE);
            reservation.BilledTotal = reservation.BilledSubtotal.Value + reservation.BilledTax.Value;

            _dbContext.SaveChanges();

            DisplayReservationByID(reservation.ReservationID);
            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        private void CreateReservation(int vehicleID, DateTime rentalDate)
        {
            Console.Clear();
            Console.WriteLine("Enter the following rental information: ");
            Console.WriteLine();
            try
            {
                var vehicle = _dbContext.RentableVehicles.First(v => v.RentableVehicleID == vehicleID);

                int customerID = 0;
                Console.Write($"Enter the customer ID number for the reservation: ");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerID = int.Parse(input);
                }

                decimal dailyRate = vehicle.StandardDailyRate;
                Console.Write($"This vehicle's daily rate is {dailyRate:C} - press <ENTER> to accept rate, or key in a new rate: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    dailyRate = decimal.Parse(input);
                }

                DateTime expectedReturnDate = DateTime.Now;
                Console.Write($"What is the expected date of return? (Enter in format YYYY-MM-DD): ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    expectedReturnDate = DateTime.Parse(input);
                }

                var reservation = _dbContext.Reservations.Add(new Reservation
                {
                    AgentID = _agentService.GetLoggedInAgentID(),
                    CustomerID = customerID,
                    RentalDate = rentalDate,
                    ExpectedReturnDate = expectedReturnDate,
                    RentableVehicle = vehicle,
                    DailyRate = dailyRate
                }
                );
                _dbContext.SaveChanges();

                Console.WriteLine($"Reservation {reservation.Entity.ReservationID} booked");
                Console.WriteLine("Press <ENTER> to continue");
                Console.ReadLine();
            } catch (Exception e)
            {
                Console.WriteLine("Error occured: ", e.Message);
            }
        }


        public void DisplayReservationList(IEnumerable<Reservation> reservations)
        {
            Console.WriteLine("ID    VehID Year Make       Expected Return");
            Console.WriteLine("-------------------------------------------");

            if (reservations != null && reservations.Any())
            {
                foreach (var reservation in reservations)
                {
                    Console.Write(reservation.ReservationID.ToString().PadLeft(5));
                    Console.Write(' ');
                    Console.Write(reservation.RentableVehicle.RentableVehicleID.ToString().PadLeft(5));
                    Console.Write(' ');
                    Console.Write(reservation.RentableVehicle.Year.PadRight(4));
                    Console.Write(' ');
                    Console.Write(reservation.RentableVehicle.Make.PadRight(10));
                    Console.Write(' ');
                    Console.Write(reservation.ExpectedReturnDate.ToString("M").PadRight(10));
                    Console.WriteLine();                    
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("There are no active reservations.");
            }
        }
    }
}