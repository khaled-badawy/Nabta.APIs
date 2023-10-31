using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Nabta_Production.BL
{
    public class FileManager : IFileManager
    {
        private readonly IConfiguration _configuration;
        private string urlUpload { get; }

        public FileManager(IConfiguration configuration)
        {
            _configuration = configuration;
            urlUpload = $"{_configuration.GetSection("ServerUploadPath").Value!}";
        }
        public async Task<string?> UploadFile(IFormFile? _iFormFile, int Id, string newName, string entityType)
        {
            if (_iFormFile is null) return null;
            try
            {
                string newFileName = RenameFile(_iFormFile,newName)!;
                var filePath = GetFilePath(newFileName,entityType,Id);
                //using (StreamWriter writer = System.IO.File.AppendText($"\\\\WEB-IDSC\\Tawasol1\\NabtaAPI\\wwwroot\\upload\\errors.txt"))
                //{
                //    writer.WriteLine("fileName + filePath try part \n" + $"{fileName + filePath}");
                //}
                using (var fileStream = new FileStream(filePath,FileMode.Create))
                {
                    await _iFormFile.CopyToAsync(fileStream);
                }

                return newFileName;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
                {
                    writer.WriteLine("error copy attachment catch part \n" + $"{ex.Message}");
                }

                return String.Empty ;
                //var message = ex.Message;
                //return message;
            }
        }
        private string GetStaticContent(string entityType , int id)
        {
            string result = "";

            switch (entityType)
            {
                case "user":
                    result = Path.Combine($"{urlUpload}\\users\\attachments\\{id}");
                    break;

                case "post":
                    result = Path.Combine($"{urlUpload}\\posts\\attachments\\{id}");
                    break;

                case "competition":
                    result = Path.Combine($"{urlUpload}\\competitions\\attachments\\{id}");
                    break;
            }
            try
            {
                if (!Directory.Exists(result))
                {
                    try
                    {
                        Directory.CreateDirectory(result);
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
                        {
                            writer.WriteLine("error create directory \n" + $"{ex.Message}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
                {
                    writer.WriteLine("error existing directory \n" + $"{ex.Message}");
                }
            }
            return result;
        }
        private string GetFilePath(string FileName ,string entityType, int id)
        {
            return Path.Combine(GetStaticContent(entityType,id), FileName);
        }
        public string? RenameFile(IFormFile? _iFormFile , string newName)
        {
            if (_iFormFile is null) return null;
            FileInfo fileInfo = new FileInfo(_iFormFile.FileName);
            return $"{newName}{fileInfo.Extension}";
        }
    }
}
