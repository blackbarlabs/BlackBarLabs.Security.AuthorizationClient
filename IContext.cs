using System;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public interface IContext
    {
        // Task<TResult> ClaimsGetAsync<TResult>(Uri claimsLocation, Func<IClaim[], TResult> success, Func<HttpStatusCode, string, TResult> error);

        Task<TResult> ClaimGetAsync<TResult>(Guid authorizationId, Uri type,
            Func<Guid, Uri, string, TResult> success,
            Func<TResult> notFound, 
            Func<HttpStatusCode, string, TResult> webFailure,
            Func<string, TResult> failure);

        Task<TResult> ClaimPutAsync<TResult>(Guid accountId, Uri distributorAdminRole, string value,
            Func<TResult> success,
            Func<TResult> notFound,
            Func<HttpStatusCode, string, TResult> httpError,
            Func<string, TResult> failure);

        Task<TResult> ClaimPostAsync<TResult>(Guid accountId, Uri distributorAdminRole, string value,
            Func<TResult> success,
            Func<HttpStatusCode, string, TResult> httpError,
            Func<string, TResult> failure);
    }
}
