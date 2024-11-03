using System.Collections.Generic;

namespace LibrarySystem.Models
{
    public class ClientProfileViewModel
    {
        public Client Client { get; set; }
        public List<RentalViewModel> RentalHistory { get; set; }
    }
}
