using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Seller
    {
        public Guid Id { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key على جدول User
        public User User { get; set; } // Navigation property

        public string StripeAccountId { get; set; } = string.Empty; // ID من Stripe
    }
}
