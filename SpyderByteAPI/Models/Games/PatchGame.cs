﻿using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PatchGame
    {
        [Required]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public GameType? Type { get; set; }

        public string? Url { get; set; }

        public IFormFile? Image { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
