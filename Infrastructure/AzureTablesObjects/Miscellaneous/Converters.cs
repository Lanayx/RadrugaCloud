namespace Infrastructure.AzureTablesObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.AuthorizationModels;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;
    using Core.Tools;
    using Core.Tools.CopyHelper;

    using Infrastructure.AzureTablesObjects.TableEntities;

    /// <summary>
    ///     Class Converters
    /// </summary>
    public static class Converters
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts MissionDraftAzure to MissionDraft..
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>MissionDraft.</returns>
        public static MissionDraft FromAzureModel(this MissionDraftAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new MissionDraft();
            azureModel.CopyTo(domainModel);
            SetMissionDependentPropertiesFromAzuremodel(domainModel, azureModel);
            return domainModel;
        }

        /// <summary>
        ///     Converts MissionAzure to Mission..
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>Mission.</returns>
        public static Mission FromAzureModel(this MissionAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new Mission();
            azureModel.CopyTo(domainModel);
            SetMissionDependentPropertiesFromAzuremodel(domainModel, azureModel);
            return domainModel;
        }

        public static Mission ConvertToMission(List<MissionAzure> relatedEntities, bool needExpand)
        {
            var missionAzure = relatedEntities.FirstOrDefault(m => m.IsMissionEntity);
            var mission = missionAzure.FromAzureModel();
            if (mission == null)
            {
                return null;
            }

            if (needExpand)
            {
                mission.Hints =
                    relatedEntities.Where(h => h.IsHintLink).Select(h => new Hint
                    {
                        Id = h.HintId,
                        Text = h.HintText,
                        Score = h.HintScore ?? 0,
                        Type = (HintType)(h.HintType ?? 0)
                    }).ToList();

                mission.PersonQualities =
                    relatedEntities.Where(u => u.IsPersonQualityLink)
                        .Select(
                            u =>
                            new PersonQualityIdWithScore
                                {
                                    Score = u.PersonQualityScore ?? 0,
                                    PersonQualityId = u.PersonQualityId
                                })
                        .ToList();                
            }

            return mission;
        }

        /// <summary>
        /// Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>MissionSet.</returns>
        public static MissionSet FromAzureModel(this MissionSetAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new MissionSet();
            azureModel.CopyTo(domainModel);
            domainModel.AgeFrom = (byte?)azureModel.AgeFrom;
            domainModel.AgeTo = (byte?)azureModel.AgeTo;
            return domainModel;
        }

        /// <summary>
        ///     Converts MissionRequestAzure to MissionRequest..
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>MissionRequest.</returns>
        public static MissionRequest FromAzureModel(this MissionRequestAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new MissionRequest();
            azureModel.CopyTo(domainModel);
            MissionRequestStatus status;
            if (Enum.TryParse(azureModel.Status, out status))
            {
                domainModel.Status = status;
            }

            domainModel.StarsCount = (byte?)azureModel.StarsCount;
            domainModel.Proof = new MissionProof
                                    {
                                        ImageUrls = azureModel.ProofImageUrls?.SplitStringByDelimiter(), 
                                        CreatedText = azureModel.CreatedText,
                                        Coordinates = azureModel.ProofCoordinates?.SplitStringByDelimiter(CommonConstants.Delimiter)?.Select(coord => coord.ConvertToGeoCoordinate()).ToList(),
                                        TimeElapsed = azureModel.TimeElapsed,
                                        NumberOfTries = (ushort?)azureModel.NumberOfTries
                                    };
            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>AppErrorInfo.</returns>
        public static AppCounters FromAzureModel(this AppCountersAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new AppCounters();
            azureModel.CopyTo(domainModel);
            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>AppErrorInfo.</returns>
        public static AppErrorInfo FromAzureModel(this AppErrorInfoAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new AppErrorInfo();
            azureModel.CopyTo(domainModel);
            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>KindAction.</returns>
        public static KindAction FromAzureModel(this KindActionAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new KindAction();
            azureModel.CopyTo(domainModel);
            domainModel.Likes = azureModel.Likes.SplitStringByDelimiter();
            domainModel.Dislikes = azureModel.Dislikes.SplitStringByDelimiter();
            return domainModel;
        }

        /// <summary>
        ///     Converts PersonQualityAzure to PersonQuality.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>PersonQuality.</returns>
        public static PersonQuality FromAzureModel(this PersonQualityAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new PersonQuality();
            azureModel.CopyTo(domainModel);
            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>User.</returns>
        public static User FromAzureModel(this UserAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new User();
            azureModel.CopyTo(domainModel);
            domainModel.CompletedMissionIds = azureModel.CompletedMissionIdsString.SplitStringByDelimiter();
            domainModel.FailedMissionIds = azureModel.FailedMissionIdsString.SplitStringByDelimiter();
            domainModel.ActiveMissionIds = azureModel.GetActiveMissions();
            domainModel.MissionSetIds = azureModel.GetMissionSets();
            domainModel.ActiveMissionSetIds = azureModel.ActiveMissionSetIdsString.SplitStringByDelimiter();
            domainModel.CompletedMissionSetIds = azureModel.CompletedMissionSetIdsString.SplitStringByDelimiter();
            domainModel.BoughtHintIds = azureModel.BoughtHintIdsString.SplitStringByDelimiter();
            domainModel.Sex = (Sex?)azureModel.Sex;
            domainModel.KindScale = (byte?)azureModel.KindScale;
            domainModel.Level = (byte?)azureModel.Level;
            domainModel.LevelPoints = (ushort?)azureModel.LevelPoints;
            domainModel.HomeCoordinate = azureModel.HomeCoordinate.ConvertToGeoCoordinate();
            domainModel.BaseEastCoordinate = azureModel.BaseEastCoordinate.ConvertToGeoCoordinate();
            domainModel.BaseNorthCoordinate = azureModel.BaseNorthCoordinate.ConvertToGeoCoordinate();
            domainModel.BaseSouthCoordinate = azureModel.BaseSouthCoordinate.ConvertToGeoCoordinate();
            domainModel.BaseWestCoordinate = azureModel.BaseWestCoordinate.ConvertToGeoCoordinate();
            domainModel.RadarCoordinate = azureModel.RadarCoordinate.ConvertToGeoCoordinate();
            domainModel.OutpostCoordinate = azureModel.OutpostCoordinate.ConvertToGeoCoordinate();
            domainModel.CoinsCount = azureModel.CoinsCount;
            return domainModel;
        }


        public static User ConvertToUser(IList<UserAzure> userRelatedEntities, bool needExpand)
        {
            var userAzure = userRelatedEntities.FirstOrDefault(u => u.IsUserEntity);
            var user = userAzure.FromAzureModel();
            if (user == null)
            {
                return null;
            }

            if (needExpand)
            {
                user.PersonQualitiesWithScores =
                    userRelatedEntities.Where(u => u.IsPersonQualityLink)
                        .Select(
                            u =>
                            new PersonQualityIdWithScore
                            {
                                Score = u.PersonQualityScore ?? 0,
                                PersonQualityId = u.PersonQualityId
                            }).ToList();
            }

            return user;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>CommonPlace.</returns>
        public static CommonPlace FromAzureModel(this CommonPlaceAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new CommonPlace();
            azureModel.CopyTo(domainModel);
            domainModel.Coordinate = azureModel.Coordinate.ConvertToGeoCoordinate();
            domainModel.IsApproved = azureModel.IsApprovedCoordinate;

            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>CommonPlaceAliace.</returns>
        public static CommonPlaceAlias FromAzureModel(this CommonPlaceAliasAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new CommonPlaceAlias();
            azureModel.CopyTo(domainModel);

            return domainModel;
        }

        /// <summary>
        ///     Froms the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>UserIdentity.</returns>
        public static UserIdentity FromAzureModel(this UserIdentityAzure azureModel)
        {
            var vkCounters = new VkCounters
                                 {
                                     Audios = (uint?)azureModel.Vk_Counters_Audios, 
                                     Followers = (uint?)azureModel.Vk_Counters_Followers, 
                                     Friends = (uint?)azureModel.Vk_Counters_Friends, 
                                     Photos = (uint?)azureModel.Vk_Counters_Photos, 
                                     Videos = (uint?)azureModel.Vk_Counters_Videos, 
                                 };
            var vkIdentity = new VkIdentity
                                 {
                                     CityId = (uint?)azureModel.Vk_CityId, 
                                     CountryId = (uint?)azureModel.Vk_CountryId, 
                                     Id = (uint)azureModel.Vk_Id, 
                                     UniversityId = (uint?)azureModel.Vk_UniversityId, 
                                     Counters = vkCounters
                                 };
            var identity = new UserIdentity
                               {
                                   HashType = (HashType)azureModel.HashType, 
                                   HashedPassword = azureModel.HashedPassword, 
                                   Id = azureModel.Id, 
                                   LoginEmail = azureModel.LoginEmail, 
                                   VkIdentity = vkIdentity,
                                   EmailConfirmationCode = azureModel.EmailConfirmationCode,
                                   EmailConfirmationAttempts = (byte)azureModel.EmailConfirmationAttempts,
                                   Device = azureModel.Device
                               };
            return identity;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>Question.</returns>
        public static Question FromAzureModel(this QuestionAzure azureModel)
        {
            var question = new Question();
            azureModel.CopyTo(question);
            return question;
        }

        /// <summary>
        ///     Converts HintRequestAzure to HintRequest.
        /// </summary>
        /// <param name="azureModel">The azure model.</param>
        /// <returns>HintRequest.</returns>
        public static HintRequest FromAzureModel(this HintRequestAzure azureModel)
        {
            if (azureModel == null)
            {
                return null;
            }

            var domainModel = new HintRequest();
            azureModel.CopyTo(domainModel);
            domainModel.Status = (HintRequestStatus)azureModel.Status;
            return domainModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>QuestionAzure.</returns>
        public static QuestionAzure ToAzureModel(this Question question)
        {
            var azureModel = new QuestionAzure();
            question.CopyTo(azureModel);
            return azureModel;
        }

        /// <summary>
        ///     Converts MissionDraft to MissionDraftAzure.
        /// </summary>
        /// <param name="missionDraft">The mission draft.</param>
        /// <returns>MissionDraftAzure.</returns>
        public static MissionDraftAzure ToAzureModel(this MissionDraft missionDraft)
        {
            var azureModel = new MissionDraftAzure();
            missionDraft.CopyTo(azureModel);
            SetMissionDependentPropertiesToAzureModel(missionDraft, azureModel);
            return azureModel;
        }

        /// <summary>
        ///     Converts Mission to MissionAzure.
        /// </summary>
        /// <param name="mission">The mission draft.</param>
        /// <returns>MissionDraftAzure.</returns>
        public static MissionAzure ToAzureModel(this Mission mission)
        {
            var azureModel = new MissionAzure();
            mission.CopyTo(azureModel);
            SetMissionDependentPropertiesToAzureModel(mission, azureModel);
            return azureModel;
        }

        /// <summary>
        /// To the azure model.
        /// </summary>
        /// <param name="missionSet">The mission set.</param>
        /// <returns>MissionSetAzure.</returns>
        public static MissionSetAzure ToAzureModel(this MissionSet missionSet)
        {
            var azureModel = new MissionSetAzure();
            missionSet.CopyTo(azureModel);
            azureModel.AgeFrom = missionSet.AgeFrom;
            azureModel.AgeTo = missionSet.AgeTo;
            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns>KindActionAzure.</returns>
        public static KindActionAzure ToAzureModel(this KindAction kindAction)
        {
            var azureModel = new KindActionAzure();
            kindAction.CopyTo(azureModel);
            azureModel.Likes = kindAction.Likes.JoinToString();
            azureModel.Dislikes = kindAction.Dislikes.JoinToString();
            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="appErrorInfo">The application error information.</param>
        /// <returns>AppErrorInfoAzure.</returns>
        public static AppErrorInfoAzure ToAzureModel(this AppErrorInfo appErrorInfo)
        {
            var azureModel = new AppErrorInfoAzure();
            appErrorInfo.CopyTo(azureModel);
            return azureModel;
        }


        /// <summary>
        /// To the azure model.
        /// </summary>
        /// <param name="appCounters">The application counters.</param>
        /// <returns></returns>
        public static AppCountersAzure ToAzureModel(this AppCounters appCounters)
        {
            var azureModel = new AppCountersAzure();
            appCounters.CopyTo(azureModel);
            return azureModel;
        }

        /// <summary>
        ///     Converts PersonQuality to PersonQualityAzure.
        /// </summary>
        /// <param name="personQuality">Type of the person.</param>
        /// <returns>PersonQualityAzure.</returns>
        public static PersonQualityAzure ToAzureModel(this PersonQuality personQuality)
        {
            var azureModel = new PersonQualityAzure();
            personQuality.CopyTo(azureModel);
            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>UserIdentityAzure.</returns>
        public static UserIdentityAzure ToAzureModel(this UserIdentity userIdentity)
        {
            var azureModel = new UserIdentityAzure
                                 {
                                     Id = userIdentity.Id, 
                                     HashType = (int)userIdentity.HashType, 
                                     HashedPassword = userIdentity.HashedPassword, 
                                     LoginEmail = userIdentity.LoginEmail, 
                                     EmailConfirmationCode = userIdentity.EmailConfirmationCode,
                                     EmailConfirmationAttempts = userIdentity.EmailConfirmationAttempts,
                                     Device = userIdentity.Device
                                 };
            if (userIdentity.VkIdentity != null)
            {
                var vk = userIdentity.VkIdentity;
                azureModel.Vk_Id = (int)vk.Id;
                azureModel.Vk_UniversityId = (int?)vk.UniversityId;
                azureModel.Vk_CityId = (int?)vk.CityId;
                azureModel.Vk_CountryId = (int?)vk.CountryId;
                if (vk.Counters != null)
                {
                    var count = vk.Counters;
                    azureModel.Vk_Counters_Audios = (int?)count.Audios;
                    azureModel.Vk_Counters_Followers = (int?)count.Followers;
                    azureModel.Vk_Counters_Friends = (int?)count.Friends;
                    azureModel.Vk_Counters_Photos = (int?)count.Photos;
                    azureModel.Vk_Counters_Videos = (int?)count.Videos;
                }
            }

            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>UserAzure.</returns>
        public static UserAzure ToAzureModel(this User user)
        {
            var azureModel = new UserAzure();
            user.CopyTo(azureModel);
            azureModel.CompletedMissionIdsString = user.CompletedMissionIds.JoinToString();
            azureModel.FailedMissionIdsString = user.FailedMissionIds.JoinToString();
            azureModel.SetActiveMissions(user.ActiveMissionIds);
            azureModel.Sex = (int?)user.Sex;
            azureModel.KindScale = user.KindScale;
            azureModel.Level = user.Level;
            azureModel.LevelPoints = user.LevelPoints;
            azureModel.HomeCoordinate = user.HomeCoordinate.ConvertToAzureCoordinate();
            azureModel.BaseEastCoordinate = user.BaseEastCoordinate.ConvertToAzureCoordinate();
            azureModel.BaseNorthCoordinate = user.BaseNorthCoordinate.ConvertToAzureCoordinate();
            azureModel.BaseSouthCoordinate = user.BaseSouthCoordinate.ConvertToAzureCoordinate();
            azureModel.BaseWestCoordinate = user.BaseWestCoordinate.ConvertToAzureCoordinate();
            azureModel.RadarCoordinate = user.RadarCoordinate.ConvertToAzureCoordinate();
            azureModel.OutpostCoordinate = user.OutpostCoordinate.ConvertToAzureCoordinate();
            azureModel.SetMissionSets(user.MissionSetIds);
            azureModel.ActiveMissionSetIdsString = user.ActiveMissionSetIds.JoinToString();
            azureModel.CompletedMissionSetIdsString = user.CompletedMissionSetIds.JoinToString();           
            azureModel.BoughtHintIdsString = user.BoughtHintIds.JoinToString();
            azureModel.CoinsCount = user.CoinsCount;
            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="commonPlace">The common place.</param>
        /// <returns>CommonPlaceAzure.</returns>
        public static CommonPlaceAzure ToAzureModel(this CommonPlace commonPlace)
        {
            var azureModel = new CommonPlaceAzure(commonPlace.Name, commonPlace.IsApproved)
                                 {
                                     SettlementId = commonPlace.SettlementId, 
                                     Coordinate = commonPlace.Coordinate.ConvertToAzureCoordinate()
                                 };

            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="commonPlace">The common place.</param>
        /// <returns>CommonPlaceAliaseAzure.</returns>
        public static CommonPlaceAliasAzure ToAzureModel(this CommonPlaceAlias commonPlace)
        {
            var azureModel = new CommonPlaceAliasAzure();
            commonPlace.CopyTo(azureModel);

            return azureModel;
        }

        /// <summary>
        ///     To the azure model.
        /// </summary>
        /// <param name="missionRequest">The mission request.</param>
        /// <returns>MissionRequestAzure</returns>
        public static MissionRequestAzure ToAzureModel(this MissionRequest missionRequest)
        {
            var azureModel = new MissionRequestAzure();
            missionRequest.CopyTo(azureModel);
            azureModel.StarsCount = missionRequest.StarsCount;
            azureModel.Status = missionRequest.Status.ToString();
            //set proofs
            azureModel.ProofImageUrls = missionRequest.Proof.ImageUrls?.JoinToString();
            azureModel.ProofCoordinates = missionRequest.Proof.Coordinates?.Select(coord => coord.ConvertToAzureCoordinate())
                .JoinToString(CommonConstants.Delimiter);
            azureModel.CreatedText = missionRequest.Proof.CreatedText;
            azureModel.TimeElapsed = missionRequest.Proof.TimeElapsed;
            azureModel.NumberOfTries = missionRequest.Proof.NumberOfTries;
            return azureModel;
        }

        /// <summary>
        ///     Convert hint request to the azure model.
        /// </summary>
        /// <param name="hintRequest">The hint request.</param>
        /// <returns>HintRequestAzure</returns>
        public static HintRequestAzure ToAzureModel(this HintRequest hintRequest)
        {
            var azureModel = new HintRequestAzure();      
            hintRequest.CopyTo(azureModel);
            azureModel.Status = (int)hintRequest.Status;
            return azureModel;            
        }

        #endregion

        #region Methods

        private static void SetMissionDependentPropertiesFromAzuremodel(Mission domainModel, MissionAzure azureModel)
        {
            domainModel.AgeFrom = (byte?)azureModel.AgeFrom;
            domainModel.AgeTo = (byte?)azureModel.AgeTo;
            domainModel.Difficulty = (byte)azureModel.Difficulty;
            domainModel.ExecutionType = (ExecutionType)azureModel.ExecutionType;
            domainModel.DependsOn = azureModel.DependsOn.SplitStringByDelimiter();
            domainModel.AnswersCount = (byte?)azureModel.AnswersCount;
            domainModel.TriesFor1Star = (byte?)azureModel.TriesFor1Star;
            domainModel.TriesFor2Stars = (byte?)azureModel.TriesFor2Stars;
            domainModel.TriesFor3Stars = (byte?)azureModel.TriesFor3Stars;
            domainModel.NumberOfPhotos = (byte?)azureModel.NumberOfPhotos;
            domainModel.CalculationFunctionParameters =
                azureModel.CalculationFunctionParameters.SplitStringByDelimiter();
            domainModel.CorrectAnswers = azureModel.CorrectAnswersString;
        }

        private static void SetMissionDependentPropertiesToAzureModel(Mission mission, MissionAzure azureModel)
        {
            azureModel.AgeFrom = mission.AgeFrom;
            azureModel.AgeTo = mission.AgeTo;
            azureModel.Difficulty = mission.Difficulty;
            azureModel.ExecutionType = (int)mission.ExecutionType;
            azureModel.DependsOn = mission.DependsOn.JoinToString();
            azureModel.AnswersCount = mission.AnswersCount;
            azureModel.TriesFor1Star = mission.TriesFor1Star;
            azureModel.TriesFor2Stars = mission.TriesFor2Stars;
            azureModel.TriesFor3Stars = mission.TriesFor3Stars;
            azureModel.NumberOfPhotos = mission.NumberOfPhotos;
            azureModel.CalculationFunctionParameters = mission.CalculationFunctionParameters.JoinToString();            
            azureModel.CorrectAnswersString = mission.CorrectAnswers;
        }

        #endregion
    }
}