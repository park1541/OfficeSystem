using OfficeSystem.DTOs.Approval;

namespace OfficeSystem.Services
{
    public interface IDocumentService
    {
        Task<DocumentResponse> CreateDocument(CreateDocumentRequest request, int drafterId);
    }
}
