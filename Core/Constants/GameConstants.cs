namespace Core.Constants
{
    using System;

    /// <summary>
    ///     Business logic constants
    /// </summary>
    public static class GameConstants
    {

        public static class Mission
        {
            public const int TemporaryCommonPlaceLimit = 9;
            
            public const double TemporaryCommonPlaceAccuracyRadius = 150;

            public const double CommonPlaceMinDistanceFromHome = 50;
        }

        public static class Experience
        {
            public const byte IncreasePerLevel = 5;

            public const ushort OnFirstLevel = 44;

            public const ushort OnSecondLevel = 65;

            public const byte PerDifficultyPoint = 10;

            public const byte UnreachableLevel = 15;
        }

        public static class KindScale
        {
            public const byte AchievementLimit = 40;

            public const byte BaseReward = 20;

            public const byte DailyRegression = 40;

            public const byte MaxUserPoints = 100;

            public const byte WithPhotoReward = 25;
        }

        public static class MissionSet
        {
            public const string FirstSetId = "959752ee-8bd8-45fe-b8e0-7a2628eabce3";

            public const string LastSetId = "ab2ccc5f-ff6d-484b-a73b-e4265be6ef7f";

            public const string SecondSetId = "926b850a-80f2-4fda-a421-a6ec0feafdd8";

            public const string ThirdSetId = "8090198f-45bf-42cd-9c5c-53d914320ef7";

            /// <summary>
            /// The maximum sets per user
            /// </summary>
            public const int MaxSetsPerUser = 3;
        }

        public static class PersonQuality
        {
            public const string ActivityQualityId = "d1922012-b71a-4a2a-9b48-2ab3dc473161";

            public const string CommunicationQualityId = "cd433d41-1f3f-448e-849b-a0d203be706d";

            public const double IncreasePerKindAction = 0.1;

            public const string LightQualityId = "dae0e563-d900-4695-9016-776ad891525e";

            public const double LightScoreMax = 100;

            public const double LightScoreMin = 0;

            public const byte StarsNeededForMissionValue = 2;
        }

        public static class Points
        {
            public const double ExtraMultiplier = Math.PI;

            public const byte IncreasePerKindAction = 1;
        }

        public static class Rating
        {
            public const byte LeadersCount = 10;

            public const byte NeighboursCountOneSide = 2;
        }

        public static class KindActions
        {
            public const byte DisplayPerPage = 10;
        }

        public static class Coins
        {
            public const byte IncreasePerKindActionMaxing = 3;
            public const byte Start = 5;
        }
    }
}