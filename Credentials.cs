using BlackBarLabs.Core.Web;
using BlackBarLabs.Security.Authorization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlackBarLabs.Security.AuthorizationClient
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
            string username, string password,
            Func<T> onSuccess, Func<string, T> onFailure)
        {
            var credentialImplicit = new Credential
            {
                AuthorizationId = authId,
                Method = CredentialValidationMethodTypes.Implicit,
                Provider = providerId,
                Token = password,
                UserId = username,
            };
            var webRequest = GetRequest();
            return await webRequest.PostAsync(credentialImplicit,
                (response) => onSuccess(), // TODO: auth header cookies
                (code, response) => onFailure(response),
                (whyFailed) => onFailure(whyFailed));
        }

        public delegate TResult CreateVoucherDelegate<TResult>(string token);
        public async static Task<T> CreateVoucherAsync<T>(Guid authId, Uri providerId,
            TimeSpan voucherDuration,
            CreateVoucherDelegate<T> onSuccess, Func<string, T> onFailure)
        {
            var token = BlackBarLabs.Security.Tokens.VoucherTools.GenerateToken(authId, DateTime.UtcNow + voucherDuration);
            var credentialVoucher = new Credential
            {
                AuthorizationId = authId,
                Method = CredentialValidationMethodTypes.Voucher,
                Provider = providerId,
                Token = token,
                UserId = authId.ToString("N"),
            };
            var webRequest = GetRequest();
            return await webRequest.PostAsync(credentialVoucher,
                (response) => onSuccess(token),
                (code, response) => onFailure(response),
                (whyFailed) => onFailure(whyFailed));
        }
    }
}