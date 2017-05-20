namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Services.DomainServices;
    using RadrugaCloud.Models;
    using Helpers;

    using ExpressionHelper = Core.Tools.ExpressionHelper;

    /// <summary>
    ///     The mission draft controller.
    /// </summary>
    [Authorize]
    public class MissionController : Controller
    {
        #region Constants

        private const string ErrorsTempKey = "ErrorsTempKey";

        private const string MissionTempKey = "MissionTempKey";

        #endregion

        #region Fields

        private readonly CommonPlacesService _commonPlacesService;

        private readonly MissionService _missionService;

        private readonly PersonQualityService _personQualityService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionController" /> class.
        /// </summary>
        /// <param name="personQualityService">The person quality service.</param>
        /// <param name="missionService">The mission service.</param>
        /// <param name="commonPlacesService">The common places service.</param>
        public MissionController(
            PersonQualityService personQualityService, 
            MissionService missionService, 
            CommonPlacesService commonPlacesService)
        {
            _personQualityService = personQualityService;
            _missionService = missionService;
            _commonPlacesService = commonPlacesService;
        }

        #endregion

        #region Properties

        private Dictionary<string, string> UserRelatedCommonPlaces
        {
            get
            {
                var itemsDictionary = new Dictionary<string, string>
                                          {
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.HomeCoordinate)), 
                                                  "Координата дома"
                                              }, 
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.BaseNorthCoordinate)), 
                                                  "Северная граница базы"
                                              }, 
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.BaseEastCoordinate)), 
                                                  "Восточная граница базы"
                                              }, 
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.BaseSouthCoordinate)), 
                                                  "Южная граница базы"
                                              }, 
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.BaseWestCoordinate)), 
                                                  "Западная граница базы"
                                              }, 
                                              {
                                                  ToUserRelatedValue(ExpressionHelper.GetPropertyName<User, GeoCoordinate>(u => u.RadarCoordinate)), 
                                                  "Радар"
                                              }, 
                                          };

                /*var list = new SelectList(itemsDictionary, "Key", "Value");*/
                return itemsDictionary;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates this instance.
        /// </summary>
        /// <returns>
        ///     ActionResult.
        /// </returns>
        public async Task<ActionResult> Create()
        {
            var model = new MissionUi();
            SetModelToTempData(model);
            ViewBag.CommonPlaces = await GetCommonPlaces();
            ViewBag.Missions = await GetBaseMissions();

            return View(model);
        }

        // GET: Missions/Edit/5

        /// <summary>
        ///     Creates the specified mission.
        /// </summary>
        /// <param name="mission">The mission.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MissionUi mission)
        {
            return await AddUpdateMission(mission, true);
        }

        // POST: Missions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mission = await _missionService.GetMission(id);
            if (mission == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(mission);
        }

        // POST: Missions/Delete/5

        /// <summary>
        ///     The delete confirmed.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _missionService.DeleteMission(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     The details.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mission = await _missionService.GetMission(id);
            if (mission == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(mission);
        }

        /// <summary>
        ///     The edit.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mission = (await _missionService.GetMission(id)).ConvertToUi();
            if (mission == null)
            {
                return HttpNotFound();
            }

            SetModelToTempData(mission);

            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            ViewBag.Missions = await GetBaseMissions(id);
            if (mission.ExecutionType == ExecutionType.CommonPlace || mission.ExecutionType == ExecutionType.Path)
                ViewBag.CommonPlaces = await GetCommonPlaces();

            return View(mission);
        }

        /// <summary>
        ///     The edit.
        /// </summary>
        /// <param name="mission">
        ///     The mission draft.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MissionUi mission)
        {
            return await AddUpdateMission(mission, false);
        }

        /// <summary>
        ///     Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        ///     Task{ActionResult}.
        /// </returns>
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 0;
            const int MissionsOnPage = 15;
            var selectOptions = new QueryOptions<Mission>
                                    {
                                        /*Filter = p => p.Author == "Defor",*/
                                        Skip = pageNumber * MissionsOnPage, 
                                        Top = MissionsOnPage + 1, 
                                        Select =
                                            draft =>
                                            new
                                                {
                                                    draft.Id, 
                                                    draft.Name, 
                                                    draft.Description, 
                                                    draft.PhotoUrl, 
                                                    draft.AgeFrom, 
                                                    draft.AgeTo, 
                                                    draft.Difficulty
                                                }
                                    };

            Expression<Func<Mission, string>> z = x => x.Name;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };

            var missions = await _missionService.GetMissions(selectOptions);
            IEnumerable<Mission> model;
            if (missions.Count <= MissionsOnPage)
            {
                ViewBag.ShowNext = false;
                model = missions;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = missions.Take(MissionsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        /// <summary>
        ///     Bases the missions editor.
        /// </summary>
        /// <param name="currentMissionId">The current mission id.</param>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> SeederBaseMissions(string currentMissionId = "")
        {
            var missions = await GetBaseMissions(currentMissionId);

            return PartialView("Controls/BaseMissionsEditor", missions);
        }

        /// <summary>
        ///     Calculations the function parameters editor.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> SeederCalculationFunctionParameters()
        {
            var places = await GetCommonPlaces();
            return PartialView("Controls/CalculationFunctionParametersEditor", places);
        }

        /// <summary>
        /// Seeders the person qualities with score.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> SeederPersonQualitiesWithScore()
        {
            var personQualities = await _personQualityService.GetPersonQualities();
            ViewBag.PersonQualities = personQualities;

            return PartialView("Controls/PersonQualitiesWithScoreEditor");
        }

        /// <summary>
        /// Seeders the person qualities with score.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        public ActionResult GetHintTemplate()
        {            
            return PartialView("Controls/HintEditor");
        }

        /// <summary>
        ///     Switches the type of the mission.
        /// </summary>
        /// <param name="executionType">Type of the execution.</param>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> SwitchMissionType(int executionType)
        {
            var model = GetModelFromTempData();
            if (model == null)
            {
                return PartialView("MissionTypes/_typeError", "Model is null");
            }

            var errors = GetErrorsFromTempData();
            if (errors != null)
            {
                foreach (var error in errors.Where(e => e.Value.Errors.Any()))
                {
                    ModelState.Add(error);
                }
            }

            var type = (ExecutionType)executionType;
            switch (type)
            {
                case ExecutionType.RightAnswer:
                    {
                        return PartialView("MissionTypes/_rightAnswer", model);
                    }

                case ExecutionType.TextCreation:
                    {
                        return PartialView("MissionTypes/_textCreation", model);
                    }

                case ExecutionType.PhotoCreation:
                    {
                        return PartialView("MissionTypes/_photoCreation", model);
                    }

                case ExecutionType.Path:
                    {
                        ViewBag.CommonPlaces = await GetCommonPlaces();
                        return PartialView("MissionTypes/_geoLocation", model);
                    }

                case ExecutionType.CommonPlace:
                    {
                        return PartialView("MissionTypes/_commonPlace", model);
                    }
                case ExecutionType.Unique:
                case ExecutionType.Video:
                    {
                        return new EmptyResult();
                    }

            }

            return PartialView("MissionTypes/_typeError", "Incorrect execution type");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _missionService.Dispose();
                _personQualityService.Dispose();
                _commonPlacesService.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<ActionResult> AddUpdateMission(MissionUi mission, bool createOperation)
        {
            if (ModelState.IsValid)
            {
                OperationResult result;
                if (mission.ExecutionType == ExecutionType.CommonPlace)
                {
                    if (createOperation)
                    {
                        var aliasExist = await _commonPlacesService.CheckAliasExist(mission.CommonPlaceAlias);
                        if (aliasExist)
                        {
                            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
                            ModelState.AddModelError("CommonPlaceAlias", "Указанное имя уже используется");
                            return await ErrorResult(mission);
                        }

                        result = await _commonPlacesService.AddAlias(new CommonPlaceAlias(mission.CommonPlaceAlias));
                        if (result.Status != OperationResultStatus.Success)
                        {
                            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
                            ModelState.AddModelError(string.Empty, result.Description);
                            return await ErrorResult(mission);
                        }
                    }
                    else
                    {
                        if (!string.Equals(mission.CommonPlaceAlias, mission.InitialCommonPlaceAlias))
                        {
                            var aliasExist = await _commonPlacesService.CheckAliasExist(mission.CommonPlaceAlias);
                            if (aliasExist)
                            {
                                ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
                                ModelState.AddModelError("CommonPlaceAlias", "Указанное имя уже используется");
                                return await ErrorResult(mission);
                            }

                            if (string.IsNullOrEmpty(mission.InitialCommonPlaceAlias))
                            {
                                result =
                                    await _commonPlacesService.AddAlias(new CommonPlaceAlias(mission.CommonPlaceAlias));
                                if (result.Status != OperationResultStatus.Success)
                                {
                                    ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
                                    ModelState.AddModelError(string.Empty, result.Description);
                                    return await ErrorResult(mission);
                                }
                            }
                            else
                            {
                                var oldAlias = new CommonPlaceAlias(mission.InitialCommonPlaceAlias)
                                {
                                    FullName =
                                        mission
                                        .CommonPlaceAlias
                                };
                                result = await _commonPlacesService.UpdateAliaseName(oldAlias);
                                if (result.Status != OperationResultStatus.Success)
                                {
                                    ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
                                    ModelState.AddModelError(string.Empty, result.Description);
                                    return await ErrorResult(mission);
                                }
                            }
                        }
                    }
                }
                
                result = createOperation
                             ? await _missionService.AddNewMission(mission.ConvertToDomain())
                             : await _missionService.UpdateMission(mission.ConvertToDomain());
                if (result.Status != OperationResultStatus.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Description);
                    return await ErrorResult(mission);
                }

                return RedirectToAction("Index");
            }


            return await ErrorResult(mission);
        }

        private async Task<ActionResult> ErrorResult(MissionUi mission)
        {
            var personQualities = (await _personQualityService.GetPersonQualities());
            ViewBag.CommonPlaces = await GetCommonPlaces();
            ViewBag.PersonQualities = personQualities;
            ViewBag.Missions = await GetBaseMissions(mission.Id);

            SetModelToTempData(mission);
            SetErrorsToTempData(ModelState);

            return View(mission);
        }

        private async Task<Dictionary<string, string>> GetBaseMissions(string currentMissionId = "")
        {
            var selectOptions = new QueryOptions<Mission>
                                    {                                        
                                        Select = m => new { m.Id, m.Name, }
                                    };
            if (!string.IsNullOrEmpty(currentMissionId))
            {
                selectOptions.Filter = m => m.Id != currentMissionId;
            }

            Expression<Func<Mission, string>> z = x => x.Name;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };

            var missions = await _missionService.GetMissions(selectOptions);
            return missions.ToDictionary(m => m.Id, m => m.Name);
        }

        private async Task<List<GroupedSelectListItem>> GetCommonPlaces()
        {
            var commonPlacesAliases = await _commonPlacesService.GetAliases(new QueryOptions<CommonPlaceAlias>());
            var userRelatedPlaces =
                UserRelatedCommonPlaces.Select(
                    u =>
                    new GroupedSelectListItem
                        {
                            GroupKey = "0",
                            GroupName = "Зависящие от пользователя",
                            Text = u.Value,
                            Value = u.Key
                        }).ToList();
            userRelatedPlaces.AddRange(
                commonPlacesAliases.Select(
                    c =>
                    new GroupedSelectListItem
                        {
                            GroupKey = "1",
                            GroupName = "Общие места",
                            Text = c.FullName,
                            Value = c.ShortName
                        }));

            return userRelatedPlaces;
        }

        private IEnumerable<KeyValuePair<string, ModelState>> GetErrorsFromTempData()
        {
            var dictionary = TempData[ErrorsTempKey] as ModelStateDictionary;
            return dictionary;
        }

        private MissionUi GetModelFromTempData()
        {
            var model = TempData[MissionTempKey] as MissionUi;
            if (model != null)
            {
                TempData.Keep(MissionTempKey);
            }

            return model;
        }

        private void SetErrorsToTempData(ModelStateDictionary dictionary)
        {
            TempData[ErrorsTempKey] = dictionary;
        }

        private void SetModelToTempData(MissionUi model)
        {
            TempData[MissionTempKey] = model;
        }

        private string ToUserRelatedValue(string userRelatedPropertyName)
        {
            return string.Concat("u_", userRelatedPropertyName);
        }

        #endregion
    }
}