﻿using Microsoft.IdentityModel.Tokens;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Services.Data.Abstract;
using SpyderByteAPI.Services.Storage.Abstract;
using System.IO.Compression;

namespace SpyderByteAPI.Services.Data
{
    public class DataService : IDataService
    {
        private const string DATABASE_FOLDER_NAME = "Databases"; // OJN: Shouldn't really be hard-coded.

        private readonly ILogger<DataService> logger;
        private readonly IConfiguration configuration;
        private readonly IStorageService storageService;

        public DataService(ILogger<DataService> logger, IConfiguration configuration, IStorageService storageService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.storageService = storageService;
        }

        public async Task<IDataResponse<bool>> Backup()
        {
            // Get the backup file name from the configuration.
            var zipFileName = configuration["Database:BackupZipFileName"];
            if (zipFileName.IsNullOrEmpty())
            {
                logger.LogError($"Failed to find database backup file name in configuration.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }

            // Get the backup file extension from the configuration.
            var backupFileExtension = configuration["Database:BackupFileExtension"];
            if (backupFileExtension.IsNullOrEmpty())
            {
                logger.LogError($"Failed to find database backup file extension in configuration.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }

            // Get the file directory.
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FOLDER_NAME);
            if (!Directory.Exists(filePath))
            {
                logger.LogError($"Failed to find file directory {filePath}.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            // Get the files from the directory.
            var files = Directory.GetFiles(filePath);
            if (files.IsNullOrEmpty())
            {
                logger.LogError($"Failed to find any files in directory {filePath}.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            try
            {
                // Create temporary copies of the database files.
                foreach (var file in files)
                {
                    using (var inputFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var outputFile = new FileStream($"{file}{backupFileExtension}", FileMode.Create))
                        {
                            inputFile.CopyTo(outputFile);
                        }
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    // Create a ZIP and add the files.
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var archiveEntry = archive.CreateEntryFromFile($"{file}{backupFileExtension}", Path.GetFileName(file));
                            // OJN: Error check this.
                        }
                    }

                    // Upload the ZIP to storage.
                    var response = await storageService.Upload(zipFileName!, memoryStream);
                    if (response.Result != ModelResult.OK)
                    {
                        logger.LogError($"Failed to upload ZIP file.");
                        return new DataResponse<bool>(false, response.Result);
                    }

                    return new DataResponse<bool>(true, ModelResult.OK);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to find database backup file extension in configuration.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }
            finally
            {
                // Delete any temporary files.
                var temporaryFiles = Directory.GetFiles(filePath, $"*{backupFileExtension}");
                foreach (var temporaryFile in temporaryFiles)
                {
                    File.Delete(temporaryFile);
                }
            }
        }
    }
}
