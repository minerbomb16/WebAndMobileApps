namespace LibrarySystem.Models
{
    public class BookProfileViewModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public bool IsRented { get; set; }
        public List<RentalHistoryEntry> RentalHistory { get; set; }
    }

    public class RentalHistoryEntry
    {
        public string ClientName { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
