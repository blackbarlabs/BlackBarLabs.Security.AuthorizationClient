using System;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public interface IContext
    {
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

        Task<TResult> CreateAuthorizationAsync<TResult>(Guid accountId,
            Func<TResult> onSuccess,
            Func<string, TResult> onFailure);

        Task<TResult> CreateSessionsWithImplicitAsync<TResult>(string username, string password,
            Func<string, string, TResult> success,
            Func<string, TResult> failed);

        Task<TResult> CreateCredentialVoucherAsync<TResult>(Guid accountId, TimeSpan timeSpan,
            Func<string, TResult> success,
            Func<string, TResult> failure);

        Task<TResult> CreateCredentialImplicitAsync<TResult>(Guid accountId, string username, string password,
            Func<TResult> success,
            Func<string, TResult> failure);
        
        Task<TResult> AuthorizationDeleteAsync<TResult>(Guid id,
            Func<TResult> success,
            Func<HttpStatusCode, string, TResult> webFailure,
            Func<TResult> failure);
    }
}
