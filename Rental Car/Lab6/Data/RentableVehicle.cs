using System;

namespace Lab6.Data
{
    public class RentableVehicle
    {
        public int RentableVehicleID { get; set; }
        public string VehicleIdentificationNumber { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Submodel { get; set; }
        public string ExteriorColor { get; set; }
        public decimal FuelCapacity { get; set; }
        public DateTime DateAcquired { get; set; }
        public decimal AcquiredPrice { get; set; }
        public decimal InitialOdometerReading { get; set; }
        public decimal StandardDailyRate { get; set; }
        public decimal CurrentOdometerReading { get; set; }
        public DateTime? DateDisposed { get; set; }
        public string DisposalReason { get; set; }
        public decimal? DisposalPrice { get; set; }
    }
}
