using OfficeSystem.Models.Enums;
using OfficeSystem.Models.Users;

namespace OfficeSystem.Models.Approval
{
    public class ApprovalLine
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public int ApproverId { get; set; }         // 결재자 (User FK)
        public User Approver { get; set; } = null!;

        public int Sequence { get; set; }           // 결재 순서
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public string? Comment { get; set; }
    }
}
