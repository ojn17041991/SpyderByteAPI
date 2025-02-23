using Microsoft.FeatureManagement;
using Moq;
using SpyderByteAPI.Controllers;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Services.Data.Abstract;

namespace SpyderByteTest.API.DataControllerTests.Helpers
{
    public class DataControllerHelper
    {
        public DataController Controller;

        private bool allowDatabaseBackups = true;
        private ModelResult currentModelResult = ModelResult.OK;

        public DataControllerHelper()
        {
            var dataService = new Mock<IDataService>();
            dataService.Setup(x =>
                x.BackupAsync()
            ).Returns(() => {
                return Task.FromResult(
                    new DataResponse<bool>(
                        true,
                        currentModelResult
                    )
                    as IDataResponse<bool>
                );
            });
            
            dataService.Setup(x =>
                x.CleanupAsync()
            ).Returns(() => {
                return Task.FromResult(
                    new DataResponse<bool>(
                        true,
                        currentModelResult
                    )
                    as IDataResponse<bool>
                );
            });

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x =>
                x.IsEnabledAsync(
                    It.IsAny<string>()
                )
            ).Returns((string featureName) =>
            {
                if (featureName == FeatureFlags.AllowDatabaseBackups)
                {
                    return Task.FromResult(allowDatabaseBackups);
                }
                return Task.FromResult(false);
            });

            var modelResources = new Mock<IStringLookup<ModelResult>>();
            modelResources.Setup(x =>
                x.GetResource(
                    It.IsAny<ModelResult>()
                )
            ).Returns((ModelResult modelResult) => {
                return string.Empty;
            });

            Controller = new(dataService.Object, featureManager.Object, modelResources.Object);
        }

        public void SetCurrentModelResult(ModelResult currentModelResult)
        {
            this.currentModelResult = currentModelResult;
        }

        public void SetAllowDatabaseBackups(bool allowDatabaseBackups)
        {
            this.allowDatabaseBackups = allowDatabaseBackups;
        }
    }
}
