namespace Core.Enums
{
    using Core.Attributes;

    /// <summary>
    /// Enum MissionType
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        /// The right answer
        /// </summary>
        [Description("Правильный ответ")]
        RightAnswer = 0,

        /// <summary>
        /// The text creation
        /// </summary>
        [Description("Развернутый ответ")]
        TextCreation = 1,

        /// <summary>
        /// The photo creation
        /// </summary>
        [Description("Фото-отчет")]
        PhotoCreation = 2,

        /// <summary>
        /// The geo coordinates
        /// </summary>
        [Description("Маршрут")]
        Path = 3,

       /////// <summary>
        /////// The geo path - new type can be pasted instead of geopath
        /////// </summary>
        //[Description("Путь")]
        //GeoPath = 4,

        /// <summary>
        /// The geo path
        /// </summary>
        [Description("Общее место")]
        CommonPlace = 5,

        /// <summary>
        /// The geo path
        /// </summary>
        [Description("Уникальная миссия")]
        Unique = 6,

        /// <summary>
        /// The video
        /// </summary>
        [Description("Видео ссылка")]
        Video = 7
    }
}