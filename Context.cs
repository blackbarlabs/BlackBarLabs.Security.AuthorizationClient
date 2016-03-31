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
        public Task<TResult> ClaimGetAsync<TResult>(Guid authorizationId, Uri type,
            Func<Guid, Uri, string, TResult> success, 
            Func<TResult> notFound,
            Func<HttpStatusCode, string, TResult> webFailure, 
            Func<string, TResult> failure)
        {
            return Claims.GetAsync(authorizationId, type,
                success,
                notFound,
                webFailure,
                failure);
        }

        public Task<TResult> ClaimPostAsync<TResult>(Guid authorizationId, Uri type, string value,
            Func<TResult> success,
            Func<HttpStatusCode, string, TResult> webFailure,
            Func<string, TResult> failure)
        {
            return Claims.PostAsync(authorizationId, type, value,
                success,
                webFailure,
                failure);
        }

        public Task<TResult> ClaimPutAsync<TResult>(Guid accountId, Uri distributorAdminRole, string value, Func<TResult> success, Func<TResult> notFound, Func<HttpStatusCode, string, TResult> httpError, Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }
    }
}
