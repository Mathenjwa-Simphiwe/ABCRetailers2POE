using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace ABCFunctions.Helpers
{
    public static class MultipartHelper
    {
        public static async Task<Dictionary<string, string>> ParseMultipartFormDataAsync(Stream body, string boundary)
        {
            var formData = new Dictionary<string, string>();
            var reader = new MultipartReader(boundary, body);

            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                {
                    if (contentDisposition.IsFormDisposition())
                    {
                        var key = contentDisposition.Name.Value;
                        using var streamReader = new StreamReader(section.Body);
                        var value = await streamReader.ReadToEndAsync();
                        formData[key] = value;
                    }
                }
            }

            return formData;
        }

        public static async Task<Stream> GetFileStreamAsync(Stream body, string boundary, string fieldName)
        {
            var reader = new MultipartReader(boundary, body);

            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                {
                    if (contentDisposition.IsFileDisposition() && contentDisposition.Name.Value == fieldName)
                    {
                        return section.Body;
                    }
                }
            }

            return null;
        }
    }
}
