using BlackBarLabs.Core.Web;
using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlackBarLabs.Security.Authorization
{
    [DataContract]
    public static class Credentials
    {
        [DataContract]
        internal class Credential : ICredential
        {
            #region Properties
            
            [DataMember]
            public Guid AuthorizationId { get; set; }

            [DataMember]
            public CredentialValidationMethodTypes Method { get; set; }

            [DataMember]
            public Uri Provider { get; set; }

            [DataMember]
            public string UserId { get; set; }

            [DataMember]
            public string Token { get; set; }

            [DataMember]
            public Uri[] ClaimsProviders { get; set; }

            #endregion
        }

        private static WebRequest GetRequest()
        {
            var authServerLocation = ConfigurationManager.AppSettings["BlackBarLabs.Security.AuthorizationClient.ServerUrl"];
            var webRequest = WebRequest.Create(authServerLocation + "/api/Credential");
            return webRequest;
        }
        
        public async static Task<T> CreateImplicitAsync<T>(Guid authId, Uri providerId,
            string username, string password, TimeSpan voucherDuration, Uri claimsLocation,
            Func<T> onSuccess, Func<string, T> onFailure)
        {
            var credentialImplicit = new Credential
            {
                AuthorizationId = authId,
                Method = CredentialValidationMethodTypes.Implicit,
                Provider = providerId,
                Token = password,
                UserId = username,
                ClaimsProviders = new Uri[] { claimsLocation },
            };
            var webRequest = GetRequest();
            return await webRequest.PostAsync(credentialImplicit,
                (response) => onSuccess(),
                (code, response) => onFailure(response));
        }

        public async static Task<T> CreateVoucherAsync<T>(Guid authId, Uri providerId,
            string username, string password, TimeSpan voucherDuration, Uri claimsLocation,
            Func<T> onSuccess, Func<string, T> onFailure)
        {
            var token = CredentialProvider.Voucher.Utilities.GenerateToken(authId, DateTime.UtcNow + voucherDuration);
            var credentialVoucher = new Credential
            {
                Method = CredentialValidationMethodTypes.Voucher,
                Provider = providerId,
                Token = token,
                UserId = authId.ToString("N"),
                ClaimsProviders = new Uri[] { claimsLocation },
            };
            var webRequest = GetRequest();
            return await webRequest.PostAsync(credentialVoucher,
                (response) => onSuccess(),
                (code, response) => onFailure(response));
        }
    }
}