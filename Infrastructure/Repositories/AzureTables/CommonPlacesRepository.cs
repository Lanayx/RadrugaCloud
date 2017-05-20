namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.Storage.Table;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    /// <summary>
    ///     The common places repository.
    /// </summary>
    public sealed class CommonPlacesRepository : ICommonPlacesRepository
    {
        /// <summary>
        ///     The _azure manager
        /// </summary>
        private readonly AzureTableStorageManager _azureManager;

        private IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommonPlacesRepository" /> class.
        /// </summary>
        public CommonPlacesRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.CommonPlaces);
        }

        private IUserRepository UserRepository => _userRepository ?? 
            (_userRepository = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>());

        /// <summary>
        ///     Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddAlias(CommonPlaceAlias alias)
        {
            var azureModel = alias.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }

        /// <summary>
        ///     Adds the temp common place.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>Task.</returns>
        public async Task<IdResult> AddCommonPlace(string userId, string commonPlaceAlias, GeoCoordinate coordinate)
        {
            var alias = await GetAlias(commonPlaceAlias);
            if (alias == null)
            {
                throw new ArgumentException("Alias invalid", nameof(commonPlaceAlias));
            }

            var user = await UserRepository.GetUser(userId);
            var uniqueCityId = user.UniqueCityId;
            var commonPlaces = (await GetCommonPlacesForAlias(uniqueCityId, alias)).ToList();
            if (commonPlaces.Any(c => c.IsApprovedCoordinate))
            {
                return new IdResult(OperationResultStatus.Error, "Already exist approved coordinate");
            }

            var approvedResult =
                await TryAddApprovedCommonPlace(uniqueCityId, alias.FullName, coordinate, commonPlaces);
            if (approvedResult != null)
            {
                return approvedResult;
            }

            var commonPlace = new CommonPlace
                                  {
                                      Coordinate = coordinate,
                                      IsApproved = false,
                                      Name = alias.FullName,
                                      SettlementId = uniqueCityId
            };

            var azureModel = commonPlace.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }

        /// <summary>
        ///     Checks the alias exist.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> CheckAliasExist(string aliasName)
        {
            var result =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<CommonPlaceAliasAzure>(
                    AzureTableConstants.CommonPlacesAliasesPartitionKey,
                    aliasName.DecreaseFullName());
            return result != null;
        }

        /// <summary>
        ///     Deletes the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteAlias(string aliasName)
        {
            var aliasAzure =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<CommonPlaceAliasAzure>(
                    AzureTableConstants.CommonPlacesAliasesPartitionKey,
                    aliasName.DecreaseFullName());
            if (aliasAzure == null)
            {
                var message = $"Can't find alias {aliasName}";
                return new OperationResult(OperationResultStatus.Error, message);
            }

            return await _azureManager.DeleteEntityAsync(aliasAzure);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <param name="aliasName">Name of the aliase.</param>
        /// <returns>Task{CommonPlaceAlias}.</returns>
        public async Task<CommonPlaceAlias> GetAlias(string aliasName)
        {
            var result =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<CommonPlaceAliasAzure>(
                    AzureTableConstants.CommonPlacesAliasesPartitionKey,
                    aliasName.DecreaseFullName());
            var alias = result.FromAzureModel();
            return alias;
        }

        /// <summary>
        ///     Gets the aliases.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{CommonPlaceAlias}}.</returns>
        public async Task<IEnumerable<CommonPlaceAlias>> GetAliases(QueryOptions<CommonPlaceAlias> options)
        {
            var tableQuery = options.GenerateTableQuery<CommonPlaceAlias, CommonPlaceAliasAzure>();
            var azureAliases = await _azureManager.GetEntitiesAsync(tableQuery);
            var aliases = azureAliases.Select(a => a.FromAzureModel()).ToList();
            return aliases.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Gets the common place.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <returns>CommonPlace.</returns>
        public async Task<CommonPlace> GetCommonPlaceByAlias(string userId, string commonPlaceAlias)
        {
            var alias = await GetAlias(commonPlaceAlias);
            if (alias == null)
            {
                throw new ArgumentException("Alias invalid", nameof(commonPlaceAlias));
            }

            var user = await UserRepository.GetUser(userId);
            var uniquePlaceId = user.UniqueCityId;
            var commonPlaces = await GetCommonPlacesForAlias(uniquePlaceId, alias);
            var approvedAlias = commonPlaces.FirstOrDefault(p => p.IsApprovedCoordinate);
            return approvedAlias.FromAzureModel();
        }

        /// <summary>
        ///     Gets the common places by alias.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <returns>Task{IEnumerable{CommonPlace}}.</returns>
        public async Task<IEnumerable<CommonPlace>> GetCommonPlacesByAlias(string userId, string commonPlaceAlias)
        {
            var alias = await GetAlias(commonPlaceAlias);
            if (alias == null)
            {
                throw new ArgumentException("Alias invalid", nameof(commonPlaceAlias));
            }

            var user = await UserRepository.GetUser(userId);
            var uniquePlaceId = user.UniqueCityId;
            var commonPlaces = await GetCommonPlacesForAlias(uniquePlaceId, alias);
            return commonPlaces.Select(c => c.FromAzureModel());
        }

        /// <summary>
        ///     Updates the name of the aliase.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateAliaseName(CommonPlaceAlias alias)
        {
            var updatedAlias = alias.ToAzureModel();
            var aliasAzure =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<CommonPlaceAliasAzure>(
                    AzureTableConstants.CommonPlacesAliasesPartitionKey,
                    updatedAlias.ShortName);
            if (aliasAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "No aliase for update");
            }

            updatedAlias.CopyToTableEntity(aliasAzure);
            var updateResult = await _azureManager.UpdateEntityAsync(aliasAzure);
            return updateResult;
        }

        private async Task<IEnumerable<CommonPlaceAzure>> GetCommonPlacesForAlias(
            string uniquePlaceId,
            CommonPlaceAlias alias)
        {
            //  var nameFilter = TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, alias.FullName);
            //var fullFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, uniquePlaceId.GetFilterById());
            
            var commonPlaces =
                await _azureManager.GetEntitiesAsync(new TableQuery<CommonPlaceAzure>().Where(uniquePlaceId.GetFilterById()));
            return commonPlaces.Where(t => t.Name == alias.FullName);
        }

        private async Task<IdResult> TryAddApprovedCommonPlace(
            string uniquePlaceId,
            string fullName,
            GeoCoordinate coordinate,
            List<CommonPlaceAzure> commonPlaces)
        {
            if (commonPlaces.Count < GameConstants.Mission.TemporaryCommonPlaceLimit)
            {
                return null;
            }

            var distances = new List<double>();
            commonPlaces.ForEach(
                c => { distances.Add(c.Coordinate.ConvertToGeoCoordinate().GetDistanceTo(coordinate)); });
            var validDistances = distances.Where(d => d <= GameConstants.Mission.TemporaryCommonPlaceAccuracyRadius);
            if (validDistances.Count() < GameConstants.Mission.TemporaryCommonPlaceLimit)
            {
                return null;
            }

            var commonPlace = new CommonPlace
                                  {
                                      Coordinate = coordinate,
                                      IsApproved = true,
                                      Name = fullName,
                                      SettlementId = uniquePlaceId
                                  };

            var azureModel = commonPlace.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }
    }
}