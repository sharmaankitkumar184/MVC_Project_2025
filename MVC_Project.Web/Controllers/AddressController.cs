using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Repositories.IRepository;
using System.Net;

namespace MVC_Project.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IAddressRepository _addrepo;
        public AddressController(ILogger<EmployeesController> logger, IAddressRepository addrepo)
        {
            _logger = logger;
            _addrepo = addrepo;
        }

        // POST: Address/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Address newAddress)
        {

            if (ModelState.IsValid)
            {
                await _addrepo.AddAddress(newAddress);

                // Respond with the new address ID and text (or whatever data you need for the dropdown)
                return Json(new
                {
                    success = true,
                    addressId = newAddress.Id,
                    addressText = $"{newAddress.Street}, {newAddress.City}, {newAddress.State}, {newAddress.ZipCode}"
                });
            }

            // If the model state is invalid, return error
            return Json(new { success = false });
        }
        }
}
