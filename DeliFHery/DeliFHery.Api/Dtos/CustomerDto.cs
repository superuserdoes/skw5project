using System.ComponentModel.DataAnnotations;

namespace DeliFHery.Api.Dtos;

public record CustomerDto(
    Guid Id,
    string Name,
    string Street,
    string City,
    string PostalCode,
    string? Notes);

public class CustomerRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}
