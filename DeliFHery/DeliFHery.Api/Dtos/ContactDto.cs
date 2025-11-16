using System.ComponentModel.DataAnnotations;
using DeliFHery.Domain;

namespace DeliFHery.Api.Dtos;

public record ContactDto(
    Guid Id,
    Guid CustomerId,
    ContactType Type,
    string Value,
    bool IsPrimary);

public class ContactRequest
{
    [Required]
    public ContactType Type { get; set; }
        = ContactType.Email;

    [Required]
    [StringLength(300)]
    public string Value { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
        = false;
}
