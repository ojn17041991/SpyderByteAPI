namespace SpyderByteServices.Models.Data
{
    public record StorageFile
    {
        public string FileName { get; init; } = string.Empty;

        public DateTimeOffset CreatedDate { get; init; }
    }
}
