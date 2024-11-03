using System;
using System.Collections.Generic;

namespace LibrarySystem.Models
{
    public class DashboardViewModel
    {
        public int TotalRentals { get; set; }
        public int TotalBooks { get; set; }
        public int TotalClients { get; set; }  // Added back TotalClients
        public List<string> RentalDates { get; set; }
        public List<int> RentalCounts { get; set; }
        public List<string> MostRentedBookTitles { get; set; }
        public List<int> MostRentedBookCounts { get; set; }
        public List<MonthlyRentalData> MonthlyRentals { get; set; }
    }

    public class MonthlyRentalData
    {
        public string YearMonth { get; set; }
        public int Count { get; set; }
    }
}
