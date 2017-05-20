namespace Services.BL
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.Interfaces.Repositories;

    public class MailService
    {
        private readonly IMailProvider _mailProvider;

        private readonly IUserIdentityRepository _userIdentityRepository;

        private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

        public MailService(IMailProvider mailProvider, IUserIdentityRepository userIdentityRepository)
        {
            _mailProvider = mailProvider;
            _userIdentityRepository = userIdentityRepository;
        }

        public async Task<OperationResult> SendEmailApproval(UserIdentity user)
        {
            if (String.IsNullOrEmpty(user.EmailConfirmationCode))
            {
                user.EmailConfirmationCode = GetRegistrationCode();
                var userUpdateResult = await _userIdentityRepository.UpdateUserIdentity(user);
                if (userUpdateResult.Status != OperationResultStatus.Success)
                {
                    return userUpdateResult;
                }
            }

            return await _mailProvider.SendMail(
                user.LoginEmail,
                "Подтверждение почтового ящика",
                $"Для подтверждения введи код в своем приложении.<br><b>{user.EmailConfirmationCode}</b>");
        }

        private string GetRegistrationCode()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return String.Format("{0:D6}", random);

        }
    }
}
