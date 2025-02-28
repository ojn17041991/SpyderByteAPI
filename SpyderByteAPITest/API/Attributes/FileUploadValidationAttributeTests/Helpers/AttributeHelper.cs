using AutoFixture;
using Microsoft.AspNetCore.Http;

namespace SpyderByteTest.API.Attributes.FileUploadValidationAttributeTests.Helpers
{
    public class AttributeHelper
    {
        private readonly Fixture fixture;

        public AttributeHelper()
        {
            fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public IFormFile GeneratePngFile()
        {
            byte[] pngBytes = new byte[]
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            };

            Stream stream = new MemoryStream(pngBytes);

            IFormFile formFile = new FormFile(
                stream,
                0,
                pngBytes.Length,
                "test",
                "test.png"
            );

            return formFile;
        }

        public IFormFile GeneratePngFileWithIncorrectHeaderBytes()
        {
            byte[] pngBytes = new byte[]
            {
                0x25, 0x50, 0x44, 0x46
            };

            Stream stream = new MemoryStream(pngBytes);

            IFormFile formFile = new FormFile(
                stream,
                0,
                pngBytes.Length,
                "test",
                "test.png"
            );

            return formFile;
        }

        public IFormFile GeneratePdfFile()
        {
            byte[] pdfBytes = new byte[]
            {
                0x25, 0x50, 0x44, 0x46
            };

            Stream stream = new MemoryStream(pdfBytes);

            IFormFile formFile = new FormFile(
                stream,
                0,
                pdfBytes.Length,
                "test",
                "test.pdf"
            );

            return formFile;
        }
    }
}
