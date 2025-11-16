using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliFHery.Persistence.Entities;

[Table("customers", Schema = "delifhery")]
public class CustomerEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string PostalCode { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string? Notes { get; set; }

    public List<DeliveryOrderEntity> Deliveries { get; set; } = [];

    public List<ContactEntity> Contacts { get; set; } = [];
}
