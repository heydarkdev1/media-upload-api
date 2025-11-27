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
            // Key Vault URL from app settings
            var keyVaultUrl = config["KeyVaultUrl"];

            // Authenticate using Managed Identity
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            // Read storage connection string
            var secret = secretClient.GetSecret("StorageConnectionString");
            var storageConnection = secret.Value.Value;

            // Create Blob client
            var blobServiceClient = new BlobServiceClient(storageConnection);
            _containerClient = blobServiceClient.GetBlobContainerClient("images");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var blob = _containerClient.GetBlobClient(file.FileName);

            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, overwrite: true);

            return Ok("File uploaded successfully");
        }
    }
}
