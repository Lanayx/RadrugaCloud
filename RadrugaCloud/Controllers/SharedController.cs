namespace RadrugaCloud.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.CommonModels;

    using Services.BL;

    /// <summary>
    /// Class SharedController
    /// </summary>
    public class SharedController : Controller
    {
        private readonly ImageService _imageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedController"/> class.
        /// </summary>
        /// <param name="imageService">The image service.</param>
        public SharedController(ImageService imageService)
        {
            _imageService = imageService;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        /// <exception cref="System.ArgumentNullException">Выберите корректное изображение</exception>
        /// <exception cref="System.Exception"></exception>
        [HttpPost]
        public async Task<string> UploadImage()
        {
            var image = Request.Files["image"];
            if (image == null || !image.ContentType.StartsWith("image"))
            {
                throw new ArgumentNullException("Выберите корректное изображение", innerException: null);
            }

            var fileData = new FileData
                               {
                                   ContentType = image.ContentType,
                                   FileName = image.FileName,
                                   InputStream = image.InputStream
                               };

            return await _imageService.SaveTemporaryImage(fileData);
        }

        #endregion
    }
}