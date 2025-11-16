using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliFHery.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string IdentitySubjectId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ContactMethod> ContactMethods { get; set; } = new();
}
