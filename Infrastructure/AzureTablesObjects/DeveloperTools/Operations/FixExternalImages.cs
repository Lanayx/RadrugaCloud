namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Infrastructure.AzureTablesObjects.TableEntities;
    using Infrastructure.InfrastructureTools;
    using Infrastructure.InfrastructureTools.Azure;

    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class DeleteAllTables
    /// </summary>
    internal class FixExternalImages : IDeveloperOperation
    {
        private const string MissionTemplate = "Mission '{0}' (id = {1}) update error code: {2}";

        private const string MissionDraftTemplate = "MissionDraft '{0}' (id = {1}) update error code: {2}";

        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            var sb = new StringBuilder();
            var helper = new DeveloperHelper();
            var imageProvider = IocConfig.GetConfiguredContainer().Resolve<IImageProvider>();
            await ProcessMissions(imageProvider, helper, sb);
            await ProcessMissionDrafts(imageProvider, helper, sb);

            if (sb.Length > 0)
            {
                return new OperationResult(OperationResultStatus.Warning, sb.ToString());
            }

            return new OperationResult(OperationResultStatus.Success);
        }

        private async Task ProcessMissionDrafts(IImageProvider imageProvider, DeveloperHelper helper, StringBuilder sb)
        {
            var missionDraftReference = helper.GetTableReference(AzureTableName.MissionDrafts);
            var missionDrafts = await missionDraftReference.ExecuteQueryAsync(new TableQuery<MissionDraftAzure>());
            var missionDraftsForUpdate =
                missionDrafts.Where(
                    m =>
                    m.RowKey == AzureTableConstants.DraftRowKey
                    && BlobInfo.GetInfoByUrl(m.PhotoUrl).Kind == BlobKind.External);
            foreach (var missionDraftAzure in missionDraftsForUpdate)
            {
                var imageResult = await imageProvider.SaveImageByUrl(missionDraftAzure.PhotoUrl, BlobContainer.MissionDraftImages);
                if (imageResult.Status != OperationResultStatus.Success)
                {
                    sb.AppendLine(
                        string.Format(
                            MissionDraftTemplate,
                            missionDraftAzure.Name,
                            missionDraftAzure.Id,
                            imageResult.Description));
                    continue;
                }

                missionDraftAzure.PhotoUrl = imageResult.Description;
                TableOperation updateOperation = TableOperation.Replace(missionDraftAzure);
                var result = await missionDraftReference.ExecuteAsync(updateOperation);
                if (!result.HttpStatusCode.EnsureSuccessStatusCode())
                {
                    sb.AppendLine(
                        string.Format(MissionDraftTemplate, missionDraftAzure.Name, missionDraftAzure.Id, result.HttpStatusCode));
                }
            }
        }

        private async Task ProcessMissions(IImageProvider imageProvider, DeveloperHelper helper, StringBuilder sb)
        {
            var missionReference = helper.GetTableReference(AzureTableName.Missions);
            var missions = await missionReference.ExecuteQueryAsync(new TableQuery<MissionAzure>());
            var missionsForUpdate =
                missions.Where(
                    m =>
                    m.RowKey == AzureTableConstants.MissionRowKey
                    && BlobInfo.GetInfoByUrl(m.PhotoUrl).Kind == BlobKind.External);
            foreach (var missionAzure in missionsForUpdate)
            {
                var imageResult = await imageProvider.SaveImageByUrl(missionAzure.PhotoUrl, BlobContainer.MissionImages);
                if (imageResult.Status != OperationResultStatus.Success)
                {
                    sb.AppendLine(string.Format(MissionTemplate, missionAzure.Name, missionAzure.Id, imageResult.Description));
                    continue;
                }

                missionAzure.PhotoUrl = imageResult.Description;
                TableOperation updateOperation = TableOperation.Replace(missionAzure);
                var result = await missionReference.ExecuteAsync(updateOperation);
                if (!result.HttpStatusCode.EnsureSuccessStatusCode())
                {
                    sb.AppendLine(string.Format(MissionTemplate, missionAzure.Name, missionAzure.Id, result.HttpStatusCode));
                }
            }
        }
    }
}