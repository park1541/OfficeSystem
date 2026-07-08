using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeSystem.Data;
using OfficeSystem.DTOs.Approval;
using OfficeSystem.Models.Approval;
using OfficeSystem.Models.Enums;

namespace OfficeSystem.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly OfficeDbContext _context;
        public DocumentService(OfficeDbContext context)
        {
            _context = context;
        }
        public async Task<DocumentResponse> CreateDocument(CreateDocumentRequest request, int drafterId)
        {
            if (request.ApproverIds.Distinct().Count() != request.ApproverIds.Count)
                return null;
            if (!await _context.DocumentTypes.AnyAsync(u => u.Id == request.DocumentTypeId))
                return null;
                var existingApproverCount = await _context.Users.CountAsync(u => request.ApproverIds.Contains(u.Id));
            if (existingApproverCount != request.ApproverIds.Count)
                return null;

            var document = new Document
            {
                Title = request.Title,              // 클라이언트가 보낸 것
                Content = request.Content,          //   "
                DocumentTypeId = request.DocumentTypeId, // "
                DrafterId = drafterId,              // 토큰에서 온 값 (매개변수)
                Status = DocumentStatus.Pending,    // 서버가 강제
                CreatedAt = DateTime.UtcNow,         // 서버가 기록
                ApprovalLines = request.ApproverIds
                .Select((approverId, index) => new ApprovalLine
                {
                    ApproverId = approverId,
                    Sequence = index + 1,
                    Status = ApprovalStatus.Pending
                })
                .ToList()
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            var saved = await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.ApprovalLines)
                    .ThenInclude(l => l.Approver)
                .FirstAsync(d => d.Id == document.Id);
            return new DocumentResponse
            {
                Id = saved.Id,
                Title = saved.Title,
                Content = saved.Content,
                DocumentType = saved.DocumentType.Name,          // 이름! saved에서 어떻게 꺼내죠?
                Status = saved.Status.ToString(),                // enum → string, 오늘 정한 방법
                CreatedAt = saved.CreatedAt,
                ApprovalLines = saved.ApprovalLines
        .Select(l => new ApprovalLineResponse
        {
            Sequence = l.Sequence,
            ApproverName = l.Approver.Name,  // ThenInclude가 실어온 것
            Status = l.Status.ToString(),
            Comment = l.Comment
        })
        .ToList()
            };
        }
    }
}
