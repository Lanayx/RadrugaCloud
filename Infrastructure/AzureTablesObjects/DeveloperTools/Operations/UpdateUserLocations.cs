using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Microsoft.Practices.Unity;

    class UpdateUserLocations: IDeveloperOperation
    {
        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>();
            var locationProvider = IocConfig.GetConfiguredContainer().Resolve<ILocationProvider>();

            var usersToUpdate = await repo.GetUsers(new QueryOptions<User>());
            foreach (var user in usersToUpdate)
            {
                if (user.HomeCoordinate != null)
                {
                    var cityInfo = await locationProvider.GetUserCityInfo(user.HomeCoordinate);
                    if (cityInfo != null)
                    {
                        user.CityShortName = cityInfo.CityShortName;
                        user.CountryShortName = cityInfo.CountryShortName;
                    }
                }
                await repo.UpdateUser(user, false);
            }
            return new OperationResult(OperationResultStatus.Success);
        }
    }
}
