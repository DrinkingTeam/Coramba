using System;
using Microsoft.AspNetCore.Mvc;

namespace Coramba.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiBaseVersionAttribute : ApiVersionAttribute
    {
        public ApiBaseVersionAttribute(string version) : base(version)
        {
        }
    }
}
