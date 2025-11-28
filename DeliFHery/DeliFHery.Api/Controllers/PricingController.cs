using DeliFHery.Api.Dtos;
using DeliFHery.Api.Mappers;
using DeliFHery.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFHery.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly IDeliFHeryLogic _logic;

    public PricingController(IDeliFHeryLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<PriceQuoteDto>> CalculatePrice([FromBody] DeliveryRequest request)
    {
        var deliveryOrder = request.ToDomain(Guid.Empty, Guid.Empty);
        var breakdown = await _logic.CalculateDeliveryPriceAsync(deliveryOrder);
        return Ok(breakdown.ToDto());
    }
}
