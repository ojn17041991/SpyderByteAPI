namespace SpyderByteAPI.Authorisation.Abstract
{
    public interface ISecretAccessor
    {
        public string ApiKey { get; }
    }
}
