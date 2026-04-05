namespace SpyderByteServices.Models.Imgur
{
    public record PostImageResponse
    {
        public string Url { get; init; } = string.Empty;

        public string ImageId { get; init; } = string.Empty;
    }
}
