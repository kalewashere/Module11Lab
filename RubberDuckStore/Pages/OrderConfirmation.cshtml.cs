using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace RubberDuckStore.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        public Order Order { get; set; }
        public Duck Duck { get; set; }

        public IActionResult OnGet(int orderId) // 
        { 
            Order = GetOrderById(orderId); // sets order id into order
            if (Order == null)  // which is used here with a condition check, which, if passes, will return a message from the not found method
            {
                return NotFound();
            }
            Duck = GetDuckById(Order.DuckId); // if check fails the duck object is set
            return Page(); // duck page returned
        }

        private Order GetOrderById(int id)  // getting order from DB via sql and where clause, which has an Id parameter
        {
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db")) // sql connection that will run when command is given to obtain order
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Order
                        {
                            Id = reader.GetInt32(0),
                            DuckId = reader.GetInt32(1),
                            CustomerName = reader.GetString(2),
                            CustomerEmail = reader.GetString(3),
                            Quantity = reader.GetInt32(4)
                        };
                    }
                }
            }
            return null;
        }

        private Duck GetDuckById(int id) // once again getting the duck via Id clause in sql connection
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
                        return new Duck
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
        }
    }
}