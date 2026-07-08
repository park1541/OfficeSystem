using OfficeSystem.Models.Enums;

namespace OfficeSystem.DTOs.Approval
{
    public class ApprovalLineResponse
    {
        public int Sequence {  get; set; }
        public string ApproverName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Comment { get; set; }

    }
}
