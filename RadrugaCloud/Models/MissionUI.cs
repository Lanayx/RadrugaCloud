namespace RadrugaCloud.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;
    using Core.Tools;

    /// <summary>
    ///     Class MissionUi
    /// </summary>
    public class MissionUi : IValidatableObject
    {
        #region Public Properties

        // Common

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required(ErrorMessage = "Название должно быть указано")]
        [DisplayName("Название*")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the photo URL.
        /// </summary>
        /// <value>The photo URL.</value>
        [Required(ErrorMessage = "Добавьте изображение")]
        [Url]
        [DisplayName("Изображение*")]
        [MaxLength(1000)]
        public string PhotoUrl { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required(ErrorMessage = "Добавьте краткое описание миссии")]
        [DisplayName("Описание*")]
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the difficulty.
        /// </summary>
        /// <value>The difficulty.</value>
        [Required]
        [DisplayName("Сложность")]
        [Range(0, 10)]
        public byte Difficulty { get; set; }

        /// <summary>
        ///     Gets or sets the age from.
        /// </summary>
        /// <value>The age from.</value>
        [DisplayName("Возраст От")]
        [Range(0, 100)]
        public byte? AgeFrom { get; set; }

        /// <summary>
        ///     Gets or sets the age to.
        /// </summary>
        /// <value>The age to.</value>
        [DisplayName("Возраст До")]
        [Range(0, 100)]
        public byte? AgeTo { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Required]
        [DisplayName("Тип миссии")]
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        /// Gets or sets the person qualities.
        /// </summary>
        /// <value>The person qualities.</value>
        [DisplayName("Качества человека")]
        public List<PersonQualityIdWithScore> PersonQualitiesWithScores { get; set; }

        /// <summary>
        /// Gets or sets hints.
        /// </summary>
        /// <value>Hints.</value>
        [DisplayName("Подсказки")]
        public List<Hint> Hints { get; set; }

        /// <summary>
        ///     Gets or sets the mission ids, current depends on. Optional
        /// </summary>
        /// <value>The depends on.</value>
        [DisplayName("Зависит от")]
        public List<string> DependsOn { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is final.
        /// </summary>
        /// <value><c>true</c> if this instance is final; otherwise, <c>false</c>.</value>
        [Required]
        [DisplayName("Испытание")]
        public bool IsFinal { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [message after completion].
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
        ///     Gets or sets the answers count.
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
        ///     Gets or sets the correct answers.
        /// </summary>
        /// <value>The correct answers.</value>
        [DisplayName("Правильные ответы(в соответствии с документацией)*")]
        [MaxLength(500)]
        public string CorrectAnswers { get; set; }

        /// <summary>
        ///     Gets or sets the number of tries for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        [DisplayName("Количество попыток на 1 звезду")]
        public byte? TriesFor1Star { get; set; }

        /// <summary>
        ///     Gets or sets the number of tries for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        [DisplayName("Количество попыток на 2 звезды")]
        public byte? TriesFor2Stars { get; set; }

        /// <summary>
        ///     Gets or sets the number of tries for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        [DisplayName("Количество попыток на 3 звезды")]
        public byte? TriesFor3Stars { get; set; }

        // Text Creation

        /// <summary>
        ///     Gets or sets the min chars count.
        /// </summary>
        /// <value>The min chars count.</value>
        [DisplayName("Минимальное количество символов*")]
        public int? MinCharsCount { get; set; }

        // Photo Creation

        /// <summary>
        ///     Gets or sets the number of photos.
        /// </summary>
        /// <value>The number of photos.</value>
        [DisplayName("Необходимое количество фотографий*")]
        public byte? NumberOfPhotos { get; set; }

        // GeoCoordinates + GeoPath

        /// <summary>
        ///     Gets or sets the accuracy radius.
        /// </summary>
        /// <value>The accuracy radius.</value>
        [DisplayName("Радиус точности*")]
        public int? AccuracyRadius { get; set; }

        /// <summary>
        ///     Gets or sets the common point alias.
        /// </summary>
        /// <value>The common point alias.</value>
        [DisplayName("Имя общего места*")]
        public string CommonPlaceAlias { get; set; }

        /// <summary>
        ///     Gets or sets the initial common place alias.
        /// </summary>
        /// <value>The initial common place alias.</value>
        public string InitialCommonPlaceAlias { get; set; }

        /// <summary>
        ///     Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        [DisplayName("Функция для расчета координат на клиентской стороне*")]
        public string UserCoordinatesCalculationFunction { get; set; }

        /// <summary>
        ///     Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        [DisplayName("Параметры для функции расчета координат*")]
        public List<string> CalculationFunctionParameters { get; set; }

        /// <summary>
        ///     Gets or sets the number of seconds for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        [DisplayName("Время (сек.) на 1 звезду*")]
        public int? SecondsFor1Star { get; set; }

        /// <summary>
        ///     Gets or sets the number of seconds for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        [DisplayName("Время (сек.) на 2 звезды*")]
        public int? SecondsFor2Stars { get; set; }

        /// <summary>
        ///     Gets or sets the number of seconds for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        [DisplayName("Время (сек.) на 3 звезды*")]
        public int? SecondsFor3Stars { get; set; }

        // View parameters

        /// <summary>
        ///     Gets the execution type list.
        /// </summary>
        /// <value>The execution type list.</value>
        public SelectList ExecutionTypeList
        {
            get
            {
                var excecutionTypes = Enum.GetValues(typeof(ExecutionType)).Cast<ExecutionType>();
                var itemsDictionary = excecutionTypes.ToDictionary(t => (int)t, t => t.GetDescription());
                var list = new SelectList(itemsDictionary, "Key", "Value", (int)ExecutionType);
                return list;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection that holds failed-validation information.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (ExecutionType)
            {
                case ExecutionType.RightAnswer:
                    {
                        if (string.IsNullOrEmpty(CorrectAnswers))
                        {
                            yield return
                                new ValidationResult(
                                    "Следует указать хотя бы один правильный ответ", 
                                    new[] { "CorrectAnswers" });
                        }

                        foreach (var result in ValidateTriesCount())
                        {
                            yield return result;
                        }

                        break;
                    }

                case ExecutionType.TextCreation:
                    {
                        if (MinCharsCount <= 0)
                        {
                            yield return
                                new ValidationResult(
                                    "Минимильное количество должно быть больше нуля", 
                                    new[] { "MinCharsCount" });
                        }

                        break;
                    }

                case ExecutionType.PhotoCreation:
                    {
                        if (NumberOfPhotos <= 0)
                        {
                            yield return
                                new ValidationResult(
                                    "Количество фотографий должно быть больше нуля", 
                                    new[] { "NumberOfPhotos" });
                        }

                        break;
                    }

                case ExecutionType.CommonPlace:
                    {
                        if (AccuracyRadius <= 0)
                        {
                            yield return
                                new ValidationResult("Укажите реальный радиус точности", new[] { "AccuracyRadius" });
                        }

                        if (string.IsNullOrEmpty(CommonPlaceAlias))
                        {
                            yield return
                                new ValidationResult("Укажите название общего места", new[] { "CommonPlaceAlias" });
                        }

                        foreach (var result in ValidateTriesCount())
                        {
                            yield return result;
                        }

                        break;
                    }

                case ExecutionType.Path:
                    {
                        if (AccuracyRadius <= 0)
                        {
                            yield return
                                new ValidationResult("Укажите реальный радиус точности", new[] { "AccuracyRadius" });
                        }

                        if (string.IsNullOrEmpty(UserCoordinatesCalculationFunction))
                        {
                            yield return
                                new ValidationResult(
                                    "Эта функция обязательна", 
                                    new[] { "UserCoordinatesCalculationFunction" });
                        }

                        if (CalculationFunctionParameters == null || !CalculationFunctionParameters.Any())
                        {
                            yield return
                                new ValidationResult(
                                    "Укажите хотя бы один параметр", 
                                    new[] { "CalculationFunctionParameters" });
                        }

                        if (CalculationFunctionParameters != null
                            && CalculationFunctionParameters.Any(string.IsNullOrEmpty))
                        {
                            yield return
                                new ValidationResult(
                                    "Не все параметры выбраны корректно",
                                    new[] { "CalculationFunctionParameters" });
                        }

                        foreach (var result in ValidateCommonGeoProperties())
                        {
                            yield return result;
                        }

                        break;
                    }

                //case ExecutionType.GeoPath:
                //    {
                //        foreach (var result in ValidateCommonGeoProperties())
                //        {
                //            yield return result;
                //        }

                //        if (PathLength <= 0)
                //        {
                //            yield return
                //                new ValidationResult(
                //                    "Необходимое расстояние должно быть больше нуля", 
                //                    new[] { "PathLength" });
                //        }

                //        break;
                //    }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Converts to domain.
        /// </summary>
        /// <returns>Mission.</returns>
        internal Mission ConvertToDomain()
        {
            var mission = new Mission
                              {
                                  // Common
                                  Id = Id, 
                                  Name = Name, 
                                  PhotoUrl = PhotoUrl, 
                                  Description = Description, 
                                  Difficulty = Difficulty, 
                                  AgeFrom = AgeFrom, 
                                  AgeTo = AgeTo, 
                                  ExecutionType = ExecutionType, 
                                  DependsOn = DependsOn, 
                                  IsFinal = IsFinal, 
                                  MessageAfterCompletion = MessageAfterCompletion,
                                  MissionSetId = MissionSetId,

                                  // Right answer type
                                  AnswersCount = AnswersCount, 
                                  ExactAnswer = ExactAnswer, 
                                  CorrectAnswers = CorrectAnswers, 
                                  TriesFor1Star = TriesFor1Star, 
                                  TriesFor2Stars = TriesFor2Stars, 
                                  TriesFor3Stars = TriesFor3Stars, 

                                  // Text Creation
                                  MinCharsCount = MinCharsCount, 

                                  // Photo Creation
                                  NumberOfPhotos = NumberOfPhotos, 

                                  // GeoCoordinates + GeoPath
                                  AccuracyRadius = AccuracyRadius, 
                                  CommonPlaceAlias = CommonPlaceAlias, 
                                  UserCoordinatesCalculationFunction = UserCoordinatesCalculationFunction, 
                                  CalculationFunctionParameters = CalculationFunctionParameters, 
                                  SecondsFor1Star = SecondsFor1Star, 
                                  SecondsFor2Stars = SecondsFor2Stars, 
                                  SecondsFor3Stars = SecondsFor3Stars,                                  
                                  Hints = Hints
            };
                       
            if (PersonQualitiesWithScores != null)
            {
                var qualitiesWithScores =
                    PersonQualitiesWithScores.Where(ptws => ptws.PersonQualityId != null)
                        .GroupBy(ptws => ptws.PersonQualityId)
                        .Select(grp => grp.First())
                        .Select(
                            ptws =>
                            {
                                var personQualityWithScore = new PersonQualityIdWithScore
                                {
                                    PersonQualityId = ptws.PersonQualityId,
                                    Score = ptws.Score
                                };
                                return personQualityWithScore;
                            }).ToList();

                mission.PersonQualities = qualitiesWithScores.Any() ? qualitiesWithScores : null;
            }           
            return mission;
        }

        private IEnumerable<ValidationResult> ValidateCommonGeoProperties()
        {
            if (SecondsFor3Stars <= 0)
            {
                yield return new ValidationResult("Лучшее время должно быть больше нуля", new[] { "SecondsFor3Stars" });
            }

            if (SecondsFor2Stars <= 0)
            {
                yield return new ValidationResult("Среднее время должно быть больше нуля", new[] { "SecondsFor2Stars" });
            }

            if (SecondsFor1Star <= 0)
            {
                yield return new ValidationResult("Худшее время должно быть больше нуля", new[] { "SecondsFor1Star" });
            }

            if (SecondsFor2Stars <= SecondsFor3Stars)
            {
                yield return
                    new ValidationResult("Среднее время должно быть больше, чем лучшее", new[] { "SecondsFor2Stars" });
            }

            if (SecondsFor1Star <= SecondsFor2Stars)
            {
                yield return
                    new ValidationResult("Худшее время должно быть больше, чем среднее", new[] { "SecondsFor1Star" });
            }
        }

        private IEnumerable<ValidationResult> ValidateTriesCount()
        {
            if (TriesFor1Star <= 0 && TriesFor2Stars <= 0 && TriesFor3Stars <= 0)
            {
                TriesFor1Star = TriesFor2Stars = TriesFor3Stars = 0;
                yield break;
            }

            if (TriesFor3Stars <= 0)
            {
                yield return
                    new ValidationResult(
                        "Лучший результат должен быть больше нуля или обнулите остальные результаты", 
                        new[] { "TriesFor3Stars" });
            }

            if (TriesFor2Stars <= 0)
            {
                yield return
                    new ValidationResult(
                        "Средний результат должен быть больше нуля или обнулите остальные результаты", 
                        new[] { "TriesFor2Stars" });
            }

            if (TriesFor1Star <= 0)
            {
                yield return
                    new ValidationResult(
                        "Худший результат должен быть больше нуля или обнулите остальные результаты", 
                        new[] { "TriesFor1Star" });
            }

            if (TriesFor2Stars <= TriesFor3Stars)
            {
                yield return
                    new ValidationResult("Средний результат должен быть больше, чем лучший", new[] { "TriesFor2Stars" });
            }

            if (TriesFor1Star <= TriesFor2Stars)
            {
                yield return
                    new ValidationResult("Худший результат должет быть больше, чем средний", new[] { "TriesFor1Star" });
            }
        }

        #endregion
    }
}