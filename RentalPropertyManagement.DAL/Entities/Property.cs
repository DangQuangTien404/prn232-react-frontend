using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace RentalPropertyManagement.DAL.Entities
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public int SquareFootage { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MonthlyRent { get; set; }

        public string Description { get; set; }
        public bool IsOccupied { get; set; }

        // Navigation properties
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
    }
}