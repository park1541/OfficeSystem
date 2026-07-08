using OfficeSystem.Models.Enums;
using OfficeSystem.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace OfficeSystem.Models.Approval
{
    public class Document
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; } = null!;

        public int DrafterId { get; set; }
        public User Drafter { get; set; } = null!;
        [MaxLength(50)]
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ApprovalLine> ApprovalLines { get; set; } = new List<ApprovalLine>();

      
    }
}
