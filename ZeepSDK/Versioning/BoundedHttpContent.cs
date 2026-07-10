using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeepSDK.Versioning;

internal static class BoundedHttpContent
{
    public static async Task<string> ReadAsUtf8StringAsync(
        HttpContent content,
        int maximumBytes,
        CancellationToken cancellationToken = default)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        if (maximumBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumBytes));
        if (content.Headers.ContentLength > maximumBytes)
            throw new InvalidDataException($"HTTP response exceeds the {maximumBytes}-byte limit.");

        using Stream source = await content.ReadAsStreamAsync();
        using MemoryStream destination = new();
        byte[] buffer = new byte[16 * 1024];
        int totalBytes = 0;

        while (true)
        {
            int bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            if (bytesRead == 0)
                break;

            totalBytes += bytesRead;
            if (totalBytes > maximumBytes)
                throw new InvalidDataException($"HTTP response exceeds the {maximumBytes}-byte limit.");

            destination.Write(buffer, 0, bytesRead);
        }

        return Encoding.UTF8.GetString(destination.ToArray());
    }
}
