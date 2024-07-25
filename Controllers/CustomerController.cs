using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CustomerController:Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)

        {

            _context = context;

        }
        //listing customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.GetCustomersAsync();
            return View(customers);
        }

        //new customer form

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _context.InsertCustomerAsync(customer.FirstName, customer.LastName, customer.Email, customer.Phone);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        //edit customer
        public async Task<IActionResult> Edit(int id)
        {
            var customer = (await _context.GetCustomersAsync()).FirstOrDefault(c => c.CustomerId == id);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string firstName, string lastName, string email, string phone)
        {
            await _context.UpdateCustomerAsync(id, firstName, lastName, email, phone);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _context.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }



    }
}
