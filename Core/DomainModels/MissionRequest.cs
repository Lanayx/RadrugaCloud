namespace Core.DomainModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Core.Enums;

    /// <summary>
    /// The mission request.
    /// </summary>
    public class MissionRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the mission id.
        /// </summary>
        public string MissionId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public MissionRequestStatus Status { get; set; }

        /// <summary>
        /// Number of stars recieven
        /// </summary>
        /// <value>The stars count.</value>
        [Range(1, 3)]
        [Display(Name = "Количество звезд")]
        public byte? StarsCount { get; set; }


        /// <summary>
        /// Gets or sets the decline reason.
        /// </summary>
        /// <value>
        /// The decline reason.
        /// </value>
        public string DeclineReason { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        /// <value>
        /// The last update date.
        /// </value>
        [Display(Name = "Дата последнего изменения")]
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the proof.
        /// </summary>
        [Display(Name = "Доказательства")]
        [Required]
        public MissionProof Proof { get; set; }

        // ----------------------------------

        /// <summary>
        /// Gets or sets the mission.
        /// </summary>
        public virtual Mission Mission { get; set; }
      
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}