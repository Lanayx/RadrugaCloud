using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.InfrastructureTools.Memory
{
    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;

    public class ImageProvider:IImageProvider
    {
        public Task<bool> DeleteImage(string photoUrl)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteImages(IEnumerable<string> photoUrls)
        {
            return Task.FromResult(true);
        }

        public Task<string> SaveImage(Core.CommonModels.FileData fileContent, Core.Enums.BlobContainer typeOfBlob)
        {
            return Task.FromResult("imagePath");
        }

        public Task<int> GetTempImagesCount()
        {
            return Task.FromResult(10);
        }

        /// <summary>
        ///     Saves the image by URL.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="container">The container.</param>
        /// <returns>Task{System.String}.</returns>
        public Task<OperationResult> SaveImageByUrl(string externalUrl, BlobContainer container)
        {
            return Task.FromResult(new OperationResult(OperationResultStatus.Success));
        }
    }
}
