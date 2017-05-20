namespace RadrugaCloud.Controllers.Api
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.NonDomainModels;

    using RadrugaCloud.Helpers;

    using Services.DomainServices;

    /// <summary>
    ///     Class UserController
    /// </summary>
    [Authorize]
    public class UserAccountController : ApiController
    {
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserAccountController(UserService userService)
        {
            _userService = userService;
        }


        /// <summary>
        /// Purchases the coins.
        /// </summary>
        /// <param name="purchase">The purchase.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<OperationResult> PurchaseCoins(Purchase purchase)
        {
            switch (purchase.StoreType)
            {
                case StoreType.AppStore:
                    return await CheckApple(purchase);
                case StoreType.GooglePlay:
                    return CheckGoogle(purchase);
                default:
                    throw new ArgumentException($"Invalid store {purchase.StoreType}");
            }
        }

        private OperationResult CheckGoogle(Purchase purchase)
        {
            // http://stackoverflow.com/questions/21807638/verifying-a-google-play-in-app-billing-signature-in-net-2048-bit-key-pkcs-1?answertab=active#tab-top


            var publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA9c6RMFKAKVF2d7Iu8cPCw7BK+o8QOlwJCZS7VgtwwXNpKt0Z1a0S8nYq/DQXehWdbGpiN9fAIUkQZqpexbWifpp68pz9ACy8kf+85OG4BbeqbSUX3Ps96JAQm2AgqtmeierMUYVQ88Gm6tvdHR5khsJGQcN4mPUzKclkSgPJ4uA7OSfdTPRJRlWkRT0HHWY/BwKq1N9RTWj+J6oJCT/es2nuImMrXJ6tj9SBuaWVgbUlris9qQj06aC91Oul11lAhnpo/onNOXDDpQup3VU3Fx50Cv8YBvo8wQ4pDAYMRTSnO0KfRCF25V5zgioMt5ax/5QL9kWLoYAMVEgxcXE1qwIDAQAB";

            byte[] purchaseInfoBytes = Encoding.UTF8.GetBytes(purchase.ReceiptData);
            byte[] signatureBytes = Convert.FromBase64String(purchase.Signature);
            byte[] publicKeyBytes = Convert.FromBase64String(publicKey);

            //using PHP's native support to verify the signature
            var result = VerifySignature_2048_Bit_PKCS1_v1_5(
                    purchaseInfoBytes,
                    signatureBytes,
                    publicKeyBytes);

            return result
                       ? new OperationResult(OperationResultStatus.Success)
                       : new OperationResult(OperationResultStatus.Error, "Error validating purchase");
        }

        private async Task<OperationResult> CheckApple(Purchase purchase)
        {
            //https://gist.github.com/menny/1985010
            //http://stackoverflow.com/questions/22933133/verifying-ios-in-app-purchase-receipt-with-c-sharp

            var appleHost = purchase.IsTest ? "ssl://sandbox.itunes.apple.com" : "ssl://buy.itunes.apple.com";
            var userId = this.GetCurrentUserId();

            var json = $"{{\"receipt-data\" : \"{purchase.ReceiptData}\"}}";
            var client = new HttpClient();

            var response = await client.PostAsync(appleHost + "/verifyReceipt", new StringContent(json));
            if (!response.IsSuccessStatusCode)
            {
                Trace.TraceError(
                    $"Purchase failed. Data:{purchase.ReceiptData}. User:{userId}. Store:{purchase.StoreType}. Product:{purchase.ProductId}."
                    + $" Test:{purchase.IsTest}. Response:{await response.Content.ReadAsStringAsync()}");
                return new OperationResult(OperationResultStatus.Error, response.ReasonPhrase);
            }
            //taking the JSON response
            var result = await response.Content.ReadAsAsync<dynamic>();
            var status = result.status;
            if (status == 0) //eithr OK or expired and needs to synch
            {
                //here are some fields from the json, btw.
                var receipt = result.receipt;
                var transactionId = receipt.transaction_id;
                var originalTransactionId = receipt.original_transaction_id;
                var latestReceiptInfo = result.latest_receipt_info;
                return new OperationResult(OperationResultStatus.Success);
            }
            else
            {
                return new OperationResult(OperationResultStatus.Error, result.status);
            }
        }

        public static RSAParameters GetRsaParameters_2048_Bit_PKCS1_v1_5(byte[] publicKey)
        {
            RSAParameters rsaParameters = new RSAParameters();

            int modulusOffset = 33;     // See comments above
            int modulusBytes = 256;     // 2048 bit key
            int exponentOffset = 291;   // See comments above
            int exponentBytes = 3;      // See comments above

            byte[] modulus = new byte[modulusBytes];
            for (int i = 0; i < modulusBytes; i++)
                modulus[i] = publicKey[modulusOffset + i];

            byte[] exponent = new byte[exponentBytes];
            for (int i = 0; i < exponentBytes; i++)
                exponent[i] = publicKey[exponentOffset + i];

            rsaParameters.Modulus = modulus;
            rsaParameters.Exponent = exponent;

            return rsaParameters;
        }

        public static bool VerifySignature_2048_Bit_PKCS1_v1_5(byte[] data, byte[] signature, byte[] publicKey)
        {
            // Compute an SHA1 hash of the raw data
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);

            // Specify the public key
            RSAParameters rsaParameters = GetRsaParameters_2048_Bit_PKCS1_v1_5(publicKey);

            // Use RSACryptoProvider to verify the signature with the public key
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(rsaParameters);

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA1");
            return rsaDeformatter.VerifySignature(hash, signature);
        }
    }
}