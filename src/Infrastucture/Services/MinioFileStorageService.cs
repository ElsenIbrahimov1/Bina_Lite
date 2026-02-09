using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastucture.Services;

public sealed class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _client;
    private readonly MinioOptions _opt;

    public MinioFileStorageService(IMinioClient client, IOptions<MinioOptions> options)
    {
        _client = client;
        _opt = options.Value;
    }

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        int propertyAdId,
        CancellationToken ct = default)
    {
        // Ensure bucket exists
        var bucketExists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_opt.Bucket),
            ct);

        if (!bucketExists)
        {
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_opt.Bucket),
                ct);
        }

        // Build object key
        var ext = Path.GetExtension(fileName);
        var objectKey = $"{propertyAdId}/{Guid.NewGuid():N}{ext}";

        // Ensure we know the size
        if (!content.CanSeek)
            throw new InvalidOperationException("Upload stream must be seekable.");

        content.Position = 0;

        await _client.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_opt.Bucket)
                .WithObject(objectKey)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType),
            ct);

        return objectKey;
    }

    public async Task DeleteFileAsync(string objectKey, CancellationToken ct = default)
    {
        await _client.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_opt.Bucket)
                .WithObject(objectKey),
            ct);
    }
}