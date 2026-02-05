namespace Application.Abstracts.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Uploads content to storage and returns the created object key.
    /// </summary>
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        int propertyAdId,
        CancellationToken ct = default);

    Task DeleteFileAsync(string objectKey, CancellationToken ct = default);
}
