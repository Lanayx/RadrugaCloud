namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools.CopyHelper;
    using InfrastructureTools;

    /// <summary>
    ///     The mission repository.
    /// </summary>
    public sealed class MissionRepository : IMissionRepository
    {
        /// <summary>
        ///     The all missions.
        /// </summary>
        private readonly List<Mission> _allMissions = new List<Mission>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRepository" /> class.
        /// </summary>
        public MissionRepository()
        {
            if (!_allMissions.Any())
            {
                _allMissions.Add(
                    new Mission
                        {
                            Id = "Mission1",
                            Name = "First Mission",
                            Difficulty = 2,
                            Description = "In this mission you need to put your ass in the air.",
                            PhotoUrl = "http://www.bubblews.com/assets/images/news/283489062_1358762112.png",
                            Hints = new List<Hint>
                                        {
                                            new Hint
                                            {
                                                Id = "TextHint",
                                                Text = "The text hint for first mission",
                                                Type = HintType.Text,
                                                Score = 2
                                            },
                                            new Hint
                                            {
                                                Id = "DirectionHint",
                                                Text = "This text should not be displayed",
                                                Type = HintType.Direction,
                                                Score = 3
                                            }
                                        },                                                                
                        
                    });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "Mission2",
                            Name = "Second Mission",
                            Difficulty = 3,
                            Description = "In this mission you have to byte your elbow.",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "Mission3",
                            Name = "Third Mission",
                            Difficulty = 9,
                            Description = "In this mission you have to sleep",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                //Unique missions
                _allMissions.Add(
                    new Mission
                        {
                            Id = "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e",
                            Name = "Outpost",
                            Difficulty = 1,
                            Description = "In this mission you have to find outpost",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            DependsOn =
                                new List<string>
                                    {
                                        "5823973e-ac62-4efe-9c5d-1755df874d99",
                                        "71130d2e-e513-4b29-ad12-7a368f4d927a",
                                        "b23f79fe-08d3-4d24-a293-51f67e2131be"
                                    },
                            MissionSetId = "OutpostSet"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "d061155a-9504-498d-a6e7-bcc20c295cde",
                            Name = "Show yourself",
                            Difficulty = 1,
                            Description = "In this mission you have to show yourself",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "5823973e-ac62-4efe-9c5d-1755df874d99",
                            Name = "Command point",
                            Difficulty = 1,
                            Description = "In this mission you have to go home",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            MissionSetId = GameConstants.MissionSet.FirstSetId
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "8ab5fa9b-15a7-49ec-8d5f-2a47d4affd52",
                            Name = "Healthy Spirit1",
                            Difficulty = 1,
                            Description = "In this mission you have  show your spirit",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "71130d2e-e513-4b29-ad12-7a368f4d927a",
                            Name = "Your Base",
                            Difficulty = 1,
                            Description = "In this mission you have to go around your base",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            DependsOn = new List <string> { "5823973e-ac62-4efe-9c5d-1755df874d99" }
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "19e18c3f-2daa-4e18-bf32-2a1f77fdc73f",
                            Name = "Help Choise",
                            Difficulty = 1,
                            Description = "In this mission you have to make a choice",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "b23f79fe-08d3-4d24-a293-51f67e2131be",
                            Name = "Radar",
                            Difficulty = 1,
                            Description = "In this mission you have to find the radar",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            DependsOn =
                                new List<string>
                                    {
                                        "5823973e-ac62-4efe-9c5d-1755df874d99",
                                        "71130d2e-e513-4b29-ad12-7a368f4d927a"
                                    },
                            MissionSetId = "RadarSet"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "524b9f98-88b2-460a-ae3f-e15df4cef2ae",
                            Name = "Censored",
                            Difficulty = 1,
                            Description = "In this mission you have to create awful poem",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "2c187275-97c0-4dcf-a1f0-89d7018a03e2",
                            Name = "Friends base",
                            Difficulty = 1,
                            Description = "In this mission you have to penetrate friends base",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            DependsOn = new List<string> { "5823973e-ac62-4efe-9c5d-1755df874d99" }
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "3be2d03c-f100-42be-bc9f-54166e108d43",
                            Name = "Perpetum mobile",
                            Difficulty = 1,
                            Description = "In this mission you have to move all the time",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });

                //Autoapproved missions
                _allMissions.Add(
                    new Mission
                        {
                            Id = "2ddf9168-b030-4b6c-a038-72593e7a75f1",
                            Name = "8 лет поисков",
                            Difficulty = 2,
                            Description = "In this mission you have to find Pushkin",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "евгений онегин",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            ExecutionType = ExecutionType.RightAnswer,
                            ExactAnswer = true
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "2ddf9168-b030-4b6c-a038-72593e7a75f2",
                            Name = "8 лет поисков",
                            Difficulty = 2,
                            Description = "In this mission you have to find Pushkin",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "евгений онегин",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            ExecutionType = ExecutionType.RightAnswer,
                            ExactAnswer = true
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "3c27b903-cf03-447e-9682-f756e70ca908",
                            Name = "Выбор за тобой",
                            Difficulty = 2,
                            Description = "In this mission you have to Star wars",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "люк скайуокер,дарт вейдер",
                            TriesFor1Star = 5,
                            TriesFor2Stars = 3,
                            TriesFor3Stars = 1,
                            AnswersCount = 1,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                            Name = "Каково космонавту?",
                            Difficulty = 2,
                            Description = "In this mission you have to Star wars",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "тяжести;центро",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            AnswersCount = 2,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd6d",
                            Name = "Каково космонавту?",
                            Difficulty = 2,
                            Description = "In this mission you have to Star wars",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "тяжести;центро",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            AnswersCount = 2,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd7d",
                            Name = "Каково космонавту?",
                            Difficulty = 2,
                            Description = "In this mission you have to Star wars",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "тяжести;центро",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            AnswersCount = 2,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd8d",
                            Name = "Каково космонавту?",
                            Difficulty = 2,
                            Description = "In this mission you have to Star wars",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "тяжести;центро",
                            TriesFor1Star = 4,
                            TriesFor2Stars = 2,
                            TriesFor3Stars = 1,
                            AnswersCount = 2,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                        {
                            Id = "fe9b1f3a-059b-4b4e-8f08-0a22a21a1ed0",
                            Name = "Секрет силы",
                            Difficulty = 2,
                            Description = "In this mission you have to find Brus Li",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            CorrectAnswers = "изучает/один/удар/десять/тысяч/раз,изучает/1/удар/10000/раз",
                            TriesFor1Star = 5,
                            TriesFor2Stars = 3,
                            TriesFor3Stars = 1,
                            AnswersCount = 1,
                            ExecutionType = ExecutionType.RightAnswer
                        });
                _allMissions.Add(
                    new Mission
                    {
                        Id = "68d75498-e44f-4bf8-a2ee-13b4998c1725",
                        Name = "Общее место",
                        Difficulty = 2,
                        Description = "Найти это место",
                        PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        TriesFor1Star = 5,
                        TriesFor2Stars = 3,
                        TriesFor3Stars = 1,
                        ExecutionType = ExecutionType.CommonPlace,
                        AccuracyRadius = 200,
                        CommonPlaceAlias = "CM 1"
                    });
                _allMissions.Add(
                    new Mission
                    {
                        Id = "68d75498-e44f-4bf8-a2ee-13b4998c17d3",
                        Name = "Общее место",
                        Difficulty = 2,
                        Description = "Найти это место",
                        PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        TriesFor1Star = 50,
                        TriesFor2Stars = 30,
                        TriesFor3Stars = 1,
                        ExecutionType = ExecutionType.CommonPlace,
                        AccuracyRadius = 200,
                        CommonPlaceAlias = "CM 1"
                    });
                _allMissions.Add(
                    new Mission
                    {
                        Id = "68d75498-e44f-4bf8-a2ee-13b4998c17d4",
                        Name = "Общее место",
                        Difficulty = 2,
                        Description = "Найти это место",
                        PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        TriesFor1Star = 50,
                        TriesFor2Stars = 25,
                        TriesFor3Stars = 1,
                        ExecutionType = ExecutionType.CommonPlace,
                        AccuracyRadius = 200,
                        CommonPlaceAlias = "CM 3"
                    });
                _allMissions.Add(
                    new Mission
                    {
                        Id = "68d75498-e44f-4bf8-a2ee-13b4998c17d5",
                        Name = "Общее место",
                        Difficulty = 2,
                        Description = "Найти это место",
                        PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        TriesFor1Star = 5,
                        TriesFor2Stars = 3,
                        TriesFor3Stars = 1,
                        ExecutionType = ExecutionType.CommonPlace,
                        AccuracyRadius = 200,
                        CommonPlaceAlias = "CM 4"
                    });
                _allMissions.Add(
                   new Mission
                   {
                       Id = "68d75498-e44f-4bf8-a2ee-13b4998c17d6",
                       Name = "Тест подсказки с общим местом",
                       Difficulty = 2,
                       Description = "Найти это место",                     
                       ExecutionType = ExecutionType.CommonPlace,           
                       CommonPlaceAlias = "ForCommonPlaceHint",
                       Hints = new List<Hint>()
                                   {
                                        new Hint
                                        {
                                            Id = "CommonPlaceHint",
                                            Type = HintType.Coordinate,
                                            Score = 4
                                        }
                                   }
                   });
                _allMissions.Add(
                   new Mission
                   {
                       Id = "68d75498-e44f-4bf8-a2ee-13b4998c17d7",
                       Name = "Тест подсказки с несуществующим общим местом",
                       Difficulty = 2,
                       Description = "Найти это место",                     
                       ExecutionType = ExecutionType.CommonPlace,             
                       CommonPlaceAlias = "NonexistentCommonPlace",
                       Hints = new List<Hint>()
                                   {
                                        new Hint
                                        {
                                            Id = "NonexistentCommonPlaceHint",
                                            Type = HintType.Coordinate,
                                            Score = 4
                                        }
                                   }
                   });

            }
        }

        /// <summary>
        ///     The add mission.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IdResult> AddMission(Mission mission)
        {
            mission.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allMissions.Add(mission));
            return new IdResult(mission.Id);
        }

        /// <summary>
        ///     The delete mission.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> DeleteMission(string id)
        {
            await Task.FromResult(_allMissions.RemoveAll(m => m.Id == id));
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        ///     The get mission.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<Mission> GetMission(string id)
        {
            return Task.Factory.StartNew(() => _allMissions.Find(mission => mission.Id == id).Copy());
        }

        /// <summary>
        ///     The get missions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<Mission>> GetMissions(QueryOptions<Mission> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allMissions);
            }

            return Task.Factory.StartNew(() => options.SimpleApply(_allMissions.AsQueryable()).ToList());
        }

        /// <summary>
        ///     Clears the links to mission set.
        /// </summary>
        /// <param name="missionIds">The mission ids.</param>
        /// <param name="missionSetId">The mission set id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> SetMissionSetForMissions(List<string> missionIds, string missionSetId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The update mission.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> UpdateMission(Mission mission)
        {
            var existingMission = await Task.FromResult(_allMissions.FirstOrDefault(ms => ms.Id == mission.Id));
            if (existingMission == null)
            {
                return OperationResult.NotFound;
            }

            mission.CopyTo(existingMission);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            throw new NotImplementedException();
        }
    }
}