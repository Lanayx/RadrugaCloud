using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Providers
{
    using System.Device.Location;

    using Core.NonDomainModels;

    public interface ILocationProvider
    {
        Task<UserCityInfo> GetUserCityInfo(GeoCoordinate coordinate);
    }
}
