using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.REST.Models;
using PetStore.Common.Services;
using PetStore.Infrastructure.Models;

namespace PetStore.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICrudServiceAsync<CustomerModel> _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICrudServiceAsync<CustomerModel> customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // GET: api/customers
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers([FromQuery] int? page, [FromQuery] int? amount)
        {
            try
            {
                IEnumerable<CustomerModel> customers;

                if (page.HasValue && amount.HasValue)
                {
                    customers = await _customerService.ReadAllAsync(page.Value, amount.Value);
                }
                else
                {
                    customers = await _customerService.ReadAllAsync();
                }

                var customerDtos = customers.Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                    PetIds = c.PetIds
                });

                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.ReadAsync(id);

                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                var customerDto = new CustomerDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Age = customer.Age,
                    PetIds = customer.PetIds
                };

                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerCreateDto customerCreateDto)
        {
            try
            {
                var customerModel = new CustomerModel(
                    customerCreateDto.Name,
                    customerCreateDto.Age
                );

                var created = await _customerService.CreateAsync(customerModel);

                if (!created)
                {
                    return BadRequest("Failed to create customer");
                }

                var customerDto = new CustomerDto
                {
                    Id = customerModel.Id,
                    Name = customerModel.Name,
                    Age = customerModel.Age,
                    PetIds = customerModel.PetIds
                };

                return CreatedAtAction(nameof(GetCustomerById), new { id = customerDto.Id }, customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerUpdateDto customerUpdateDto)
        {
            try
            {
                if (id != customerUpdateDto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingCustomer = await _customerService.ReadAsync(id);

                if (existingCustomer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                existingCustomer.Name = customerUpdateDto.Name;
                existingCustomer.Age = customerUpdateDto.Age;

                var updated = await _customerService.UpdateAsync(existingCustomer);

                if (!updated)
                {
                    return BadRequest("Failed to update customer");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID {CustomerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await _customerService.ReadAsync(id);

                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                var deleted = await _customerService.RemoveAsync(customer);

                if (!deleted)
                {
                    return BadRequest("Failed to delete customer");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID {CustomerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

