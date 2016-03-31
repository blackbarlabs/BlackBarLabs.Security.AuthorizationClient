using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public class MockContext : IContext
    {
        List<IClaim> Claims = new List<IClaim>();

        public Task<TResult> ClaimsGetAsync<TResult>(Uri claimsLocation, Func<IClaim[], TResult> success, Func<HttpStatusCode, string, TResult> error)
        {
            return Task.FromResult(success(this.Claims.ToArray()));
        }

        public void AddClaim(Uri exampleClaimType, string exampleClaimValue)
        {
            Claims.Add(new Claims.Claim()
            {
                Type = exampleClaimType,
                Value = exampleClaimValue,
            });
        }

        public Task<TResult> ClaimGetAsync<TResult>(Guid authorizationId, Uri type, Func<Guid, Uri, string, TResult> success, Func<TResult> notFound, Func<HttpStatusCode, string, TResult> webFailure, Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ClaimPutAsync<TResult>(Guid accountId, Uri distributorAdminRole, string value, Func<TResult> success, Func<TResult> notFound, Func<HttpStatusCode, string, TResult> httpError, Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ClaimPostAsync<TResult>(Guid accountId, Uri distributorAdminRole, string value, Func<TResult> success, Func<HttpStatusCode, string, TResult> httpError, Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }
    }
}
