namespace Services.UniqueMissions
{
    using System;
    using System.Collections.Generic;

    using Services.UniqueMissions.Missions;

    /// <summary>
    /// Class UniqueMissionsDictionary
    /// </summary>
    public static class UniqueMissionsDictionary
    {
        private static readonly Dictionary<string, Type> InnerDictionary =
            new Dictionary<string, Type>
                {
                    { "5823973e-ac62-4efe-9c5d-1755df874d99", typeof(CommandPoint) },
                    { "d061155a-9504-498d-a6e7-bcc20c295cde", typeof(ShowYourself) },
                    { "71130d2e-e513-4b29-ad12-7a368f4d927a", typeof(YourBase)},
                    { "3be2d03c-f100-42be-bc9f-54166e108d43", typeof(PerpetumMobile)},
                    { "524b9f98-88b2-460a-ae3f-e15df4cef2ae", typeof(Censored)},
                    { "2c187275-97c0-4dcf-a1f0-89d7018a03e2", typeof(FriendBase)},
                    { "8ab5fa9b-15a7-49ec-8d5f-2a47d4affd52", typeof(HealthySpirit1)},
                    { "b23f79fe-08d3-4d24-a293-51f67e2131be", typeof(Radar)},
                    { "19e18c3f-2daa-4e18-bf32-2a1f77fdc73f", typeof(HelpChoice)},
                    { "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e", typeof(Outpost)}
                };

        /// <summary>
        /// Gets the type of the mission.
        /// </summary>
        /// <param name="missionId">The mission id.</param>
        /// <returns>Type.</returns>
        public static Type GetMissionType(string missionId)
        {
            return !InnerDictionary.ContainsKey(missionId) ? null : InnerDictionary[missionId];
        }
    }
}
