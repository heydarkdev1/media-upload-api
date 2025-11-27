using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace MediaUploadApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly BlobContainerClient _containerClient;

        public UploadController(IConfiguration config)
        {
            // 1. Get Key Vault URL from appsettings or Azure App Settings
            var keyVaultUrl = config["KeyVaultUrl"];

            // 2. Connect to Key Vault using Managed Identity
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            // 3. Read the storage connection string secret
            var secret = secretClient.GetSecret("StorageConnectionString");
            var storageConnection = secret.Value.Value;

            // 4. Create Blob container client
            var blobServiceClient = new BlobServiceClient(storageConnection);
            _containerClient = blobServiceClient.GetBlobContainerClient("images");
        }

        // ---------------------------------------------------------
        // SIMPLE TEST ENDPOINT — CONFIRM API IS RUNNING
        // ---------------------------------------------------------
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Upload API is running");
        }

        // ---------------------------------------------------------
        // FILE UPLOAD ENDPOINT (FORM-DATA)
        // ---------------------------------------------------------
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                var blob = _containerClient.GetBlobClient(file.FileName);

                using var stream = file.OpenReadStream();
                await blob.UploadAsync(stream, overwrite: true);

                return Ok("File uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }
    }
}
