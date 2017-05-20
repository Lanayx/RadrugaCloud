namespace Core.Interfaces.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels;
    using Core.CommonModels.Results;
    using Core.Enums;

    /// <summary>
    /// Interface IImageProvider
    /// </summary>
    public interface IImageProvider
    {
        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="photoUrl">The photo URL.</param>
        /// <returns>Task{System.Boolean}.</returns>
        Task<bool> DeleteImage(string photoUrl);

        /// <summary>
        /// Deletes the images.
        /// </summary>
        /// <param name="photoUrls">The photo urls.</param>
        /// <returns>Task{System.Boolean}.</returns>
        Task<bool> DeleteImages(IEnumerable<string> photoUrls);

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        /// <param name="typeOfBlob">The type of BLOB.</param>
        /// <returns>Task{System.String}.</returns>
        Task<string> SaveImage(FileData fileContent, BlobContainer typeOfBlob);

        /// <summary>
        /// Gets the temp images count.
        /// </summary>
        /// <returns>Task{System.Int32}.</returns>
        Task<int> GetTempImagesCount();

        /// <summary>
        ///     Saves the image by URL.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        Task<OperationResult> SaveImageByUrl(string externalUrl, BlobContainer container);
    }
}
