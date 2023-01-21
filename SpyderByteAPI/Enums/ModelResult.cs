namespace SpyderByteAPI.Enums
{
    public enum ModelResult
    {
        OK = 1000,
        Created = 1001,

        Error = 2000,
        NotFound = 2001,
        IDGivenForIdentityField = 2002,
        IDMismatchInPut = 2003
    }
}
