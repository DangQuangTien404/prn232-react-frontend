using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalPropertyManagement.DAL.Entities
{
    public class Contract
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PropertyId { get; set; }
        public int TenantId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal RentAmount { get; set; }

        public Enums.ContractStatus Status { get; set; }

        // Navigation properties
        public virtual Property Property { get; set; }
        public virtual User Tenant { get; set; }
    }
}