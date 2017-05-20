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

    class UpdateKindActionsCount: IDeveloperOperation
    {
        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>();
            var kindActionsRepository = IocConfig.GetConfiguredContainer().Resolve<IKindActionRepository>();

            var usersToUpdate = await repo.GetUsers(new QueryOptions<User>());
            foreach (var user in usersToUpdate)
            {
                var userKindActions =
                    await kindActionsRepository.GetKindActions(
                        new QueryOptions<KindAction> { Filter = (kindAction) => kindAction.UserId == user.Id });

                user.KindActionsCount = userKindActions.Count;
                if (user.KindActionsCount > 0)
                {
                    await kindActionsRepository.RemoveKindActions(user.Id);
                    foreach (var userKindAction in userKindActions)
                    {
                        await kindActionsRepository.AddKindAction(userKindAction);
                    }
                    await repo.UpdateUser(user, false);
                }

                
            }
            return new OperationResult(OperationResultStatus.Success);
        }
    }
}
