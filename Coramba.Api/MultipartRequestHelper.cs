using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Coramba.Api
{
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }

        public static async Task<bool> UploadFilesAsync(
            HttpRequest request,
            ModelStateDictionary modelState,
            FormOptions formsOptions,
            ILogger logger,
            Func<Stream, string, Task<bool>> fileAction
            )
        {
            if (!IsMultipartContentType(request.ContentType))
            {
                modelState.AddModelError("File", $"The request couldn't be processed (Error 1).");

                logger.LogError($"Wrong content-type: {request.ContentType}");

                return false;
            }

            var boundary = GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType), formsOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader && HasFileContentDisposition(contentDisposition))
                {
                    if (!await fileAction(section.Body, contentDisposition.FileName.Value))
                        return true;
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return true;
        }

        public static async Task<bool> UploadFileAsync(
            HttpRequest request,
            ModelStateDictionary modelState,
            FormOptions formsOptions,
            ILogger logger,
            Func<Stream, string, Task> fileAction
        )
        {
            var has = false;
            if (!await UploadFilesAsync(request, modelState, formsOptions, logger, async (fs, fn) =>
            {
                await fileAction(fs, fn);
                has = true;
                return false;
            }))
                return false;
            if (!has)
            {
                modelState.AddModelError("File", "File is required");
                return false;
            }

            return true;
        }
    }
}
