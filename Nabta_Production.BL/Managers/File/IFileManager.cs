using Microsoft.AspNetCore.Http;

namespace Nabta_Production.BL
{
    public interface IFileManager
    {
        Task<string?> UploadFile(IFormFile? _iFormFile, int Id, string newName , string entityType);
        string? RenameFile(IFormFile? _iFormFile, string newName);
    }
}
