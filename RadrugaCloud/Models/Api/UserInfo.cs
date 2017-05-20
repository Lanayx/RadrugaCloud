using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadrugaCloud.Models.Api
{
    using Core.DomainModels;

    public class UserInfo: User
    {
        public bool HasEmail { get; set; }

        public bool HasVk { get; set; }
    }
}