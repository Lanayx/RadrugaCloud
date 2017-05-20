namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Infrastructure.InfrastructureTools;

    /// <summary>
    ///     The common places repository.
    /// </summary>
    public sealed class CommonPlacesRepository : ICommonPlacesRepository
    {
        private readonly List<CommonPlaceAlias> _commonPlaceAliases = new List<CommonPlaceAlias>();

        private readonly List<CommonPlace> _commonPlaces = new List<CommonPlace>();

        private readonly UserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommonPlacesRepository" /> class.
        /// </summary>
        public CommonPlacesRepository()
        {
            _userRepository = new UserRepository();

            if (!_commonPlaces.Any())
            {
                // Approved common places
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = true,
                            Name = "CM 1",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                    {
                        IsApproved = true,
                        Name = "ForCommonPlaceHint",
                        Coordinate = new GeoCoordinate(24, 77),
                        SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                    });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = true,
                            Name = "CM 1",
                            Coordinate = new GeoCoordinate(50, 50),
                            SettlementId = "3f572659-9ea8-42fc-99bf-2f8b5d56f9d1"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = true,
                            Name = "CM 2",
                            Coordinate = new GeoCoordinate(24, 15),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = true,
                            Name = "CM 2",
                            Coordinate = new GeoCoordinate(80, 80),
                            SettlementId = "3f572659-9ea8-42fc-99bf-2f8b5d56f9d1"
                        });
                // Temp common places
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 3",
                            Coordinate = new GeoCoordinate(10, 10),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 3",
                            Coordinate = new GeoCoordinate(11, 11),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 3",
                            Coordinate = new GeoCoordinate(12, 12),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
                _commonPlaces.Add(
                    new CommonPlace
                        {
                            IsApproved = false,
                            Name = "CM 4",
                            Coordinate = new GeoCoordinate(0, 0),
                            SettlementId = "04c00ab8-696b-47ae-ba7a-97dfe299d1d2"
                        });
            }

            if (!_commonPlaceAliases.Any())
            {
                _commonPlaceAliases.Add(new CommonPlaceAlias("CM 1"));
                _commonPlaceAliases.Add(new CommonPlaceAlias("CM 2"));
                _commonPlaceAliases.Add(new CommonPlaceAlias("CM 3"));
                _commonPlaceAliases.Add(new CommonPlaceAlias("CM 4"));
                _commonPlaceAliases.Add(new CommonPlaceAlias("ForCommonPlaceHint"));
            }
        }

        /// <summary>
        ///     Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddAlias(CommonPlaceAlias alias)
        {
            await Task.Factory.StartNew(() => _commonPlaceAliases.Add(alias));
            return new IdResult(alias.ShortName);
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
                throw new ArgumentException("Alias invalid", "commonPlaceAlias");
            }

            var user = await _userRepository.GetUser(userId);
            var uniqueCityId = user.UniqueCityId;
            var commonPlaces = GetCommonPlacesForAlias(uniqueCityId, alias).ToList();
            if (commonPlaces.Any(c => c.IsApproved))
            {
                return new IdResult(OperationResultStatus.Error, "Already exist approved coordinate");
            }

            var approvedResult = TryAddApprovedCommonPlace(uniqueCityId, alias.FullName, coordinate, commonPlaces);
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

            _commonPlaces.Add(commonPlace);
            return new IdResult(uniqueCityId);
        }

        /// <summary>
        ///     Checks the alias exist.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> CheckAliasExist(string aliasName)
        {
            return
                await
                Task<bool>.Factory.StartNew(
                    () => _commonPlaceAliases.Any(a => a.ShortName == aliasName.DecreaseFullName()));
        }

        /// <summary>
        ///     Deletes the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteAlias(string aliasName)
        {
            await Task.FromResult(_commonPlaceAliases.RemoveAll(m => m.ShortName == aliasName.DecreaseFullName()));
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{CommonPlaceAlias}.</returns>
        public async Task<CommonPlaceAlias> GetAlias(string aliasName)
        {
            var alias =
                await
                Task<CommonPlaceAlias>.Factory.StartNew(
                    () => _commonPlaceAliases.FirstOrDefault(a => a.ShortName == aliasName.DecreaseFullName()));
            return alias;
        }

        /// <summary>
        ///     Gets the aliases.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{CommonPlaceAlias}}.</returns>
        public async Task<IEnumerable<CommonPlaceAlias>> GetAliases(QueryOptions<CommonPlaceAlias> options)
        {
            if (options == null)
            {
                return await Task.Factory.StartNew(() => _commonPlaceAliases.AsEnumerable());
            }

            return await Task.Factory.StartNew(
                () =>
                    {
                        var missions = options.SimpleApply(_commonPlaceAliases.AsQueryable()).AsEnumerable();
                        return missions;
                    });
        }

        /// <summary>
        ///     Gets the common place.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <returns>CommonPlace.</returns>
        /// <exception cref="System.ArgumentException">Alias invalid;commonPlaceAlias</exception>
        public async Task<CommonPlace> GetCommonPlaceByAlias(string userId, string commonPlaceAlias)
        {
            var alias = await GetAlias(commonPlaceAlias);
            if (alias == null)
            {
                throw new ArgumentException("Alias invalid", "commonPlaceAlias");
            }

            var user = await _userRepository.GetUser(userId);
            var uniquePlaceId = user.UniqueCityId;
            var commonPlaces = GetCommonPlacesForAlias(uniquePlaceId, alias);
            var approvedAlias = commonPlaces.FirstOrDefault(p => p.IsApproved);
            return approvedAlias;
        }

        /// <summary>
        ///     Gets the common places by alias.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <returns>Task{IEnumerable{CommonPlace}}.</returns>
        /// <exception cref="System.ArgumentException">Alias invalid;commonPlaceAlias</exception>
        public async Task<IEnumerable<CommonPlace>> GetCommonPlacesByAlias(string userId, string commonPlaceAlias)
        {
            var alias = await GetAlias(commonPlaceAlias);
            if (alias == null)
            {
                throw new ArgumentException("Alias invalid", "commonPlaceAlias");
            }

            var user = await _userRepository.GetUser(userId);
            var uniquePlaceId = user.UniqueCityId;
            var commonPlaces = GetCommonPlacesForAlias(uniquePlaceId, alias);
            return commonPlaces;
        }

        /// <summary>
        ///     Updates the name of the aliase.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateAliaseName(CommonPlaceAlias alias)
        {
            var currentAlias =
                await
                Task.Factory.StartNew(() => _commonPlaceAliases.FirstOrDefault(c => c.ShortName == alias.ShortName));
            if (currentAlias == null)
            {
                return new OperationResult(OperationResultStatus.Error, "No aliase for update");
            }

            currentAlias.FullName = alias.FullName;
            return new OperationResult(OperationResultStatus.Success);
        }

        private List<CommonPlace> GetCommonPlacesForAlias(string uniquePlaceId, CommonPlaceAlias @alias)
        {
            return _commonPlaces.Where(c => c.SettlementId == uniquePlaceId && c.Name == alias.FullName).ToList();
        }

        private IdResult TryAddApprovedCommonPlace(
            string uniquePlaceId,
            string fullName,
            GeoCoordinate coordinate,
            List<CommonPlace> commonPlaces)
        {
            if (commonPlaces.Count() < GameConstants.Mission.TemporaryCommonPlaceLimit)
            {
                return null;
            }

            var distances = new List<double>();
            commonPlaces.ForEach(c => { distances.Add(c.Coordinate.GetDistanceTo(coordinate)); });
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

            _commonPlaces.Add(commonPlace);
            return new IdResult(uniquePlaceId);
        }
    }
}