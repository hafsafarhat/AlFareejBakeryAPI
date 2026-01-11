using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlFareejBakeryAPI.Models;

namespace AlFareejBakeryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AlFareejBakerySweetsContext _context;

        public OrdersController(AlFareejBakerySweetsContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.TransactionId == id);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == order.CustomerId);
            if (!customerExists)
            {
                return BadRequest(new { message = $"Customer with ID {order.CustomerId} does not exist" });
            }

            // Validate product exists
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null)
            {
                return BadRequest(new { message = $"Product with ID {order.ProductId} does not exist" });
            }

            // Set default values
            order.OrderDate = order.OrderDate ?? DateOnly.FromDateTime(DateTime.Now);
            order.OrderTime = order.OrderTime ?? TimeOnly.FromDateTime(DateTime.Now);
            order.Status = order.Status ?? "Pending";
            order.DiscountAmount = order.DiscountAmount ?? 0;

            // Calculate price based on product price and quantity if not provided
            if (order.Price == 0)
            {
                order.Price = product.Price * order.Quantity;
            }

            // Don't set navigation properties - they're not needed for insert
            order.Customer = null;
            order.Product = null;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload order with navigation properties for response
            var createdOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.TransactionId == order.TransactionId);

            return CreatedAtAction(nameof(GetOrder), new { id = order.TransactionId }, createdOrder);
        }

        // PUT: api/orders/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            if (order.Status == "Completed")
            {
                return BadRequest(new { message = "Order is already completed" });
            }

            if (order.Status == "Cancelled")
            {
                return BadRequest(new { message = "Cannot complete a cancelled order" });
            }

            order.Status = "Completed";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Order {id} has been marked as completed",
                order
            });
        }

        // PUT: api/orders/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.TransactionId == id);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            if (order.Status == "Cancelled")
            {
                return BadRequest(new { message = "Order is already cancelled" });
            }

            if (order.Status == "Completed")
            {
                return BadRequest(new { message = "Cannot cancel a completed order" });
            }

            order.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Order {id} has been cancelled",
                order
            });
        }
    }
}