using BlackBarLabs.Core;
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

        private async static Task<string> FetchSessionToken(Session session)
        {

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

        public async static Task<string> CreateWithVoucherAsync(Guid authId, string authToken)
        {
            var providerId = ConfigurationManager.AppSettings["BlackbarLabs.Security.CredentialProvider.Voucher.provider"].ToUri();

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
                Credentials = credentialVoucher,
            };

            return await FetchSessionToken(session);
        }
        
        public async static Task<string> CreateWithImplicitAsync(string username, string password)
        {
            var providerId = ConfigurationManager.AppSettings["BlackbarLabs.Security.CredentialProvider.Implicit.provider"].ToUri();

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

            return await FetchSessionToken(session);
        }
    }
}
