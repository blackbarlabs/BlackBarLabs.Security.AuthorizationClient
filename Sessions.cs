using BlackBarLabs.Core.Web;
using BlackBarLabs.Security.Authorization;
using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public static class Sessions
    {
        [DataContract]
        private class Session : ISession
        {
            [DataMember]
            public Guid Id { get; set; }

            [DataMember]
            public Guid AuthorizationId { get; set; }

            [DataMember]
            public ICredential Credentials { get; set; }
            
            [DataMember]
            public string RefreshToken { get; set; }

            [DataMember]
            public AuthHeaderProps SessionHeader { get; set; }
        }

        private static WebRequest GetRequest()
        {
            var authServerLocation = ConfigurationManager.AppSettings["BlackBarLabs.Security.AuthorizationClient.ServerUrl"];
            var webRequest = WebRequest.Create(authServerLocation + "/api/Session");
            return webRequest;
        }

        public async static Task CreateWithVoucherAsync(Guid authId, Uri providerId, string authToken)
        {
            var credentialVoucher = new Credentials.Credential
            {
                Method = CredentialValidationMethodTypes.Voucher,
                Provider = providerId,
                Token = authToken,
                UserId = authId.ToString("N"),
            };

            var session = new Session()
            {
                Id = Guid.NewGuid(),
                AuthorizationId = authId,
                Credentials = credentialVoucher,
            };

            var webRequest = GetRequest();
            await webRequest.PostAsync(session, (response) => true, (responseCode, response) => false);
        }
        
        public async static Task<string> CreateWithImplicitAsync(Uri providerId, string username, string password)
        {
            var credentialImplicit = new Credentials.Credential
            {
                Method = CredentialValidationMethodTypes.Implicit,
                Provider = providerId,
                Token = password,
                UserId = username,
            };

            var session = new Session()
            {
                Id = Guid.NewGuid(),
                Credentials = credentialImplicit,
            };

            var webRequest = GetRequest();
            return await webRequest.PostAsync(session,
                (response) =>
                {
                    var responseText = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                    var responseSession = Newtonsoft.Json.JsonConvert.DeserializeObject<Session>(responseText);
                    return responseSession.SessionHeader.Value;
                },
                (responseCode, response) => default(string));
        }
    }
}
