using DeliFHery.Api.Dtos;
using DeliFHery.Api.Mappers;
using DeliFHery.Api.Security;
using DeliFHery.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFHery.Api.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize(Policy = AuthorizationPolicies.ApiUser)]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliFHeryLogic _logic;

    public DeliveriesController(IDeliFHeryLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveries()
    {
        var deliveries = await _logic.GetDeliveriesAsync();
        return Ok(deliveries.Select(d => d.ToDto()));
    }

    [HttpGet("{deliveryId:guid}")]
    public async Task<ActionResult<DeliveryDto>> GetDelivery(Guid deliveryId)
    {
        var delivery = await _logic.GetDeliveryAsync(deliveryId);
        if (delivery is null)
        {
            return NotFound();
        }

        return Ok(delivery.ToDto());
    }

    [HttpGet("~/api/customers/{customerId:guid}/deliveries")]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesForCustomer(Guid customerId)
    {
        if (!await _logic.CustomerExistsAsync(customerId))
        {
            return NotFound();
        }

        var deliveries = await _logic.GetDeliveriesForCustomerAsync(customerId);
        return Ok(deliveries.Select(d => d.ToDto()));
    }

    [HttpPost("~/api/customers/{customerId:guid}/deliveries")]
    public async Task<ActionResult<DeliveryDto>> CreateDelivery(Guid customerId, [FromBody] DeliveryRequest request)
    {
        if (!await _logic.CustomerExistsAsync(customerId))
        {
            return NotFound();
        }

        var delivery = request.ToDomain(Guid.Empty, customerId);
        await _logic.AddDeliveryForCustomerAsync(customerId, delivery);
        return CreatedAtAction(nameof(GetDelivery), new { deliveryId = delivery.Id }, delivery.ToDto());
    }

    [HttpPut("{deliveryId:guid}")]
    public async Task<ActionResult<DeliveryDto>> UpdateDelivery(Guid deliveryId, [FromBody] DeliveryRequest request)
    {
        var existing = await _logic.GetDeliveryAsync(deliveryId);
        if (existing is null)
        {
            return NotFound();
        }

        var updated = request.ToDomain(deliveryId, existing.CustomerId);
        await _logic.UpdateDeliveryAsync(updated);
        return Ok(updated.ToDto());
    }

    [HttpDelete("{deliveryId:guid}")]
    public async Task<IActionResult> DeleteDelivery(Guid deliveryId)
    {
        var removed = await _logic.DeleteDeliveryAsync(deliveryId);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
