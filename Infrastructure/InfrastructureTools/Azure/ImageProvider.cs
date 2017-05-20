namespace Infrastructure.InfrastructureTools.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Core.CommonModels;
    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using AzureTablesObjects;

    using Core.Tools;

    /// <summary>
    ///     Class ImagesProvider
    /// </summary>
    public class ImageProvider : IImageProvider
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Deletes the image.
        /// </summary>
        /// <param name="photoUrl">The photo URL.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> DeleteImage(string photoUrl)
        {
            var info = BlobInfo.GetInfoByUrl(photoUrl);
            var manager = GetManager(info.Container);
            if (manager == null)
            {
                return false;
            }

            return await manager.DeleteBlob(info.FullName);
        }

        /// <summary>
        /// Deletes the images.
        /// </summary>
        /// <param name="photoUrls">The photo urls.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> DeleteImages(IEnumerable<string> photoUrls)
        {
            if (!photoUrls.AnyValues())
                return true;
            var succededTasks = new List<Task<bool>>();
            foreach (var photoUrl in photoUrls)
            {
                var info = BlobInfo.GetInfoByUrl(photoUrl);
                var manager = GetManager(info.Container);
                if (manager == null)
                {
                    return false;
                }

                succededTasks.Add(manager.DeleteBlob(info.FullName));
            }

            var results = await Task.WhenAll(succededTasks);
            return results.All(result => result);
        }

        /// <summary>
        ///     Gets the temp images count.
        /// </summary>
        /// <returns>Task{System.Int32}.</returns>
        public async Task<int> GetTempImagesCount()
        {
            var images = await GetImages(BlobContainer.TempStorage);
            return images.Count;
            }

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        public async Task<string> SaveImage(FileData image, BlobContainer container)
        {
            var manager = GetManager(container);
            if (manager == null || image == null)
            {
                return string.Empty;
            }

            var uniqueName = GetUniqueImageName(Path.GetExtension(image.FileName));
            var contentType = image.ContentType;

            return await manager.AddBlockBlob(uniqueName, image.InputStream, contentType);
        }

        /// <summary>
        ///     Saves the image by URL.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        public async Task<OperationResult> SaveImageByUrl(string externalUrl, BlobContainer container)
        {

            var info = BlobInfo.GetInfoByUrl(externalUrl);

            var uniqueName = GetUniqueImageName(info.Extension);

            var webRequest = WebRequest.Create(externalUrl);

            using (var response = await webRequest.GetResponseAsync())
            {
                using (var content = response.GetResponseStream())
                {
                    if (content == null)
                    {
                        return new OperationResult(
                            OperationResultStatus.Warning,
                            "Content of specified source is empty");
                    }

                    var contentType = response.ContentType;

                    var manager = GetManager(container);
                    if (manager == null)
                    {
                        return new OperationResult(OperationResultStatus.Warning, "Incorrect input");
                    }

                    var url = await manager.AddBlockBlob(uniqueName, content, contentType);

                    return new OperationResult(OperationResultStatus.Success, url);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fixes the image URL on update.
        /// </summary>
        /// <param name="oldUrl">The old URL.</param>
        /// <param name="newUrl">The new URL.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        internal async Task<string> SaveImageToProductionBlobOnUpdate(
            string oldUrl, 
            string newUrl, 
            BlobContainer container)
        {
            if (string.Equals(oldUrl, newUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                return oldUrl;
            }

            await DeleteImage(oldUrl);

            return await SaveImageToProductionBlobOnAdd(newUrl, container);
        }

        internal async Task SaveImagesToProductionBlobOnUpdate(
            List<string> oldUrls,
            List<string> newUrls,
            BlobContainer container)
        {
            var anyOldUrls = oldUrls.AnyValues();
            var anyNewUrls = newUrls.AnyValues();

            var notTouch =
                anyOldUrls && anyNewUrls
                ? oldUrls.Intersect(newUrls).ToIList()
                : new List<string>();

            if (anyOldUrls)
                foreach (var oldUrl in oldUrls)
                    if (!notTouch.Contains(oldUrl))
                        await DeleteImage(oldUrl);

            if (anyNewUrls)
                foreach (var newUrl in newUrls)
                    if (!notTouch.Contains(newUrl))
                        await SaveImageToProductionBlobOnAdd(newUrl, container);
        }

        /// <summary>
        ///     Fixes the image URL on add.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        internal async Task<string> SaveImageToProductionBlobOnAdd(string imageUrl, BlobContainer container)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return string.Empty;
            }

            var result = await SaveImageByUrl(imageUrl, container);
            if (result.Status != OperationResultStatus.Success)
        {
            Trace.TraceError("SaveImageToProductionBlobOnAdd error: {0}", result.Description);
                return imageUrl;
            }

            await DeleteImage(imageUrl);

            return result.Description;
        }

        private static AzureBlobManager GetManager(BlobContainer container)
        {
            if (container == BlobContainer.Undefined)
            {
                return null;
            }

            return new AzureBlobManager(container);
        }

        /// <summary>
        ///     Gets the images.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>Task{IEnumerable{System.String}}.</returns>
        private async Task<List<string>> GetImages(BlobContainer container)
        {
            var manager = GetManager(container);
            if (manager == null)
            {
                return null;
            }

            return await manager.GetBlockBlobs();
        }

        private string GetUniqueImageName(string extension)
        {
            return $"image_{Guid.NewGuid().ToString("N")}{extension}";
        }

        #endregion
    }
}