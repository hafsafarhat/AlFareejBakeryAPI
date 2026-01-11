using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlFareejBakeryAPI.Models;

namespace AlFareejBakeryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AlFareejBakerySweetsContext _context;

        public CustomersController(AlFareejBakerySweetsContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for duplicate email
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
            {
                return BadRequest(new { message = "A customer with this email already exists" });
            }

            // Set default values
            customer.JoinDate = customer.JoinDate ?? DateOnly.FromDateTime(DateTime.Now);
            customer.TotalSpending = customer.TotalSpending ?? 0;
            customer.Frequency = customer.Frequency ?? 0;
            customer.Churned = customer.Churned ?? false;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }

            // Check if email is being changed and if it's already taken
            if (existingCustomer.Email != customer.Email)
            {
                if (await _context.Customers.AnyAsync(c => c.Email == customer.Email && c.CustomerId != id))
                {
                    return BadRequest(new { message = "A customer with this email already exists" });
                }
            }

            // Update all properties
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Age = customer.Age;
            existingCustomer.Gender = customer.Gender;
            existingCustomer.PostalCode = customer.PostalCode;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.MembershipStatus = customer.MembershipStatus;
            existingCustomer.JoinDate = customer.JoinDate;
            existingCustomer.LastPurchaseDate = customer.LastPurchaseDate;
            existingCustomer.TotalSpending = customer.TotalSpending;
            existingCustomer.AverageOrderValue = customer.AverageOrderValue;
            existingCustomer.Frequency = customer.Frequency;
            existingCustomer.PreferredCategory = customer.PreferredCategory;
            existingCustomer.Churned = customer.Churned;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound(new { message = $"Customer with ID {id} not found" });
                }
                throw;
            }

            return Ok(existingCustomer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}