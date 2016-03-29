using BlackBarLabs.Core.Web;
using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlackBarLabs.Security.Authorization
{
    [DataContract]
    public static class Claims
    {
        [DataContract]
        internal class Claim : IClaim
        {
            #region Properties
            
            [DataMember]
            public string Issuer { get; set; }

            [DataMember]
            public string OriginalIssuer { get; set; }

            [DataMember]
            public IDictionary<string, string> Properties { get; set; }

            [DataMember]
            public string Type { get; set; }

            [DataMember]
            public string Value { get; set; }

            [DataMember]
            public string ValueType { get; set; }

            #endregion
        }
        
        public async static Task<TResult> FetchClaimsAsync<TResult>(Uri claimsLocation,
            Func<IClaim[], TResult> onSuccess, Func<HttpStatusCode, string, TResult> onFailure)
        {
            var webRequest = WebRequest.Create(claimsLocation);
            return await webRequest.GetAsync<Claim[], TResult>(
                (claims) => onSuccess(claims),
                (code, response) => onFailure(code, response));
        }
    }
}