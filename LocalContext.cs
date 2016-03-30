using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackBarLabs.Security.Authorization;
using System.Net;
using System.Net.Http;
using static BlackBarLabs.Security.Authorization.Claims;
using System.Collections.Specialized;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public class LocalContext : IContext
    {
        public Task<TResult> ClaimsGetAsync<TResult>(Uri claimsLocation, Func<IClaim[], TResult> success, Func<HttpStatusCode, string, TResult> error)
        {
            var queryParams = claimsLocation.ParseQueryString();
            var claims = ParseClaims(queryParams);
            return Task.FromResult(success(claims.ToArray()));
        }

        private IEnumerable<IClaim> ParseClaims(NameValueCollection queryParams)
        {
            foreach (var queryParamKey in queryParams.Keys)
            {
                Uri typeUri = new Uri("http://exception.com");
                string value = string.Empty;
                try
                {
                    var typeString = System.Web.HttpUtility.UrlDecode((string)queryParamKey);
                    typeUri = new Uri(typeString);
                    value = (string)queryParams[(string)queryParamKey];
                }
                catch (Exception ex)
                {
                    value = ex.Message;
                }
                yield return new Claim()
                {
                        Type = typeUri,
                        Value = value,
                };
            }
        }
    }
}
