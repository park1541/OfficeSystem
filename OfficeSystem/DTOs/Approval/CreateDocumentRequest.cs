using System.ComponentModel.DataAnnotations;

namespace OfficeSystem.DTOs.Approval
{
    public class CreateDocumentRequest
    {
        [Required(ErrorMessage = "제목을 입력해주세요.")]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required(ErrorMessage = "내용을 입력해주세요.")]
        public string Content { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "문서 종류를 선택해주세요.")]
        public int DocumentTypeId { get; set; }
        [Required(ErrorMessage = "결제자가 누구인지 알려주세요.")]
        [MinLength(1)]
        public List<int> ApproverIds { get; set; } = [];
    }
}
