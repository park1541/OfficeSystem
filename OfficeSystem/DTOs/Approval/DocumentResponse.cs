namespace OfficeSystem.DTOs.Approval
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ApprovalLineResponse> ApprovalLines { get; set; } = [];
    }
}
