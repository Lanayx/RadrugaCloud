namespace RadrugaCloud.Tests.Authorization
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    using NUnit.Framework;
    using Thinktecture.IdentityModel.Client;

    [TestFixture]
    public class LoginTest
    {
        [Test]
        public async Task InvalidLogin()
        {
            var client = new OAuth2Client(new Uri(ConfigurationManager.AppSettings["ApiUrl"] + "token"));
            TokenResponse token = await client.RequestResourceOwnerPasswordAsync("User1Login", "WrongPassword");
            Assert.IsTrue(token.IsError);
        }

        [Test]
        public async Task Login()
        {
            var client = new OAuth2Client(new Uri(ConfigurationManager.AppSettings["ApiUrl"] + "token"));
            TokenResponse token = await client.RequestResourceOwnerPasswordAsync("User1Login", "User1Password");
            //hashed password is AF7csm9IcRKPmV28w1u1yWe2EnO02AwZOfhBdKSPoWZgbplnyr4PPk9dDSXfSo7ss7+b
            Assert.IsFalse(token.IsError);
        }
    }
}