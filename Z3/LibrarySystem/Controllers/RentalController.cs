using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class RentalController : Controller
    {
        private readonly string _rentalsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rentals.json");
        private readonly string _clientsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "clients.json");
        private readonly string _booksJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books.json");

        // Load all rentals from the JSON file
        private List<Rental> LoadRentals()
        {
            if (!System.IO.File.Exists(_rentalsJsonFilePath))
            {
                return new List<Rental>();
            }

            var jsonData = System.IO.File.ReadAllText(_rentalsJsonFilePath);
            return JsonConvert.DeserializeObject<List<Rental>>(jsonData) ?? new List<Rental>();
        }

        // Save the list of rentals to the JSON file
        private void SaveRentals(List<Rental> rentals)
        {
            var jsonData = JsonConvert.SerializeObject(rentals, Formatting.Indented);
            System.IO.File.WriteAllText(_rentalsJsonFilePath, jsonData);
        }

        // Load all clients from the JSON file
        private List<Client> LoadClients()
        {
            if (!System.IO.File.Exists(_clientsJsonFilePath))
            {
                return new List<Client>();
            }

            var jsonData = System.IO.File.ReadAllText(_clientsJsonFilePath);
            return JsonConvert.DeserializeObject<List<Client>>(jsonData) ?? new List<Client>();
        }

        // Load all books from the JSON file
        private List<Book> LoadBooks()
        {
            if (!System.IO.File.Exists(_booksJsonFilePath))
            {
                return new List<Book>();
            }

            var jsonData = System.IO.File.ReadAllText(_booksJsonFilePath);
            return JsonConvert.DeserializeObject<List<Book>>(jsonData) ?? new List<Book>();
        }

        // Save the list of books to the JSON file
        private void SaveBooks(List<Book> books)
        {
            var jsonData = JsonConvert.SerializeObject(books, Formatting.Indented);
            System.IO.File.WriteAllText(_booksJsonFilePath, jsonData);
        }

        // Index action to list all rentals with optional sorting, filtering, and searching
        public IActionResult Index(
            string sortColumn = null,
            string sortOrder = null,
            DateTime? filterRentalDate = null,
            DateTime? filterReturnDate = null,
            string filterClientName = null,
            string filterBookTitle = null,
            string filterReturnStatus = null)
        {
            var rentals = LoadRentals();
            var clients = LoadClients();
            var books = LoadBooks();

            // Merge rental data with client and book details for display
            var rentalDetails = rentals.Select(rental => new RentalViewModel
            {
                RentalId = rental.Id,
                ClientId = rental.ClientId,  // Ensure ClientId is correctly assigned here
                BookId = rental.BookId,
                ClientName = clients.FirstOrDefault(c => c.Id == rental.ClientId)?.Name ?? "Unknown Client",
                BookTitle = books.FirstOrDefault(b => b.Id == rental.BookId)?.Title ?? "Unknown Book",
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
            }).ToList();


            // Apply filtering based on rental date
            if (filterRentalDate.HasValue)
            {
                rentalDetails = rentalDetails.Where(r => r.RentalDate.Date == filterRentalDate.Value.Date).ToList();
            }

            // Apply filtering based on return date
            if (filterReturnDate.HasValue)
            {
                rentalDetails = rentalDetails.Where(r => r.ReturnDate.HasValue && r.ReturnDate.Value.Date == filterReturnDate.Value.Date).ToList();
            }

            // Apply filtering based on client name
            if (!string.IsNullOrEmpty(filterClientName))
            {
                rentalDetails = rentalDetails.Where(r => r.ClientName.Contains(filterClientName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply filtering based on book title
            if (!string.IsNullOrEmpty(filterBookTitle))
            {
                rentalDetails = rentalDetails.Where(r => r.BookTitle.Contains(filterBookTitle, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply filtering based on return status
            if (!string.IsNullOrEmpty(filterReturnStatus))
            {
                if (filterReturnStatus == "Returned")
                {
                    rentalDetails = rentalDetails.Where(r => r.ReturnDate.HasValue).ToList();
                }
                else if (filterReturnStatus == "Not Returned")
                {
                    rentalDetails = rentalDetails.Where(r => !r.ReturnDate.HasValue).ToList();
                }
            }

            // Apply sorting based on the column and order
            rentalDetails = SortRentals(rentalDetails, sortColumn, sortOrder);

            // Pass sorting and filtering state to the view
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.FilterRentalDate = filterRentalDate;
            ViewBag.FilterReturnDate = filterReturnDate;
            ViewBag.FilterClientName = filterClientName;
            ViewBag.FilterBookTitle = filterBookTitle;
            ViewBag.FilterReturnStatus = filterReturnStatus;

            return View(rentalDetails);
        }

        // AJAX endpoint to get sorted, filtered, and searched rentals without reloading the page
        public IActionResult GetSortedRentals(
            string sortColumn,
            string sortOrder,
            DateTime? filterRentalDate = null,
            DateTime? filterReturnDate = null,
            string filterClientName = null,
            string filterBookTitle = null,
            string filterReturnStatus = null)
        {
            var rentals = LoadRentals();
            var clients = LoadClients();
            var books = LoadBooks();

            // Merge rental data with client and book details for display
            var rentalDetails = rentals.Select(rental => new RentalViewModel
            {
                RentalId = rental.Id,
                ClientId = rental.ClientId, // Ensure ClientId is included
                ClientName = clients.FirstOrDefault(c => c.Id == rental.ClientId)?.Name ?? "Unknown Client",
                BookId = rental.BookId,
                BookTitle = books.FirstOrDefault(b => b.Id == rental.BookId)?.Title ?? "Unknown Book",
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate
            }).ToList();

            // Apply filtering based on rental date
            if (filterRentalDate.HasValue)
            {
                rentalDetails = rentalDetails.Where(r => r.RentalDate.Date == filterRentalDate.Value.Date).ToList();
            }

            // Apply filtering based on return date
            if (filterReturnDate.HasValue)
            {
                rentalDetails = rentalDetails.Where(r => r.ReturnDate.HasValue && r.ReturnDate.Value.Date == filterReturnDate.Value.Date).ToList();
            }

            // Apply filtering based on client name
            if (!string.IsNullOrEmpty(filterClientName))
            {
                rentalDetails = rentalDetails.Where(r => r.ClientName.Contains(filterClientName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply filtering based on book title
            if (!string.IsNullOrEmpty(filterBookTitle))
            {
                rentalDetails = rentalDetails.Where(r => r.BookTitle.Contains(filterBookTitle, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply filtering based on return status
            if (!string.IsNullOrEmpty(filterReturnStatus))
            {
                if (filterReturnStatus == "Returned")
                {
                    rentalDetails = rentalDetails.Where(r => r.ReturnDate.HasValue).ToList();
                }
                else if (filterReturnStatus == "Not Returned")
                {
                    rentalDetails = rentalDetails.Where(r => !r.ReturnDate.HasValue).ToList();
                }
            }

            // Apply sorting based on the column and order
            rentalDetails = SortRentals(rentalDetails, sortColumn, sortOrder);

            // Return sorted, filtered, and searched rentals as JSON response
            return Json(rentalDetails.Select(r => new
            {
                r.RentalId,
                r.ClientId, // Include ClientId in the JSON response
                r.ClientName,
                r.BookId,
                r.BookTitle,
                RentalDate = r.RentalDate.ToString("yyyy-MM-dd"), // Format to yyyy-MM-dd
                ReturnDate = r.ReturnDate?.ToString("yyyy-MM-dd") ?? "Not Returned", // Format or "Not Returned"
                IsReturned = r.ReturnDate.HasValue // Use whether ReturnDate has a value for the return status
            }));
        }


        // Helper function to handle sorting of rentals
        private List<RentalViewModel> SortRentals(List<RentalViewModel> rentals, string sortColumn, string sortOrder)
        {
            return sortColumn switch
            {
                "ClientName" => sortOrder == "asc" ? rentals.OrderBy(r => r.ClientName).ToList() :
                                sortOrder == "desc" ? rentals.OrderByDescending(r => r.ClientName).ToList() : rentals,
                "BookTitle" => sortOrder == "asc" ? rentals.OrderBy(r => r.BookTitle).ToList() :
                               sortOrder == "desc" ? rentals.OrderByDescending(r => r.BookTitle).ToList() : rentals,
                "RentalDate" => sortOrder == "asc" ? rentals.OrderBy(r => r.RentalDate).ToList() :
                                sortOrder == "desc" ? rentals.OrderByDescending(r => r.RentalDate).ToList() : rentals,
                "ReturnDate" => sortOrder == "asc" ? rentals.OrderBy(r => r.ReturnDate).ToList() :
                                sortOrder == "desc" ? rentals.OrderByDescending(r => r.ReturnDate).ToList() : rentals,
                _ => rentals.OrderBy(r => r.RentalId).ToList() // Default sort by RentalId if no sorting is specified
            };
        }

        // Create action - Display the form to add a new rental
        public IActionResult Create()
        {
            // Load and sort clients alphabetically by name
            ViewBag.Clients = LoadClients().OrderBy(c => c.Name).ToList();

            // Load and sort available books alphabetically by title
            ViewBag.AvailableBooks = LoadBooks().Where(b => !b.IsRented).OrderBy(b => b.Title).ToList();

            return View();
        }

        // Create action - Save new rental to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rental rental)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = LoadClients();
                ViewBag.AvailableBooks = LoadBooks().Where(b => !b.IsRented).ToList();
                return View(rental);
            }

            var rentals = LoadRentals();
            var books = LoadBooks();
            var bookToRent = books.FirstOrDefault(b => b.Id == rental.BookId);

            if (bookToRent == null || bookToRent.IsRented)
            {
                ModelState.AddModelError("", "The selected book is not available for rent.");
                ViewBag.Clients = LoadClients();
                ViewBag.AvailableBooks = books.Where(b => !b.IsRented).ToList();
                return View(rental);
            }

            // Mark the book as rented
            bookToRent.IsRented = true;
            SaveBooks(books);

            // Assign a new ID to the rental
            rental.Id = rentals.Any() ? rentals.Max(r => r.Id) + 1 : 1;
            rental.RentalDate = DateTime.Now;
            rentals.Add(rental);

            // Save the updated rental list to the JSON file
            SaveRentals(rentals);

            // Redirect to the index action to display all rentals
            return RedirectToAction(nameof(Index));
        }

        // Edit action - Display the form to edit an existing rental
        public IActionResult Edit(int id)
        {
            var rentals = LoadRentals();
            var rental = rentals.FirstOrDefault(r => r.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            ViewBag.Clients = LoadClients();
            ViewBag.Books = LoadBooks();
            return View(rental);
        }

        // Edit action - Save updated rental details to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Rental updatedRental)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = LoadClients();
                ViewBag.Books = LoadBooks();
                return View(updatedRental);
            }

            var rentals = LoadRentals();
            var rental = rentals.FirstOrDefault(r => r.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            // Update rental details
            rental.ClientId = updatedRental.ClientId;
            rental.BookId = updatedRental.BookId;
            rental.RentalDate = updatedRental.RentalDate;
            rental.ReturnDate = updatedRental.ReturnDate;

            // Save the updated rental list to the JSON file
            SaveRentals(rentals);

            // Redirect to the index action to display all rentals
            return RedirectToAction(nameof(Index));
        }

        // Delete action - Confirm deletion of a rental
        public IActionResult Delete(int id)
        {
            var rentals = LoadRentals();
            var rental = rentals.FirstOrDefault(r => r.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            var clients = LoadClients();
            var books = LoadBooks();

            // Merge client and book details for display
            ViewBag.ClientName = clients.FirstOrDefault(c => c.Id == rental.ClientId)?.Name ?? "Unknown Client";
            ViewBag.BookTitle = books.FirstOrDefault(b => b.Id == rental.BookId)?.Title ?? "Unknown Book";
            return View(rental);
        }

        // Delete action - Remove a rental from the JSON file
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var rentals = LoadRentals();
            var rental = rentals.FirstOrDefault(r => r.Id == id);

            if (rental != null)
            {
                // Update the status of the associated book to "not rented"
                var books = LoadBooks();
                var bookToUpdate = books.FirstOrDefault(b => b.Id == rental.BookId);

                if (bookToUpdate != null)
                {
                    bookToUpdate.IsRented = false;
                    SaveBooks(books); // Save the updated list of books
                }

                // Remove the rental from the list and save
                rentals.Remove(rental);
                SaveRentals(rentals);
            }

            // Redirect to the index action to display all rentals
            return RedirectToAction(nameof(Index));
        }

        // Return action - Return a rented book
        public IActionResult Return(int id, string sortColumn = null, string sortOrder = null, DateTime? filterRentalDate = null, DateTime? filterReturnDate = null, string filterClientName = null, string filterBookTitle = null, string filterReturnStatus = null)
        {
            var rentals = LoadRentals();
            var books = LoadBooks();
            var rental = rentals.FirstOrDefault(r => r.Id == id);

            if (rental != null && rental.ReturnDate == null)
            {
                // Mark the book as available
                var bookToReturn = books.FirstOrDefault(b => b.Id == rental.BookId);
                if (bookToReturn != null)
                {
                    bookToReturn.IsRented = false;
                    SaveBooks(books);
                }

                // Set the return date
                rental.ReturnDate = DateTime.Now;
                SaveRentals(rentals);
            }

            // Redirect back to the Index action with the same filters and sorting
            return RedirectToAction(nameof(Index), new
            {
                sortColumn,
                sortOrder,
                filterRentalDate,
                filterReturnDate,
                filterClientName,
                filterBookTitle,
                filterReturnStatus
            });
        }

    }
}
