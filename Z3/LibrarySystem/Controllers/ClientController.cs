using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class ClientController : Controller
    {
        private readonly string _clientsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "clients.json");
        private readonly string _rentalsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rentals.json");
        private readonly string _booksJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books.json");

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

        // Save the list of clients to the JSON file
        private void SaveClients(List<Client> clients)
        {
            var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
            System.IO.File.WriteAllText(_clientsJsonFilePath, jsonData);
        }

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

        // Index action to list all clients with optional sorting, filtering, and searching
        public IActionResult Index(string sortColumn = null, string sortOrder = null, string searchQuery = null)
        {
            var clients = LoadClients();

            // Apply searching based on name, email, or phone
            if (!string.IsNullOrEmpty(searchQuery))
            {
                clients = clients.Where(c => c.Name.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                                             c.Email.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                                             c.Phone.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply sorting based on the column and order
            clients = SortClients(clients, sortColumn, sortOrder);

            // Pass sorting and searching state to the view
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchQuery = searchQuery;

            return View(clients);
        }

        // AJAX endpoint to get sorted and searched clients without reloading the page
        public IActionResult GetSortedClients(string sortColumn, string sortOrder, string searchQuery = null)
        {
            var clients = LoadClients();

            // Apply searching based on name, email, or phone
            if (!string.IsNullOrEmpty(searchQuery))
            {
                clients = clients.Where(c => c.Name.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                                             c.Email.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                                             c.Phone.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply sorting based on the column and order
            clients = SortClients(clients, sortColumn, sortOrder);

            // Project client data to send only necessary fields as JSON response
            var clientData = clients.Select(c => new
            {
                id = c.Id,       // Ensure lowercase names match the JavaScript expectations
                name = c.Name,
                email = c.Email,
                phone = c.Phone
            }).ToList();

            return Json(clientData);
        }

        // Helper function to handle sorting of clients
        private List<Client> SortClients(List<Client> clients, string sortColumn, string sortOrder)
        {
            return sortColumn switch
            {
                "Name" => sortOrder == "asc" ? clients.OrderBy(c => c.Name).ToList() :
                          sortOrder == "desc" ? clients.OrderByDescending(c => c.Name).ToList() : clients,
                "Email" => sortOrder == "asc" ? clients.OrderBy(c => c.Email).ToList() :
                           sortOrder == "desc" ? clients.OrderByDescending(c => c.Email).ToList() : clients,
                "Phone" => sortOrder == "asc" ? clients.OrderBy(c => c.Phone).ToList() :
                           sortOrder == "desc" ? clients.OrderByDescending(c => c.Phone).ToList() : clients,
                _ => clients.OrderBy(c => c.Id).ToList() // Default sort by ID if no sorting is specified
            };
        }

        // Create action - Display the form to add a new client
        public IActionResult Create()
        {
            return View();
        }

        // Create action - Save new client to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var clients = LoadClients();
            // Assign a new ID to the client
            client.Id = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;
            clients.Add(client);

            // Save the updated client list to the JSON file
            SaveClients(clients);

            // Redirect to the index action to display all clients
            return RedirectToAction(nameof(Index));
        }

        // Edit action - Display form to edit an existing client
        public IActionResult Edit(int id)
        {
            var clients = LoadClients();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // Edit action - Save updated client details to JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Client updatedClient)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedClient);
            }

            var clients = LoadClients();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            // Update client details
            client.Name = updatedClient.Name;
            client.Email = updatedClient.Email;
            client.Phone = updatedClient.Phone;

            // Save the updated client list to the JSON file
            SaveClients(clients);

            // Redirect to the index action to display all clients
            return RedirectToAction(nameof(Index));
        }

        // Delete action - Confirm deletion of a client
        public IActionResult Delete(int id)
        {
            var clients = LoadClients();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // Delete action - Remove a client from the JSON file
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var clients = LoadClients();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
                SaveClients(clients);
            }

            // Redirect to the index action to display all clients
            return RedirectToAction(nameof(Index));
        }

        // Client Profile action
        public IActionResult Profile(int id)
        {
            var clients = LoadClients();
            var rentals = LoadRentals();
            var books = LoadBooks();

            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            // Get rental history for this client
            var clientRentals = rentals
                .Where(r => r.ClientId == id)
                .Select(r => new RentalViewModel
                {
                    RentalId = r.Id,
                    ClientId = r.ClientId,
                    ClientName = client.Name,
                    BookId = r.BookId, // Ensure BookId is populated here
                    BookTitle = books.FirstOrDefault(b => b.Id == r.BookId)?.Title ?? "Unknown Book",
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate
                })
                .ToList();



            var model = new ClientProfileViewModel
            {
                Client = client,
                RentalHistory = clientRentals
            };

            return View(model);
        }
    }
}
