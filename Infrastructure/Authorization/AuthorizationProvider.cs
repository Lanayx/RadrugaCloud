// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Class AuthorizationProvider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure.Authorization
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Microsoft.Owin.Security.OAuth;
    /// <summary>
    ///     Class AuthorizationProvider
    /// </summary>
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// The _user identity repository.
        /// </summary>
        private readonly IUserIdentityRepository _userIdentityRepository;

        /// <summary>
        /// The _user repository
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationProvider"/> class.
        /// </summary>
        /// <param name="userIdentityRepository">
        /// The user identity repository.
        /// </param>
        public AuthorizationProvider(IUserIdentityRepository userIdentityRepository, IUserRepository userRepository)
        {
            _userIdentityRepository = userIdentityRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user
        ///     has provided name and password
        ///     credentials directly into the client application's user interface, and the client application is using those to
        ///     acquire an "access_token" and
        ///     optional "refresh_token". If the web application supports the
        ///     resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To
        ///     issue an
        ///     access token the context.Validated must be called with a new ticket containing the claims about the resource owner
        ///     which should be associated
        ///     with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by
        ///     malicious callers.
        ///     The default behavior is to reject this grant type.
        ///     See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context">
        /// The context of the event carries information in and results out.
        /// </param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity =
                (await
                 _userIdentityRepository.GetUsersIdentities(
                     new QueryOptions<UserIdentity> { Filter = user => user.LoginEmail == context.UserName }))
                    .FirstOrDefault();
            if (identity == null
                || !HashHelper.VerifyHashedPassword(identity.HashedPassword, context.Password, identity.HashType)
                || !String.IsNullOrEmpty(identity.EmailConfirmationCode))
            {
                context.Rejected();
                return;
            }

            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim(ClaimTypes.Sid, identity.Id));
            context.Validated(id);
        }

        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id", and that the correct credentials for
        ///     that client are
        ///     present on the request. If the web application accepts Basic authentication credentials,
        ///     context.TryGetBasicCredentials(out clientId, out clientSecret) may be called to acquire those values if present in
        ///     the request header. If the web
        ///     application accepts "client_id" and "client_secret" as form encoded POST parameters,
        ///     context.TryGetFormCredentials(out clientId, out clientSecret) may be called to acquire those values if present in
        ///     the request body.
        ///     If context.Validated is not called the request will not proceed further.
        /// </summary>
        /// <param name="context">
        /// The context of the event carries information in and results out.
        /// </param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Factory.StartNew(() => { context.Validated(); });
        }

        /// <summary>
        /// Called at the final stage of a successful Token endpoint request. An application may implement this call in order to do any final
        /// modification of the claims being used to issue access or refresh tokens. This call may also be used in order to add additional
        /// response parameters to the Token endpoint's json response body.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var userId = context.Identity.FindFirst(ClaimTypes.Sid).Value;
            var user = await _userRepository.GetUser(userId);
            if (user == null) {
                context.AdditionalResponseParameters.Add("requiredFields", new Core.DomainModels.User().CheckImportantFields().JoinToString());
                return;
            }

            var neededFields = user.CheckImportantFields();
            if (neededFields.Any())
            {
                context.AdditionalResponseParameters.Add("requiredFields", neededFields.JoinToString());
            } 
        }
    }
}