namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;

    using Core.AuthorizationModels;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools;

    /// <summary>
    /// Class DeleteAllTables
    /// </summary>
    internal class FillTablesWithTestData : IDeveloperOperation
    {
        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            try
            {
                await FillPersonQualities();
                await FillMissions();
                await FillMissionDrafts();
                await FillUsers();
                await FillUserIdentities();
                await FillQuestions();
                await FillMissionRequests();
            }
            catch (Exception ex)
            {
                return new OperationResult(OperationResultStatus.Error, ex.ToString());
            }

            return new OperationResult(OperationResultStatus.Success);
        }

        private async Task FillMissionRequests()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IMissionRequestRepository>();

            await
                repo.AddMissionRequest(
                    new MissionRequest
                        {
                            Id = "Request1",
                            MissionId = "Mission1",
                            Status = MissionRequestStatus.NotChecked,
                            UserId = "User1Id",
                            Mission = new Mission { Name = "Super Mission" },
                            User = new User { NickName = "John" },
                            LastUpdateDate = DateTime.UtcNow,
                            Proof =
                                new MissionProof
                                    {
                                        ImageUrls =
                                            new List<string>
                                                {
                                                    "http://cs540104.vk.me/c540102/v540102927/1dbd2/YHovj9WmxPs.jpg",
                                                    "http://cs540104.vk.me/c7007/v7007850/215bf/SXFDsiqoFzE.jpg"
                                                }
                                    }
                        });
            await
                repo.AddMissionRequest(
                    new MissionRequest
                        {
                            Id = "Request2",
                            MissionId = "Mission2",
                            Status = MissionRequestStatus.NotChecked,
                            UserId = "User1Id",
                            Mission = new Mission { Name = "Super Mission 2" },
                            User = new User { NickName = "John" },
                            Proof = new MissionProof { CreatedText = "This is a long story to describe" },
                            LastUpdateDate = DateTime.UtcNow.AddDays(-1)
                        });
        }

        private List<PersonQualityIdWithScore> GetPersonQualitiesWithScoresTriple(List<int> positivePositions)
        {
            var personQualitiesWithScores = new List<PersonQualityIdWithScore>
                                                { 
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_target" , Score = -1 },
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_young" , Score = -1 },
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_old" , Score = -1 }
                                                };
            foreach (var position in positivePositions)
            {
                personQualitiesWithScores[position].Score = 1;
            }

            return personQualitiesWithScores;
        }

        private List<PersonQualityIdWithScore> GetPersonQualitiesWithScoresDouble(double score)
        {
            var personQualitiesWithScores = new List<PersonQualityIdWithScore>
                                                { 
                                                    new PersonQualityIdWithScore { PersonQualityId = "PersonQuality1" , Score = score },
                                                    new PersonQualityIdWithScore { PersonQualityId = "PersonQuality2" , Score = score }
                                                };
            
            return personQualitiesWithScores;
        }

        private async Task FillQuestions()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IQuestionRepository>();

            await
                repo.AddQuestion(
                    new Question
                        {
                            Id = "Id_Question1",
                            Name = "Age test",
                            Text = "How old are you?",
                            Options =
                                new List<QuestionOption>
                                    {
                                        new QuestionOption
                                            {
                                                Text = "Before 13",
                                                Number = 1,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 1 })
                                            },
                                        new QuestionOption
                                            {
                                                Text = "13-19",
                                                Number = 2,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 0, 2 })
                                            },
                                        new QuestionOption
                                            {
                                                Text = "Older then 19",
                                                Number = 3,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 2 })
                                            }
                                    }
                        });

            await
                repo.AddQuestion(
                    new Question
                        {
                            Id = "Id_Question2",
                            Name = "Iq test",
                            Text = "What is your iq?",
                            Options =
                                new List<QuestionOption>()
                                    {
                                        new QuestionOption
                                            {
                                                Text = "Below 90",
                                                Number = 1,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(-1)
                                            },
                                        new QuestionOption
                                            {
                                                Text = "90-120",
                                                Number = 2,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(1)
                                            },
                                        new QuestionOption
                                            {
                                                Text = "Over 120",
                                                Number = 3,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(3)
                                            }
                                    }
                        });
        }

        private async Task FillUserIdentities()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IUserIdentityRepository>();
            await repo.AddUserIdentity(
                   new UserIdentity
                   {
                       Id = "User1Id",
                       LoginEmail = "User1Login",
                       HashedPassword =
                           HashHelper.GetPasswordHash("User1Password"),
                       HashType = HashHelper.CurrentHashType
                   });
        }

        private async Task FillUsers()
        {
            var personQuality = new PersonQuality { Id = "PersonQuality1", Name = "Холерик" };
            var personQualityWithScore = new PersonQualityIdWithScore { PersonQualityId = personQuality.Id, Score = 2 };
            var repo = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>();
            await
                repo.AddUser(
                    new User
                        {
                            Id = "User1Id",
                            AvatarUrl = "http://cs540104.vk.me/c540102/v540102420/e88c/P64liS_pPNk.jpg",
                            DateOfBirth = new DateTime(1990, 1, 1),
                            NickName = "John",
                            HomeCoordinate = new GeoCoordinate(53.9, 27.56667, 199),
                            KindScale = 87,
                            Points = 120,
                            EnablePushNotifications = true,
                            Level = 7,
                            Sex = Sex.Male,
                            PersonQualitiesWithScores = new List<PersonQualityIdWithScore> { personQualityWithScore }
                        });
        }

        private async Task FillMissionDrafts()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IMissionDraftRepository>();
            await
                repo.AddMissionDraft(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft1",
                            Name = "First MissionDraft",
                            AddDate = DateTime.UtcNow,
                            Description = "In this missionDraft you need to put your ass in the air.",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg"
                        });
            await
                repo.AddMissionDraft(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft2",
                            Name = "Second MissionDraft",
                            Description = "In this missionDraft you have to byte your elbow.",
                            AddDate = DateTime.UtcNow,
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            AgeFrom = 10,
                            Author = "Odin",
                            PersonQualities =
                                new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                Score = 0,
                                                PersonQualityId ="PersonQuality1"
                                            },
                                        new PersonQualityIdWithScore
                                            {
                                                Score = 0,
                                                PersonQualityId = "PersonQuality2"
                                            }
                                    }
                        });
            await
                repo.AddMissionDraft(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft3",
                            Name = "Third MissionDraft",
                            AddDate = DateTime.UtcNow,
                            Description = "In this missionDraft you have to byte your elbow.",
                            PhotoUrl = "http://cs540104.vk.me/c540103/v540103018/14e6d/ZNqbo3CDv7Y.jpg",
                            AgeFrom = 18,
                            Author = "Odin",
                            PersonQualities =
                                new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                Score = 0,
                                                PersonQualityId = "Id_young"
                                            }
                                    }
                        });
        }

        private async Task FillMissions()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IMissionRepository>();
            await
                repo.AddMission(
                    new Mission
                        {
                            Id = "Mission1",
                            Name = "First Mission",
                            Description = "In this mission you need to put your ass in the air.",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            Hints = new List<Hint>
                                        {
                                            new Hint
                                                {
                                                    Id = "Hint1",
                                                    Text = "The text hint for first mission",
                                                    Type = HintType.Text,
                                                    Score = 2
                                                },
                                            new Hint
                                                {
                                                    Id = "Hint2",
                                                    Type = HintType.Coordinate,
                                                    Score = 3
                                                },
                                            new Hint
                                                {
                                                    Id = "Hint3",                                                    
                                                    Type = HintType.Direction,
                                                    Score = 4
                                                }

                                        }
                        });
            await
                repo.AddMission(
                    new Mission
                        {
                            Id = "Mission2",
                            Name = "Second Mission",
                            Description = "In this mission you have to byte your elbow.",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        });
            await
                repo.AddMission(
                    new Mission
                        {
                            Id = "Mission3",
                            Name = "Third Mission",
                            Description = "In this mission you have to sleep",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                        });
        }

        private async Task FillPersonQualities()
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IPersonQualityRepository>();
            await
                repo.AddPersonQuality(
                    new PersonQuality
                        {
                            Id = "Id_target",
                            Name = "Target guy",
                            Description = "This is a Person type for age testing"
                        });
            await
                repo.AddPersonQuality(
                    new PersonQuality
                        {
                            Id = "Id_young",
                            Name = "Young guy",
                            Description = "This is a second Person type for age testing"
                        });
            await
                repo.AddPersonQuality(
                    new PersonQuality
                        {
                            Id = "Id_old",
                            Name = "Old guy",
                            Description = "This is a thirgd Person type for age testing"
                        });
            await
                repo.AddPersonQuality(
                    new PersonQuality
                        {
                            Id = "PersonQuality1",
                            Name = "First PersonQuality",
                            Description = "This is a Person type for testing"
                        });
            await
                repo.AddPersonQuality(
                    new PersonQuality
                        {
                            Id = "PersonQuality2",
                            Name = "Second PersonQuality",
                            Description = "This is a second Person type for testing"
                        });
        }
    }
}