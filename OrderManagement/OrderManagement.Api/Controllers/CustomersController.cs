using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Dtos;
using OrderManagement.Api.Mappers;
using OrderManagement.Domain;
using OrderManagement.Logic;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(IOrderManagementLogic logic) : ControllerBase
{
    private IOrderManagementLogic Logic { get; } = logic ?? throw new ArgumentNullException(nameof(logic));

    [HttpGet]
    public async Task<IEnumerable<CustomerDto>> GetCustomers([FromQuery] Rating? rating)
    {
        if(rating  is null)
            return (await Logic.GetCustomersAsync()).Select(x => x.ToCustomerDto());
        return (await logic.GetCustomersByRatingAsync(rating.Value)).Select(x => x.ToCustomerDto());
       
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById([FromRoute] Guid id)
    {
        var customer = await Logic.GetCustomerAsync(id);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }
}