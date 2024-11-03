namespace LibrarySystem.Models
{
    public class RentalViewModel
    {
        public int RentalId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int BookId { get; set; } // Add this property
        public string BookTitle { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;
        public string FormattedRentalDate => RentalDate.ToString("yyyy-MM-dd");
        public string FormattedReturnDate => ReturnDate?.ToString("yyyy-MM-dd") ?? "Not Returned";
    }
}
