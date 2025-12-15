using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalPropertyManagement.DAL.Entities
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }

        // Foreign Keys
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public int? AssignedProviderId { get; set; } // Nullable Foreign Key

        public string Description { get; set; }
        public Enums.RequestPriority Priority { get; set; }
        public Enums.RequestStatus Status { get; set; }
        public DateTime SubmittedDate { get; set; }

        public string AttachmentUrl { get; set; }

        // Navigation properties
        public virtual User Tenant { get; set; }
        public virtual Property Property { get; set; }

        [ForeignKey("AssignedProviderId")]
        public virtual User AssignedProvider { get; set; }
    }
}