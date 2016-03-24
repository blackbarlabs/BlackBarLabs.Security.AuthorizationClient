using BlackBarLabs.Core.Web;
using BlackBarLabs.Security.Authorization;
using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public static class Authorizations
    {
        [DataContract]
        private class Authorization : IAuthorization
        {
            [DataMember]
            public Uri[] CredentialProviders { get; set; }

            [DataMember]
            public Guid Id { get; set; }
        }

        private static WebRequest GetRequest()
        {
            var authServerLocation = ConfigurationManager.AppSettings["BlackBarLabs.Security.AuthorizationClient.ServerUrl"];
            var webRequest = WebRequest.Create(authServerLocation + "/api/Authorization");
            return webRequest;
        }

        public async static Task<T> CreateAsync<T>(Guid authId,
            Func<T> onSuccess, Func<string, T> onFailure)
        {
            var auth = new Authorization()
            {
                Id = authId,
            };

            var webRequest = GetRequest();
            return await webRequest.PostAsync(auth,
                (response) => onSuccess(),
                (code, response) => onFailure(response));
        }

        public static async Task<TResult> DeleteAsync<TResult>(Guid authId, 
            Func<TResult> success,
            Func<HttpStatusCode, string, TResult> failedResponse,
            Func<TResult> couldNotConnect)
        {
            var auth = new Authorization()
            {
                Id = authId,
            };

            var webRequest = GetRequest();
            return await webRequest.DeleteAsync(auth,
                (response) => success(),
                (code, response) => failedResponse(code, response));
        }
    }
}
