namespace Core.DomainModels
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Core.Enums;
    using Core.NonDomainModels;

    /// <summary>
    /// The mission.
    /// </summary>
    public class Mission
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mission"/> class.
        /// </summary>
        public Mission()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mission"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public Mission(string id)
        {
            Id = id;
        }

        #region Public Properties

        // Common

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the photo url.
        /// </summary>
        [DisplayName("Картинка")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DisplayName("Описание")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        [DisplayName("Сложность")]
        public byte Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the age from.
        /// </summary>
        [DisplayName("Возраст От")]
        public byte? AgeFrom { get; set; }

        /// <summary>
        /// Gets or sets the age to.
        /// </summary>
        [DisplayName("Возраст До")]
        public byte? AgeTo { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DisplayName("Тип миссии")]
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        /// Gets or sets the mission ids, current depends on. Optional
        /// </summary>
        /// <value>The depends on.</value>
        [DisplayName("Зависит от")]
        public List<string> DependsOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is final.
        /// </summary>
        /// <value><c>true</c> if this instance is final; otherwise, <c>false</c>.</value>
        [DisplayName("Испытание")]
        public bool IsFinal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [message after completion].
        /// </summary>
        /// <value><c>true</c> if [message after completion]; otherwise, <c>false</c>.</value>
        [DisplayName("Сообщение по завершению")]
        public string MessageAfterCompletion { get; set; }

        /// <summary>
        /// Gets or sets the mission set id.
        /// </summary>
        /// <value>The mission set id.</value>
        public string MissionSetId { get; set; }

        // Right answer type
        
        /// <summary>
        /// Gets or sets the answers count.
        /// </summary>
        /// <value>The answers count.</value>
        [DisplayName("Количество правильных ответов, которые нужно дать")]
        public byte? AnswersCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exact answer].
        /// </summary>
        /// <value><c>true</c> if [exact answer]; otherwise, <c>false</c>.</value>
        [DisplayName("Необходим точный ответ")]
        public bool? ExactAnswer { get; set; }

        /// <summary>
        /// Gets or sets the correct answers.
        /// </summary>
        [DisplayName("Правильные ответы")]
        public string CorrectAnswers { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        [DisplayName("Количество попыток на 3 звезды")]
        public byte? TriesFor3Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        [DisplayName("Количество попыток на 2 звезды")]
        public byte? TriesFor2Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        [DisplayName("Количество попыток на 1 звезду")]
        public byte? TriesFor1Star { get; set; }

        // Text Creation

        /// <summary>
        /// Gets or sets the min chars count.
        /// </summary>
        /// <value>The min chars count.</value>
        [DisplayName("Минимальное количество символов")]
        public int? MinCharsCount { get; set; }

        // Photo Creation

        /// <summary>
        /// Gets or sets the number of photos.
        /// </summary>
        /// <value>The number of photos.</value>
        [DisplayName("Необходимое количество фотографий")]
        public byte? NumberOfPhotos { get; set; }

        // GeoCoordinates + GeoPath

        /// <summary>
        /// Gets or sets the accuracy radius.
        /// </summary>
        /// <value>The accuracy radius.</value>
        [DisplayName("Радиус точности")]
        public int? AccuracyRadius { get; set; }

        /// <summary>
        /// Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        [DisplayName("Функция для расчета координат на клиентской стороне")]
        public string UserCoordinatesCalculationFunction { get; set; }

        /// <summary>
        /// Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        [DisplayName("Параметры для функции расчета координат")]
        public List<string> CalculationFunctionParameters { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        [DisplayName("Время (сек.) на 3 звезды")]
        public int? SecondsFor3Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        [DisplayName("Время (сек.) на 2 звезды")]
        public int? SecondsFor2Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        [DisplayName("Время (сек.) на 1 звезду")]
        public int? SecondsFor1Star { get; set; }

        /// <summary>
        /// Gets or sets the common point alias.
        /// </summary>
        /// <value>The common point alias.</value>
        [DisplayName("Имя общего места")]
        public string CommonPlaceAlias { get; set; }

        /// <summary>
        /// Gets or sets the person qualities.
        /// </summary>
        /// <value>
        /// The person qualities.
        /// </value>
        [DisplayName("Качества человека")]
        public List<PersonQualityIdWithScore> PersonQualities { get; set; }


        /// <summary>
        /// Gets or sets hints.
        /// </summary>
        /// <value>
        /// Hints.
        /// </value>
        [DisplayName("Подсказки")]
        public List<Hint> Hints { get; set; }

        #endregion
    }
}