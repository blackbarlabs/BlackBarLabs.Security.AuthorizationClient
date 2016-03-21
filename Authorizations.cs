﻿using BlackBarLabs.Core.Web;
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
            public Uri[] ClaimsProviders { get; set; }

            [DataMember]
            public CredentialsType[] CredentialProviders { get; set; }

            [DataMember]
            public Guid Id { get; set; }
        }

        private static WebRequest GetRequest()
        {
            var authServerLocation = ConfigurationManager.AppSettings["BlackBarLabs.Security.AuthorizationClient.ServerUrl"];
            var webRequest = WebRequest.Create(authServerLocation + "/api/Authorization");
            return webRequest;
        }

        public async static Task<T> CreateImplicitVoucherAsync<T>(Guid authId, Uri providerId,
            string username, string password, TimeSpan voucherDuration, Uri claimsLocation,
            Func<T> onSuccess, Func<string, T> onFailure)
        {
            var credentialImplicit = new CredentialsType
            {
                Method = CredentialValidationMethodTypes.Implicit,
                Provider = providerId,
                Token = password,
                UserId = username,
                ClaimsProviders = new Uri [] { claimsLocation },
            };
            
            var token = CredentialProvider.Voucher.Utilities.GenerateToken(authId, DateTime.UtcNow + voucherDuration);
            var credentialVoucher = new CredentialsType
            {
                Method = CredentialValidationMethodTypes.Voucher,
                Provider = providerId,
                Token = token,
                UserId = authId.ToString("N"),
                ClaimsProviders = new Uri[] { claimsLocation },
            };

            var auth = new Authorization()
            {
                Id = authId,
                CredentialProviders = new CredentialsType []
                {
                    credentialVoucher,
                    credentialImplicit,
                }
            };

            var webRequest = GetRequest();
            return await webRequest.PostAsync(auth,
                (response) => onSuccess(),
                (code, response) => onFailure(response));
        }

        public delegate T CreateVoucherCredentialDelegate<T>();
        public delegate T CreateImplicitCredentialDelegate<T>();
        public delegate TResult CreateSuccessDelegate<TResult, TResultVoucher, TResultImplicit>(
            CreateVoucherCredentialDelegate<TResultVoucher> createVoucher, CreateImplicitCredentialDelegate<TResultImplicit> createImplicit);
        public static async Task CreateAsync<TResult, TResultVoucher, TResultImplicit>(Guid authId,
            CreateSuccessDelegate<TResult, TResultVoucher, TResultImplicit> success, Func<string, TResult> failure)
        {
            var auth = new Authorization()
            {
                Id = authId,
                CredentialProviders = new CredentialsType[] { }
            };
            
            var webRequest = GetRequest();
            return await webRequest.PostAsync(auth,
                (response) => success(
                    () => CreateVoucherAsync(authId)),
                (code, response) => failure(response));
        }

        public static async Task<TResult> CreateVoucherAsync<TResult>(Guid authId)
        {

        }

        public static string GenerateToken(Guid authId, TimeSpan voucherDuration)
        {
            var token = CredentialProvider.Voucher.Utilities.GenerateToken(authId, DateTime.UtcNow + voucherDuration);
            return token;
        }
    }
}
