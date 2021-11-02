using Lab6.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lab6.Services
{
    public class CustomerService
    {
        private readonly ReservationSystemContext _dbContext;

        public CustomerService(ReservationSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DisplayManageCustomersMenu()
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
            Console.WriteLine(" AC - Add a customer");
            Console.WriteLine(" SC - Search for a customer");
            Console.WriteLine(" UC - Update a customer");
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
                    case "AC":
                        validInput = true;
                        AddCustomer();
                        break;
                    case "SC":
                        validInput = true;
                        SearchForCustomer();
                        break;
                    case "UC":
                        validInput = true;
                        UpdateCustomer();
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

        private void AddCustomer()
        {
            Console.Clear();
            Console.WriteLine("Enter new customer information:");
            Console.WriteLine();

            Console.Write("Enter customer first name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter customer last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter date of birth in YYYY-MM-DD format: ");
            string dateOfBirth = Console.ReadLine();

            Console.Write("Enter address: ");
            string address = Console.ReadLine();

            Console.Write("Enter city: ");
            string city = Console.ReadLine();

            Console.Write("Enter state: ");
            string state = Console.ReadLine();

            Console.Write("Enter ZIP code: ");
            string postalCode = Console.ReadLine();
            try
            {
                int zip = Int32.Parse(postalCode);
            } catch(Exception e)
            {
                Console.WriteLine("Customer operation failed. Invalid Zip Code.");
                return;
            }
            

            Console.Write("Enter phone number in XXX-YYY-ZZZZ format: ");
            string phone = Console.ReadLine();

            var newVehicle = _dbContext.Customers.Add(new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = DateTime.Parse(dateOfBirth),
                Address = address,
                City = city,
                State = state,
                PostalCode = postalCode,
                PhoneNumber = phone
            }
            );
            _dbContext.SaveChanges();

            Console.WriteLine($"Vehicle {newVehicle.Entity.CustomerID} added");
        }
        private void SearchForCustomer()
        {
            Console.Clear();

            string customerSearchInput = null;
            bool validSubInput = false;
            do
            {
                Console.WriteLine("Specify a last name or a phone number, or enter RT to return");
                customerSearchInput = Console.ReadLine();

                if (customerSearchInput.ToUpperInvariant() == "RT")
                {
                    validSubInput = true;
                    return;
                }

                if (!string.IsNullOrEmpty(customerSearchInput))
                {
                    validSubInput = true;

                    var foundCustomers =
                        _dbContext.Customers.Where(cust =>
                        cust.LastName.ToLower().Contains(customerSearchInput.ToLower())
                        || cust.FirstName.ToLower().Contains(customerSearchInput.ToLower())
                        || cust.PhoneNumber.Replace("-", "").Contains(customerSearchInput.Replace("-", "")));

                    Console.WriteLine("   ID First Name      Last Name       City            St ZIP   Phone");
                    Console.WriteLine("-----------------------------------------------------------------------");
                    if (foundCustomers != null && foundCustomers.Any())
                    {
                        foreach(var customer in foundCustomers)
                        {
                            Console.Write(customer.CustomerID.ToString().PadLeft(5));
                            Console.Write(' ');
                            Console.Write(customer.FirstName.PadRight(15));
                            Console.Write(' ');
                            Console.Write(customer.LastName.PadRight(15));
                            Console.Write(' ');
                            Console.Write(customer.City.PadRight(15));
                            Console.Write(' ');
                            Console.Write(customer.State.PadRight(2));
                            Console.Write(' ');
                            Console.Write(customer.PostalCode.Substring(0,5).PadRight(5));
                            Console.Write(' ');
                            Console.Write(customer.PhoneNumber.PadRight(12));
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No customers were found with the specified search criteria");
                    }
                }
                else
                {
                    Console.WriteLine("You must specify a search criterion");
                }

            } while (!validSubInput);

            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }
        
        public void UpdateCustomer()
        {
            bool validSubInput = false;
            string customerInput = null;
            do
            {
                Console.WriteLine("Specify the customer ID number to edit, or enter RT to return");
                customerInput = Console.ReadLine();

                if (customerInput.ToUpperInvariant() == "RT")
                {
                    validSubInput = true;
                    return;
                }

                if (int.TryParse(customerInput, out int parsedCustomerID))
                {
                    if (_dbContext.Customers.Any(a => a.CustomerID == parsedCustomerID))
                    {
                        validSubInput = true;
                        UpdateCustomer(parsedCustomerID);
                    }
                }
            } while (!validSubInput);
        }
        

        private void UpdateCustomer(int customerID)
        {
            Console.Clear();
            Console.WriteLine("Enter updated vehicle information:");
            Console.WriteLine();

            string input = null;

            var customerToUpdate = _dbContext.Customers.FirstOrDefault(v => v.CustomerID == customerID);

            if (customerToUpdate != null)
            {

                Console.Write($"Enter customer first name, or press <ENTER> to leave {customerToUpdate.FirstName}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.FirstName = input;
                }

                Console.Write($"Enter customer last name, or press <ENTER> to leave {customerToUpdate.LastName}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.LastName = input;
                }

                Console.Write($"Enter customer date of birth in YYYY-MM-DD format, or press <ENTER> to leave {customerToUpdate.DateOfBirth:d}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.DateOfBirth = DateTime.Parse(input);
                }

                Console.Write($"Enter customer address, or press <ENTER> to leave {customerToUpdate.Address}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.Address = input;
                }

                Console.Write($"Enter customer city, or press <ENTER> to leave {customerToUpdate.City}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.City = input;
                }

                Console.Write($"Enter customer state, or press <ENTER> to leave {customerToUpdate.State}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.State = input;
                }

                Console.Write($"Enter customer ZIP code, or press <ENTER> to leave {customerToUpdate.PostalCode}: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.PostalCode = input;
                }

                Console.Write($"Enter customer phone number in XXX-YYY-ZZZZ format, or press <ENTER> to leave {customerToUpdate.PhoneNumber}: ");   
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    customerToUpdate.PhoneNumber = input;
                }

                _dbContext.SaveChanges();

                Console.WriteLine("Customer has been updated");
            }
            else
            {
                Console.WriteLine($"ID number {customerID} is not a valid customer");
            }

            Console.WriteLine("Press <ENTER> to continue");
            Console.ReadLine();
        }

        private void DisplaySubmenuHeading()
        {
            Console.Clear();
            Console.WriteLine("Enter a menu option to continue:");
            Console.WriteLine();
        }
    }
}
