using System;

namespace Lab6.Data
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int AgentID { get; set; }
        public int? CustomerID { get; set; }
        public int RentableVehicleID { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public decimal DailyRate { get; set; }
        public bool SelfPay { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public decimal? TotalTripMileage { get; set; }
        public decimal? FuelRemaining { get; set; }
        public string BilledTo { get; set; }
        public decimal? BilledSubtotal { get; set; }
        public decimal? BilledTax { get; set; }
        public decimal? BilledTotal { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual RentableVehicle RentableVehicle { get; set; }
    }
}
