using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Autodesk.Forge.Client;
using Coramba.Common;

namespace Coramba.DataAccess.Forge.Api
{
    public static class ForgeApiValidators
    {
        public static void StatusCodes<TResult>(ApiResponse<TResult> response, params HttpStatusCode[] statusCode)
        {
            var intStatusCodes = statusCode.Cast<int>().ToArray();
            if (!response.StatusCode.In(intStatusCodes))
                throw new Exception($"Status code is {response.StatusCode}, expected one of ({intStatusCodes.Flatten(", ")})");
        }

        public static void StatusCodes(HttpResponseMessage response, params HttpStatusCode[] statusCode)
        {
            var intStatusCodes = statusCode.Cast<int>().ToArray();
            if (!response.StatusCode.In(statusCode))
                throw new Exception($"Status code is {response.StatusCode}, expected one of ({intStatusCodes.Flatten(", ")})");
        }
    }
}