namespace Services.BL
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.Interfaces.Repositories;

    /// <summary>
    /// Class ImageService
    /// </summary>
    public class ImageService
    {
        /// <summary>
        /// The _image provider
        /// </summary>
        private readonly IImageProvider _imageProvider;

        /// <summary>
        /// The _user repository
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="imageProvider">The image provider.</param>
        /// <param name="userRepository">The user repository.</param>
        public ImageService(IImageProvider imageProvider, IUserRepository userRepository)
        {
            _imageProvider = imageProvider;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Saves the temporary image.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task{System.String}.</returns>
        public Task<string> SaveTemporaryImage(FileData file)
        {
            return _imageProvider.SaveImage(file, BlobContainer.TempStorage);
        }

        /// <summary>
        /// Saves the avatar image.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="file">The file.</param>
        /// <returns>Task{System.String}.</returns>
        public async Task<string> SaveAvatarImage(string userId, FileData file)
        {
            var result = await _imageProvider.SaveImage(file, BlobContainer.AvatarImages);
            if (String.IsNullOrEmpty(result))
            {
                var user = await _userRepository.GetUser(userId);
                if (String.IsNullOrEmpty(user.AvatarUrl))
                {
                    await _imageProvider.DeleteImage(user.AvatarUrl);
                }
            }
            return result;
        }

        /// <summary>
        /// Saves the kind action image.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task{System.String}.</returns>
        public Task<string> SaveKindActionImage(FileData file)
        {
            return _imageProvider.SaveImage(file, BlobContainer.KindActionImages);
        }

        /// <summary>
        /// Saves the proof image.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task{System.String}.</returns>
        public Task<string> SaveProofImage(FileData file)
        {
            return _imageProvider.SaveImage(file, BlobContainer.ProofImages);
        }

        /// <summary>
        /// Gets the temp images count.
        /// </summary>
        /// <returns>Task{System.Int32}.</returns>
        public Task<int> GetTempImagesCount()
        {
            return _imageProvider.GetTempImagesCount();
        }
    }
}
