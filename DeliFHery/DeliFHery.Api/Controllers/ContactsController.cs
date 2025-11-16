using DeliFHery.Api.Dtos;
using DeliFHery.Api.Mappers;
using DeliFHery.Api.Security;
using DeliFHery.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFHery.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize(Policy = AuthorizationPolicies.ApiUser)]
public class ContactsController : ControllerBase
{
    private readonly IDeliFHeryLogic _logic;

    public ContactsController(IDeliFHeryLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
    }

    [HttpGet("~/api/customers/{customerId:guid}/contacts")]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContactsForCustomer(Guid customerId)
    {
        if (!await _logic.CustomerExistsAsync(customerId))
        {
            return NotFound();
        }

        var contacts = await _logic.GetContactsForCustomerAsync(customerId);
        return Ok(contacts.Select(c => c.ToDto()));
    }

    [HttpGet("{contactId:guid}")]
    public async Task<ActionResult<ContactDto>> GetContact(Guid contactId)
    {
        var contact = await _logic.GetContactAsync(contactId);
        if (contact is null)
        {
            return NotFound();
        }

        return Ok(contact.ToDto());
    }

    [HttpPost("~/api/customers/{customerId:guid}/contacts")]
    public async Task<ActionResult<ContactDto>> CreateContact(Guid customerId, [FromBody] ContactRequest request)
    {
        if (!await _logic.CustomerExistsAsync(customerId))
        {
            return NotFound();
        }

        var contact = request.ToDomain(Guid.Empty, customerId);
        await _logic.AddContactForCustomerAsync(customerId, contact);
        return CreatedAtAction(nameof(GetContact), new { contactId = contact.Id }, contact.ToDto());
    }

    [HttpDelete("{contactId:guid}")]
    public async Task<IActionResult> DeleteContact(Guid contactId)
    {
        var removed = await _logic.DeleteContactAsync(contactId);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
