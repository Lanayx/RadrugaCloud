namespace RadrugaCloud.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Tools;

    using Microsoft.Owin.Security;

    using Newtonsoft.Json.Linq;

    using RadrugaCloud.Helpers;
    using RadrugaCloud.Models.Api;

    using Services.AuthorizationServices;
    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     Controller for user identity operations
    /// </summary>
    public class UserIdentityController : ApiController
    {
        #region Fields

        private readonly UserIdentityService _identityService;

        private readonly UserService _userService;

        private readonly MailService _mailService;

        private static string serverVkToken = String.Empty;

        private AppCountersService _appCountersService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentityController" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="identityService">The identity service.</param>
        /// <param name="mailService">The mail service.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public UserIdentityController(UserService userService, UserIdentityService identityService, MailService mailService, AppCountersService appCountersService)
        {
            _userService = userService;
            _identityService = identityService;
            _mailService = mailService;
            _appCountersService = appCountersService;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Registers the specified register model.
        /// </summary>
        /// <param name="requiredFields">The required fields.</param>
        /// <returns>
        /// Task{AddResult}.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Currently not implemented</exception>
        [HttpPost]
        [Authorize]
        public async Task<OperationResult> RequiredFields(RequiredFields requiredFields)
        {
            if (!ModelState.IsValid)
                return new RegisterResult(OperationResultStatus.Error, "Validation error");

            User user = await _userService.GetUser(this.GetCurrentUserId());
            var existingUser = false;
            if (user != null)
                existingUser = true;
            else
                user = new User();
            if (String.IsNullOrEmpty(user.Id))
                user.Id = this.GetCurrentUserId();
            if (String.IsNullOrEmpty(user.NickName))
                user.NickName = requiredFields.NickName;
            if (user.DateOfBirth == null)
                user.DateOfBirth = requiredFields.DateOfBirth;
            if (user.Sex == null || user.Sex == Sex.NotSet)
                user.Sex = requiredFields.Sex;

            var neededFields = user.CheckImportantFields();
            if (neededFields.Any())
            {
                return new RegisterResult(OperationResultStatus.Error, "Validation error");
            }

            var registerResult = existingUser 
                ? await _userService.UpdateUser(user)
                : await _userService.AddNewUser(user);
            

            return registerResult;
        }

        /// <summary>
        ///     Registers the vk.
        /// </summary>
        /// <param name="registerModel">The register model.</param>
        /// <returns>Task{RegisterResult}.</returns>
        [HttpPost]
        public async Task<RegisterResult> RegisterVk(RegisterVkModel registerModel)
        {
            if (registerModel == null || registerModel.Id == 0 || String.IsNullOrEmpty(registerModel.AccessToken)
                || !(await CheckVkToken(registerModel)))
            {
                return new RegisterResult(OperationResultStatus.Error, "Wrong Vk data");
            }

            var identity = new UserIdentity
                               {
                                   VkIdentity = registerModel.GetVkIdentity(),
                                   Device = registerModel.Device
                               };
            var result = await _identityService.AddUserIdentity(identity);
            if (result.Status == OperationResultStatus.Error)
            {
                return RegisterResult.FromIdResult(result);
            }

            RegisterResult registerResult;
            if (result.Status == OperationResultStatus.Success)
            {
                var user = new User { Id = result.Id };
                registerModel.FillUserFields(user);
                registerResult = RegisterResult.FromIdResult(await _userService.AddNewUser(user));
                if (registerResult.Status != OperationResultStatus.Success)
                {
                    return registerResult;
                }

                var neededFields = user.CheckImportantFields();
                if (neededFields.Any())
                {
                    registerResult.Status = OperationResultStatus.Warning;
                    registerResult.Description = neededFields.JoinToString();
                }

                await _appCountersService.UserRegistered();
            }
            else //warning, user exists
            {
                registerResult = new RegisterResult(result.Id);
            }

            if (Request != null)
            {
                SignInRequest(registerResult);
            }

            return registerResult;
        }

        ///// <summary>
        ///// Resets the password.
        ///// </summary>
        ///// <param name="newPassword">The new password.</param>
        ///// <returns>
        ///// Task{OperationResult}.
        ///// </returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<OperationResult> ResetPassword([FromBody] string newPassword)
        //{
        //    if (String.IsNullOrEmpty(newPassword))
        //        return new OperationResult(OperationResultStatus.Error, "Password can't be empty");
        //    var identity = await _identityService.GetUserIdentity(this.GetCurrentUserId());
        //    identity.HashedPassword = HashHelper.GetPasswordHash(newPassword);
        //    return await _identityService.UpdateUserIdentity(identity);
        //}

        /// <summary>
        ///     Validates the approval code.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task{OperationResult}.</returns>
        [HttpPost]
        public async Task<RegisterResult> ValidateApprovalCode(ResetPasswordModel model)
        {
            var queryOptions = new QueryOptions<UserIdentity> { Filter = i => i.LoginEmail == model.Email };
            var identity = (await _identityService.GetUsersIdentities(queryOptions)).FirstOrDefault();
            if (identity == null)
            {
                return new RegisterResult(OperationResultStatus.Error, "Specified email was not found");
            }

            if (model.ApprovalCode == identity.EmailConfirmationCode && identity.EmailConfirmationAttempts <= 5)
            {
                identity.EmailConfirmationCode = "";
                identity.EmailConfirmationAttempts = 0;
                await _identityService.UpdateUserIdentity(identity);

                var result = new RegisterResult(identity.Id);
                SignInRequest(result);
                return result;
            }
            if (identity.EmailConfirmationAttempts <= 5)
            {
                identity.EmailConfirmationAttempts += 1;
                await _identityService.UpdateUserIdentity(identity);
                return new RegisterResult(OperationResultStatus.Error, "Approval code is not correct");
            }

            return new RegisterResult(OperationResultStatus.Error, "Maximum attempts reached");
        }


        /// <summary>
        /// Registers the specified register model.
        /// </summary>
        /// <param name="registerModel">The register model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OperationResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return new OperationResult(OperationResultStatus.Error, "Validation error");

            var queryOptions = new QueryOptions<UserIdentity> { Filter = i => i.LoginEmail == registerModel.LoginEmail };
            var identity = (await _identityService.GetUsersIdentities(queryOptions)).FirstOrDefault();
            if (identity == null)
            {
                identity = new UserIdentity
                               {
                                   LoginEmail = registerModel.LoginEmail,
                                   HashType = HashHelper.CurrentHashType,
                                   HashedPassword = HashHelper.GetPasswordHash(registerModel.Password),
                                   Device = registerModel.Device
                               };

                var addResult = await _identityService.AddUserIdentity(identity);
                if (addResult.Status != OperationResultStatus.Success)
                    return addResult;
                identity.Id = addResult.Id;

                //TODO make calls in parallel
                await _appCountersService.UserRegistered();
                return await _mailService.SendEmailApproval(identity);
            }
            else
            {
                if (!String.IsNullOrEmpty(identity.EmailConfirmationCode))
                    return await _mailService.SendEmailApproval(identity);
            }

            return new OperationResult(OperationResultStatus.Error, "Email is already in use");
        }

        /// <summary>
        ///     Validates the email on forgot password.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Task{ValidateResult}.</returns>
        [HttpPost]
        public async Task<OperationResult> ForgotPassword([FromBody][EmailAddress] string email)
        {
            if (!ModelState.IsValid)
                return new OperationResult(OperationResultStatus.Error, "Validataion error");

            var queryOptions = new QueryOptions<UserIdentity> { Filter = i => i.LoginEmail == email };
            var identity = (await _identityService.GetUsersIdentities(queryOptions)).FirstOrDefault();
            if (identity != null)
            {
                return await _mailService.SendEmailApproval(identity);
            }

            return new OperationResult(OperationResultStatus.Error, "Specified email was not found");
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult> AddVkIdentity(RegisterVkModel registerModel)
        {
            if (registerModel == null || registerModel.Id == 0 || String.IsNullOrEmpty(registerModel.AccessToken)
                || !(await CheckVkToken(registerModel)))
            {
                return new OperationResult(OperationResultStatus.Error, "Wrong Vk data");
            }

            var user = await _identityService.GetUserIdentity(this.GetCurrentUserId());
            user.VkIdentity = registerModel.GetVkIdentity();
            return await _identityService.UpdateUserIdentity(user);
        }


        [Authorize]
        [HttpPost]
        public async Task<OperationResult> AddEmail(RegisterModel registerModel)
        {
            var validateResult = ValidateAddEmailRequest(registerModel);
            if (validateResult.Status != OperationResultStatus.Success)
            {
                return validateResult;
            }

            var queryOptions = new QueryOptions<UserIdentity> { Filter = i => i.LoginEmail == registerModel.LoginEmail };
            var identities = await _identityService.GetUsersIdentities(queryOptions);
            if (identities.Any())
            {
                return new OperationResult(OperationResultStatus.Error, "Email is already in use");
            }

            var user = await _identityService.GetUserIdentity(this.GetCurrentUserId());

            // We don't have to approve email in the first version
            user.LoginEmail = registerModel.LoginEmail;
            user.HashedPassword = HashHelper.GetPasswordHash(registerModel.Password);
            user.HashType = HashHelper.CurrentHashType;
            return await _identityService.UpdateUserIdentity(user);
        }



        #endregion

        #region Methods

       

        private void SignInRequest(RegisterResult registerResult)
        {
            var ctx = Request.GetOwinContext();
            var claimsIdentity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, registerResult.Id));
            var ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties());
            var currentUtc = DateTime.UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            var token = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            ctx.Authentication.SignIn(claimsIdentity);
            registerResult.OAuthToken = token;
        }



        private async Task<bool> CheckVkToken(RegisterVkModel registerModel, bool secondCall = false)
        {
            var client = new HttpClient();
            if (String.IsNullOrEmpty(serverVkToken))
            {
                var serverTokenResponse = await client.GetStringAsync(
                    "https://oauth.vk.com/access_token?client_id=4452546&client_secret=X7RGucnH1Q7GMnI3ndhw&v=5.30&grant_type=client_credentials");
                dynamic serverJson = JObject.Parse(serverTokenResponse);
                if (serverJson.access_token == null)
                    throw new Exception("Can't authorize server in vk");
                serverVkToken = serverJson.access_token.Value;
            }
            var checkTokenResponse = await client.GetStringAsync(
                    "https://api.vk.com/method/secure.checkToken?client_secret=X7RGucnH1Q7GMnI3ndhw&access_token="+
                    serverVkToken + "&token=" + registerModel.AccessToken);
            dynamic clientJson = JObject.Parse(checkTokenResponse);
            if (clientJson.response != null && clientJson.response.user_id!=null) 
                return clientJson.response.user_id.Value == registerModel.Id;
            if (clientJson.error != null && clientJson.error.error_code != null && clientJson.error.error_code.Value != 15 && !secondCall)//possible server token outdate
            {
                serverVkToken = string.Empty;
                return await CheckVkToken(registerModel, true);
            }
            if (clientJson.error != null && clientJson.error.error_code != null && clientJson.error.error_code.Value == 15)//invalid token
            {
                return false;
            }
            throw new Exception("Can't perform a client check in vk");
        }


        private OperationResult ValidateAddEmailRequest(RegisterModel model)
        {
            if (model == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Wrong model");
            }

            if (string.IsNullOrEmpty(model.LoginEmail))
            {
                return new OperationResult(OperationResultStatus.Error, "Email is not specified or incorrect");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return new OperationResult(OperationResultStatus.Error, "New password is not specified or incorrect");
            }

            return new OperationResult(OperationResultStatus.Success, "Success");
        }

        #endregion
    }
}