namespace SpyderByteAPI.Models.Imgur
{
    public record PostImageResponse
    {
        public string Url { get; set; } = string.Empty;

        public string ImageId { get; set; } = string.Empty;
    }
}
