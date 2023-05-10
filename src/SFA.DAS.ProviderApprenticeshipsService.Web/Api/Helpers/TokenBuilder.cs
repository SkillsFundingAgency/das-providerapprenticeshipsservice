using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Constants;
using System;
using System.Security.Cryptography;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Api.Helpers
{
    public interface ITokenBuilder
    {
        string CreateToken();
    }

    public sealed class TokenBuilder: ITokenBuilder
    {
        private byte[] SecretKey { get; set; }
        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly TokenData _tokenData = new TokenData();

        public TokenBuilder(ITokenDataSerializer tokenDataSerializer, IDfESignInServiceConfiguration config)
        {
            _tokenDataSerializer = tokenDataSerializer;
            _tokenData.Header.Add("typ", AuthConfig.TokenType);
            _tokenData.Header.Add("alg", JsonWebAlgorithm.GetAlgorithm(AuthConfig.Algorithm));
            _tokenData.Payload.Add("aud", AuthConfig.Aud);
            _tokenData.Payload.Add("iss", config.DfEOidcClientConfiguration.ClientId);

            SecretKey = System.Text.Encoding.UTF8.GetBytes(config.DfEOidcClientConfiguration.ApiServiceSecret);
        }

        public string CreateToken()
        {
            var headerBytes = System.Text.Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(_tokenData.Header));
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(_tokenData.Payload));
            var bytesToSign = System.Text.Encoding.UTF8.GetBytes($"{Base64Encode(headerBytes)}.{Base64Encode(payloadBytes)}");
            var signedBytes = SignToken(SecretKey, bytesToSign);

            return $"{Base64Encode(headerBytes)}.{Base64Encode(payloadBytes)}.{Base64Encode(signedBytes)}";
        }

        private static byte[] SignToken(byte[] key, byte[] bytesToSign)
        {
            using (var algorithm = HMAC.Create(AuthConfig.Algorithm))
            {
                algorithm.Key = key;
                return algorithm.ComputeHash(bytesToSign);
            }
        }
        private static string Base64Encode(byte[] stringInput)
        {
            return Convert.ToBase64String(stringInput).Split(new[] { '=' })[0].Replace('+', '-').Replace('/', '_');
        }
    }
}
