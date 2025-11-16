using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeliFHery.Domain;

namespace DeliFHery.Persistence.Entities;

[Table("contacts", Schema = "delifhery")]
public class ContactEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public CustomerEntity? Customer { get; set; }

    public ContactType Type { get; set; }

    [Required]
    [MaxLength(256)]
    public string Value { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
}
