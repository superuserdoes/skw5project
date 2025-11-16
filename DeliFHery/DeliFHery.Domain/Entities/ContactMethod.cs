using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliFHery.Domain.Entities
{
    public class ContactMethod
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string Type { get; set; } = string.Empty; // "Email" or "Phone"

        public string Value { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }

}
