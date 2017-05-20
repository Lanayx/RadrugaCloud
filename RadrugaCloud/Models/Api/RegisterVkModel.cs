namespace RadrugaCloud.Models.Api
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    using Core.AuthorizationModels;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Tools.CopyHelper;

    /// <summary>
    /// Class RegisterVkModel
    /// </summary>
    [DataContract]
    public class RegisterVkModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember(Name = "id")]
        public uint Id { get; set; }


        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        [DataMember(Name = "city")]
        public VkReference City { get; set; }

        /// <summary>
        /// Gets or sets the counters.
        /// </summary>
        /// <value>The counters.</value>
        [DataMember(Name = "counters")]
        public Counters Counters { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [DataMember(Name = "country")]
        public VkReference Country { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        [DataMember(Name = "bdate")]
        public string DateOfBirth { get; set; }
     
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>The name of the nick.</value>
        [DataMember(Name = "nickname")]
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the second nickname.
        /// </summary>
        /// <value>The nick name2.</value>
        [DataMember(Name = "screen_name")]
        public string NickName2 { get; set; }

        /// <summary>
        /// Gets or sets the sex id.
        /// </summary>
        /// <value>The sex id.</value>
        [DataMember(Name = "sex")]
        public byte? SexId { get; set; }

        /// <summary>
        /// Gets or sets the university id.
        /// </summary>
        /// <value>The university id.</value>
        [DataMember(Name = "university")]
        public uint? UniversityId { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [DataMember(Name = "device")]
        public string Device { get; set; }

        #endregion

        #region Methods

        internal void FillUserFields(User user)
        {
            user.NickName = !string.IsNullOrEmpty(NickName)
                                ? NickName
                                : NickName2.EndsWith(Id.ToString(CultureInfo.InvariantCulture))
                                      ? string.Empty
                                      : NickName2;

            if (!string.IsNullOrEmpty(DateOfBirth))
            {
                var dateMothYear = DateOfBirth.Split('.').Select(x => Convert.ToInt32(x)).ToArray();
                if (dateMothYear.Length == 3)
                {
                    user.DateOfBirth = new DateTime(dateMothYear[2], dateMothYear[1], dateMothYear[0]);
                }
            }

            switch (SexId)
            {
                case 1:
                    user.Sex = Sex.Female;
                    break;
                case 2:
                    user.Sex = Sex.Male;
                    break;
                default:
                    user.Sex = Sex.NotSet;
                    break;
            }
        }

        internal VkIdentity GetVkIdentity()
        {
            return new VkIdentity
                       {
                           Id = Id, 
                           Counters = (VkCounters)Counters?.CopyTo(new VkCounters()), 
                           UniversityId = UniversityId, 
                           CountryId = Country?.Id, 
                           CityId = City?.Id
                       };
        }

        #endregion
    }
}