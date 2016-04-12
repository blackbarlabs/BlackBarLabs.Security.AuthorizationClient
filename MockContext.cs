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
        Dictionary<string, Tuple<Guid, Uri, string>> claims = new Dictionary<string, Tuple<Guid, Uri, string>>();

        private static string GetClaimKey(Guid authorizationId, Uri type)
        {
            var key = authorizationId.ToString("N") + type.AbsoluteUri;
            return key;
        }

        public async Task<TResult> ClaimGetAsync<TResult>(Guid authorizationId, Uri type,
            Func<Guid, Uri, string, TResult> success,
            Func<TResult> notFound,
            Func<HttpStatusCode, string, TResult> webFailure,
            Func<string, TResult> failure)
        {
            await Task.FromResult(true);
            var key = GetClaimKey(authorizationId, type);
            if (claims.ContainsKey(key))
            {
                var claim = claims[key];
                return success(claim.Item1, claim.Item2, claim.Item3);
            }
            return notFound();
        }

        public async Task<TResult> ClaimPutAsync<TResult>(Guid authorizationId, Uri type, string value,
            Func<TResult> success,
            Func<TResult> notFound, 
            Func<HttpStatusCode, string, TResult> httpError,
            Func<string, TResult> failure)
        {
            await Task.FromResult(true);
            var key = GetClaimKey(authorizationId, type);
            if (!claims.ContainsKey(key))
                return notFound();

            claims[key] = Tuple.Create(authorizationId, type, value);
            return success();
        }

        public async Task<TResult> ClaimPostAsync<TResult>(Guid authorizationId, Uri type, string value, 
            Func<TResult> success,
            Func<HttpStatusCode, string, TResult> httpError,
            Func<string, TResult> failure)
        {
            await Task.FromResult(true);
            var key = GetClaimKey(authorizationId, type);
            if (claims.ContainsKey(key))
                return httpError(HttpStatusCode.Conflict, "Already Exists");

            claims[key] = Tuple.Create(authorizationId, type, value);
            return success();
        }

        public Task<TResult> CreateAuthorizationAsync<TResult>(Guid accountId, 
            Func<TResult> onSuccess, 
            Func<string, TResult> onFailure)
        {
            return Task.FromResult(onSuccess());
        }

        public async Task<TResult> CreateSessionsWithImplicitAsync<TResult>(string username, string password,
            Func<string, string, TResult> success,
            Func<string, TResult> failed)
        {
            await Task.FromResult(true);
            var claims = new System.Security.Claims.Claim[] { };
            var jwtToken = BlackBarLabs.Security.Tokens.JwtTools.CreateToken(Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow, DateTimeOffset.UtcNow + TimeSpan.FromDays(1.0),
                claims);
            return success("Authorization", jwtToken);
        }

        public Task<TResult> CreateCredentialVoucherAsync<TResult>(Guid accountId, TimeSpan timeSpan,
            Func<string, TResult> success,
            Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> CreateCredentialImplicitAsync<TResult>(Guid accountId, string username, string password, Func<TResult> success, Func<string, TResult> failure)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> AuthorizationDeleteAsync<TResult>(Guid id, Func<TResult> success, Func<HttpStatusCode, string, TResult> webFailure, Func<TResult> failure)
        {
            throw new NotImplementedException();
        }
    }
}
