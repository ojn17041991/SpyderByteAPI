﻿using SpyderByteAPI.Attributes;
using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PatchGame
    {
        [Required]
        public Guid Id { get; set; }

        [RegularExpression(@"[^<>\\/]{1,50}", ErrorMessage = "Name does not meet validation requirements.")]
        public string? Name { get; set; }

        public GameType? Type { get; set; }

        public string? Url { get; set; }

        [FileUploadValidation(ErrorMessage = "Image file can only be of type png.")]
        public IFormFile? Image { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
