using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;

namespace RubberDuckStore.Pages
{
    public class OrderFormModel : PageModel // using page model call
    {
        [BindProperty]
        public Order? Order { get; set; }
        public Duck? Duck { get; set; }
// implement onget method - overides parent class
public IActionResult OnGet(int duckId)
{
    Duck = GetDuckById(duckId);
    if (Duck == null)
    { // returns if no duck
        return NotFound();
    }
    Order = new Order { DuckId = duckId }; 
    return Page(); // returning duck page
} // clsoing onget method

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Duck = GetDuckById(Order.DuckId);
                return Page(); 
            }

            int orderId = SaveOrder(Order); // saves the order and sets into orderid

            return RedirectToPage("OrderConfirmation", new { orderId = orderId });
        } // closing OnPost

        private Duck GetDuckById(int id) // obtains data from db with sql connectio to get duck by its id number. this will run everytime a duck is selected
        {
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Duck // returns a new duck object with data set into new variables
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    }
                }
            }
            return null;
        } // closing GetDuckById

        private int SaveOrder(Order order) // this method will save the orer when inout
        {
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db")) // sql connection with parameters set below, wil run everytime an order is submitted
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Orders (DuckId, CustomerName, CustomerEmail, Quantity)
                    VALUES (@DuckId, @CustomerName, @CustomerEmail, @Quantity);
                    SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@DuckId", order.DuckId);
                command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                command.Parameters.AddWithValue("@CustomerEmail", order.CustomerEmail);
                command.Parameters.AddWithValue("@Quantity", order.Quantity);

                return Convert.ToInt32(command.ExecuteScalar()); // returning data from command, converting to int
            }
        } // closing SaveOrder
    }

    public class Order // model class for order with getters and setters
    {
        public int Id { get; set; }
        public int DuckId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required, EmailAddress]
        public string CustomerEmail { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    } // closing order
}