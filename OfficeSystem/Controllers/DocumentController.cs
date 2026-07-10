using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSystem.DTOs.Approval;
using OfficeSystem.Services;
using System.Security.Claims;

namespace OfficeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateDocument([FromBody]CreateDocumentRequest request)
        {
            var drafterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _documentService.CreateDocument(request, drafterId);
            if (response == null)
                return BadRequest("문서 생성에 실패했습니다.");
            return Ok(response);
            
        }

    }
}
