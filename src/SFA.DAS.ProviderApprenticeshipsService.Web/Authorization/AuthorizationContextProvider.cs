using System;
using System.Web;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.ProviderPermissions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly HttpContextBase _httpContext;
//        private readonly IHashingService _hashingService;
//        private readonly IAuthenticationService _authenticationService;
//
//        public AuthorizationContextProvider(HttpContextBase httpContext, IHashingService hashingService, IAuthenticationService authenticationService)
//        {
//            _httpContext = httpContext;
//            _hashingService = hashingService;
//            _authenticationService = authenticationService;
//        }

        public AuthorizationContextProvider(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            
            //todo: see VerificationOfRelationship POST & get
            // GET: VerificationOfRelationship(long providerId, string hashedCommitmentId)
            // POST: VerificationOfRelationship(VerificationOfRelationshipViewModel viewModel)
            //    public class VerificationOfRelationshipViewModel {
            //    public long ProviderId { get; set; }
            //    public string HashedCommitmentId { get; set; }
            
            // look for providerid
            // look for AccountLegalEntityPublicHashedId / AccountLegalEntityId directly
            // if not found, look for HashedCommitmentId / CommitmentId and fetch AccountLegalEntityPublicHashedIdFrom db
            // unhash along the way if necessary
            // bit nasty going to db every time
            // does this get called for every action?
            // in provider permissions case, we only actually need the context if a provider permission has been set on the DasAuthorizeAttribute, otherwise we should avoid getting the context
            
//            authorizationContext.Set(AuthorizationContextKeys.AccountLegalEntityId, GetAccountLegalEntityId());
//            authorizationContext.Set(AuthorizationContextKeys.ProviderId, GetProviderId());

            authorizationContext.Set("AccountLegalEntityId", GetAccountLegalEntityId());
            authorizationContext.Set("ProviderId", GetProviderId());
            
            return authorizationContext;
        }
        
        private string GetAccountLegalEntityId()
        {
            throw new NotImplementedException();
//            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteDataKeys.AccountHashedId, out var accountHashedId))
//            {
//                return null;
//            }
//
//            return (string)accountHashedId;
        }

        private long GetProviderId()
        {
            throw new NotImplementedException();
        }

//        private Guid? GetUserRef()
//        {
//            throw new NotImplementedException();
//        }
    }
}