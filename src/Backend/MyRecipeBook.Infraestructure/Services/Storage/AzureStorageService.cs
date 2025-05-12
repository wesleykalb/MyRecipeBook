using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;

namespace MyRecipeBook.Infraestructure.Services.Storage;

public class AzureStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }


    public async Task<string> GetImageUrl(User user, string fileName)
    {
        var containerName = user.UserIdentifier.ToString();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var exist = await container.ExistsAsync();

        if (!exist.Value) return string.Empty;

        var blobClient = container.GetBlobClient(fileName);
        exist = await blobClient.ExistsAsync();

        if (exist.Value)
        {
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(MyRecipeBookRuleConstants.MAX_IMAGE_LIFETIME_MINUTES)
            };
            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        return string.Empty;
    }

    public async Task Upload(User user, Stream file, string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(fileName);

        await blobClient.UploadAsync(file, overwrite: true);    
    }

    public async Task Delete(User user, string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        var exist = await container.ExistsAsync();

        if (exist.Value)
        {
            await container.DeleteBlobIfExistsAsync(fileName);
        }
    }

    public async Task DeleteContainer(Guid userIdentifier)
    {
        var container = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());
        
        await container.DeleteIfExistsAsync();
    }
}