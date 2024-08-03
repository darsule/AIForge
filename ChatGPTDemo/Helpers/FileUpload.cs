using Microsoft.AspNetCore.Components.Forms;

namespace OpenAiAPIDemo.Helpers
{
    public class FileUpload
    {
        private long maxFileSize = 1024 * 1024 * 1; // represents 3MB
        private int maxAllowedFiles = 3;
        private List<string> errors = new();

        public async Task LoadFiles(InputFileChangeEventArgs e)
        {
            errors.Clear();

            if (e.FileCount > maxAllowedFiles)
            {
                errors.Add($"Error: Attempting to upload {e.FileCount} files, but only {maxAllowedFiles} files are allowed");
                return;
            }

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    string userName = Environment.UserName.Split(".").First();
                    string targetPath = Path.Combine(tempPath, userName);
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    string targetFileFullPath = Path.Combine(targetPath, file.Name);
                    await using FileStream fs = new(targetFileFullPath, FileMode.Create);
                    await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                }
                catch (Exception ex)
                {
                    errors.Add($"File: {file.Name} Error: {ex.Message}");
                }
            }
        }
    }
}
