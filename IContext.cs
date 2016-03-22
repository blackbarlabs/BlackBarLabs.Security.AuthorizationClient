using System;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public interface IContext
    {
        Task<TResult> ClaimsGetAsync<TResult>(Uri claimsLocation, Func<IClaim[], TResult> success, Func<HttpStatusCode, string, TResult> error);
    }
}
