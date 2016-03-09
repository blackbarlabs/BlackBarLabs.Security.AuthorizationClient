﻿using BlackBarLabs.Security.Authorization;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BlackBarLabs.Security.AuthorizationClient
{
    public static class Authorizations
    {
        private class Authorization : IAuthorization
        {
            public CredentialsType[] CredentialProviders { get; set; }

            public Guid Id { get; set; }
        }

        public async static Task CreateImplicitVoucherAsync(Guid authId, string password)
        {
            var authServerLocation = ConfigurationManager.AppSettings["BlackBarLabs.Security.AuthorizationClient.ServerUrl"];
            
            var trustedVoucherProverId = CredentialProvider.Voucher.Utilities.GetTrustedProviderId();
            var token = CredentialProvider.Voucher.Utilities.GenerateToken(authId, DateTime.UtcNow + TimeSpan.FromMinutes(10.0));

            var credentialVoucher = new CredentialsType
            {
                Method = CredentialValidationMethodTypes.Voucher,
                Provider = trustedVoucherProverId,
                Token = token,
                UserId = authId.ToString("N"),
            };
            var credentialImplicit = new CredentialsType
            {
                Method = CredentialValidationMethodTypes.Implicit,
                Provider = new Uri("http://www.example.com/Auth"),
                Token = password,
                UserId = authId.ToString("N"),
            };

            var auth = new Authorization()
            {
                Id = Guid.NewGuid(),
                CredentialProviders = new CredentialsType []
                {
                    credentialVoucher,
                    credentialImplicit,
                }
            };

            var authJson = Newtonsoft.Json.JsonConvert.SerializeObject(auth);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authServerLocation + "/api/Authorization");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(authJson);
                streamWriter.Flush();
            }
            var createAuthResponse = ((HttpWebResponse)(await httpWebRequest.GetResponseAsync()));

        }
    }
}
