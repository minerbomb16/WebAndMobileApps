using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class BookController : Controller
    {
        private readonly string _jsonFilePathBooks = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books.json");
        private readonly string _jsonFilePathRentals = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rentals.json");
        private readonly string _jsonFilePathClients = Path.Combine(Directory.GetCurrentDirectory(), "Data", "clients.json");

        // Load data from JSON files
        private List<Book> LoadBooks() =>
            System.IO.File.Exists(_jsonFilePathBooks)
                ? JsonConvert.DeserializeObject<List<Book>>(System.IO.File.ReadAllText(_jsonFilePathBooks)) ?? new List<Book>()
                : new List<Book>();

        private List<Rental> LoadRentals() =>
            System.IO.File.Exists(_jsonFilePathRentals)
                ? JsonConvert.DeserializeObject<List<Rental>>(System.IO.File.ReadAllText(_jsonFilePathRentals)) ?? new List<Rental>()
                : new List<Rental>();

        private List<Client> LoadClients() =>
            System.IO.File.Exists(_jsonFilePathClients)
                ? JsonConvert.DeserializeObject<List<Client>>(System.IO.File.ReadAllText(_jsonFilePathClients)) ?? new List<Client>()
                : new List<Client>();

        // Save books to the JSON file
        private void SaveBooks(List<Book> books) =>
            System.IO.File.WriteAllText(_jsonFilePathBooks, JsonConvert.SerializeObject(books, Formatting.Indented));

        // List books with optional sorting, filtering, and searching
        public IActionResult Index(
            string sortColumn = null,
            string sortOrder = null,
            string filterGenre = null,
            int? filterYear = null,
            string searchQuery = null,
            bool? filterIsRented = null)
        {
            var books = LoadBooks();

            // Apply filtering, searching, and sorting
            books = FilterAndSearchBooks(books, filterGenre, filterYear, filterIsRented, searchQuery);
            books = SortBooks(books, sortColumn, sortOrder);

            // Set view data for the state
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.FilterGenre = filterGenre;
            ViewBag.FilterYear = filterYear;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.FilterIsRented = filterIsRented;

            return View(books);
        }

        // AJAX endpoint for sorted, filtered, and searched books
        public IActionResult GetSortedBooks(
            string sortColumn,
            string sortOrder,
            string filterGenre = null,
            int? filterYear = null,
            bool? filterIsRented = null,
            string searchQuery = null)
        {
            var books = LoadBooks();
            books = FilterAndSearchBooks(books, filterGenre, filterYear, filterIsRented, searchQuery);
            books = SortBooks(books, sortColumn, sortOrder);

            return Json(books);
        }

        // Filter and search books helper method
        private List<Book> FilterAndSearchBooks(List<Book> books, string genre, int? year, bool? isRented, string query) =>
            books.Where(b =>
                (string.IsNullOrEmpty(genre) || b.Genre == genre) &&
                (!year.HasValue || b.Year == year.Value) &&
                (!isRented.HasValue || b.IsRented == isRented.Value) &&
                (string.IsNullOrEmpty(query) ||
                 b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                 b.Author.Contains(query, StringComparison.OrdinalIgnoreCase)))
                 .ToList();

        // Helper function to handle sorting of books
        private List<Book> SortBooks(List<Book> books, string sortColumn, string sortOrder) =>
            sortColumn switch
            {
                "Title" => sortOrder == "asc" ? books.OrderBy(b => b.Title).ToList() : books.OrderByDescending(b => b.Title).ToList(),
                "Author" => sortOrder == "asc" ? books.OrderBy(b => b.Author).ToList() : books.OrderByDescending(b => b.Author).ToList(),
                "Genre" => sortOrder == "asc" ? books.OrderBy(b => b.Genre).ToList() : books.OrderByDescending(b => b.Genre).ToList(),
                "Year" => sortOrder == "asc" ? books.OrderBy(b => b.Year).ToList() : books.OrderByDescending(b => b.Year).ToList(),
                "IsRented" => sortOrder == "asc" ? books.OrderBy(b => b.IsRented).ToList() : books.OrderByDescending(b => b.IsRented).ToList(),
                _ => books.OrderBy(b => b.Id).ToList()
            };

        // Book creation form display
        public IActionResult Create() => View();

        // Save new book to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid) return View(book);

            var books = LoadBooks();
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);

            SaveBooks(books);
            return RedirectToAction(nameof(Index));
        }

        // Edit book form display
        public IActionResult Edit(int id)
        {
            var book = LoadBooks().FirstOrDefault(b => b.Id == id);
            return book == null ? NotFound() : View(book);
        }

        // Save updated book details to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book updatedBook)
        {
            if (!ModelState.IsValid) return View(updatedBook);

            var books = LoadBooks();
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            // Update book properties
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Genre = updatedBook.Genre;
            book.Year = updatedBook.Year;
            book.IsRented = updatedBook.IsRented;

            SaveBooks(books);
            return RedirectToAction(nameof(Index));
        }

        // Delete book confirmation display
        public IActionResult Delete(int id)
        {
            var book = LoadBooks().FirstOrDefault(b => b.Id == id);
            return book == null ? NotFound() : View(book);
        }

        // Confirm deletion and update JSON
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var books = LoadBooks();
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                books.Remove(book);
                SaveBooks(books);
            }

            return RedirectToAction(nameof(Index));
        }

        // Book profile action to display book details
        public IActionResult Profile(int id)
        {
            var books = LoadBooks();
            var rentals = LoadRentals();
            var clients = LoadClients();

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            var rentalHistory = rentals
                .Where(r => r.BookId == id)
                .Select(r => new RentalHistoryEntry
                {
                    ClientName = clients.FirstOrDefault(c => c.Id == r.ClientId)?.Name ?? "Unknown",
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate
                }).ToList();

            var viewModel = new BookProfileViewModel
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Year = book.Year,
                IsRented = book.IsRented,
                RentalHistory = rentalHistory
            };

            return View(viewModel);
        }
    }
}
