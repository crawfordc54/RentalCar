using Lab6.Data;
using Lab6.Services;
using System;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            using var dbContext = new ReservationSystemContext();

            AgentService agentService = new AgentService(dbContext);

            if (agentService.Authenticate())
            {
                RentableVehicleService rentableVehicleService = new RentableVehicleService(dbContext);
                CustomerService customerService = new CustomerService(dbContext);
                ReservationService reservationService = new ReservationService(dbContext, agentService, rentableVehicleService);

                MainMenuService menu = new MainMenuService(agentService, rentableVehicleService, customerService, reservationService);
                menu.StartMenu();
            }
            else
            {
                Console.WriteLine("Access denied");
            }
        }
    }
}
