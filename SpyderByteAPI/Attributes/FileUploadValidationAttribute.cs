using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Attributes
{
    public class FileUploadValidationAttribute : ValidationAttribute
    {
        public bool ExistingResource = false;

        private readonly string pngExtension = ".png";
        private readonly byte[] pngBytes = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        public override bool IsValid(object? value)
        {
            IFormFile formFile = (IFormFile)value!;
            if (formFile == null) return ExistingResource;
            if (Path.GetExtension(formFile.FileName) != pngExtension) return false;

            using (var reader = new BinaryReader(formFile.OpenReadStream()))
            {
                var headerBytes = reader.ReadBytes(pngBytes.Length);
                return pngBytes.SequenceEqual(headerBytes);
            }
        }
    }
}
