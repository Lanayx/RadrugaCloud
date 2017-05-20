using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AzureTables
{
    using System.Diagnostics;

    using Core.Interfaces.Repositories;

    using Infrastructure.AzureTablesObjects;
    using Infrastructure.AzureTablesObjects.TableEntities;

    public sealed class UserDataRepository: IUserDataRepository
    {
        private readonly AzureTableStorageManager _azureManager;

        public UserDataRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.UserData);
        }

        public async Task<Core.CommonModels.Results.OperationResult> PostMessageToDevelopers(string userId, string message)
        {
            var userData = new UserData(AzureTableConstants.UserDataMessage) { Text = message, UserId = userId };
            return await _azureManager.AddEntityAsync(userData);
        }

        public void Dispose()
        {
            //nothing to dispose
        }
    }
}
