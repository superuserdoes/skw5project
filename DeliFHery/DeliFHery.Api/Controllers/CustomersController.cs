using DeliFHery.Api.Dtos;
using DeliFHery.Api.Mappers;
using DeliFHery.Api.Security;
using DeliFHery.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFHery.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Policy = AuthorizationPolicies.ApiUser)]
public class CustomersController : ControllerBase
{
    private readonly IDeliFHeryLogic _logic;

    public CustomersController(IDeliFHeryLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _logic.GetCustomersAsync();
        return Ok(customers.Select(c => c.ToDto()));
    }

    [HttpGet("{customerId:guid}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(Guid customerId)
    {
        var customer = await _logic.GetCustomerAsync(customerId);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerRequest request)
    {
        var customer = request.ToDomain(Guid.Empty);
        await _logic.AddCustomerAsync(customer);
        return CreatedAtAction(nameof(GetCustomerById), new { customerId = customer.Id }, customer.ToDto());
    }

    [HttpPut("{customerId:guid}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid customerId, [FromBody] CustomerRequest request)
    {
        var existingCustomer = await _logic.GetCustomerAsync(customerId);
        if (existingCustomer is null)
        {
            return NotFound();
        }

        var updatedCustomer = request.ToDomain(customerId);
        await _logic.UpdateCustomerAsync(updatedCustomer);
        return Ok(updatedCustomer.ToDto());
    }

    [HttpDelete("{customerId:guid}")]
    public async Task<IActionResult> DeleteCustomer(Guid customerId)
    {
        var removed = await _logic.DeleteCustomerAsync(customerId);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
