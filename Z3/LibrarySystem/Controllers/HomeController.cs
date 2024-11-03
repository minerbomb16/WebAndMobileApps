using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _rentalsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rentals.json");
        private readonly string _booksJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books.json");
        private readonly string _clientsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "clients.json");

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

        // Dashboard view with chart data
        public IActionResult Index()
        {
            var rentals = LoadRentals();
            var books = LoadBooks();
            var clients = LoadClients();

            // Prepare data for the ViewModel
            var dashboardData = new DashboardViewModel
            {
                TotalRentals = rentals.Count,
                TotalBooks = books.Count,
                TotalClients = clients.Count,  // Set the TotalClients property
                RentalDates = rentals
                    .GroupBy(r => r.RentalDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Key.ToString("yyyy-MM-dd"))
                    .ToList(),
                RentalCounts = rentals
                    .GroupBy(r => r.RentalDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Count())
                    .ToList(),
                MostRentedBookTitles = rentals
                    .GroupBy(r => r.BookId)
                    .Select(g => new
                    {
                        Title = books.FirstOrDefault(b => b.Id == g.Key)?.Title ?? "Unknown",
                        Count = g.Count()
                    })
                    .OrderByDescending(g => g.Count)
                    .Take(5)
                    .Select(g => g.Title)
                    .ToList(),
                MostRentedBookCounts = rentals
                    .GroupBy(r => r.BookId)
                    .Select(g => g.Count())
                    .OrderByDescending(g => g)
                    .Take(5)
                    .ToList(),
                MonthlyRentals = rentals
                    .GroupBy(r => new { r.RentalDate.Year, r.RentalDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new MonthlyRentalData
                    {
                        YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Count = g.Count()
                    })
                    .ToList()
            };

            return View(dashboardData);
        }
    }
}
