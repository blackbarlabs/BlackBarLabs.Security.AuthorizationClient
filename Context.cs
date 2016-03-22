using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public class Context : IContext
    {
        public Task<TResult> ClaimsGetAsync<TResult>(Uri claimsLocation, Func<IClaim[], TResult> success, Func<HttpStatusCode, string, TResult> error)
        {
            return Claims.FetchClaimsAsync(claimsLocation, success, (code, message) => error(code, message));
        }
    }
}
