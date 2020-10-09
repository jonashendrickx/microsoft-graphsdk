using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraphSdkDemo.Contracts.Utility.Archiver;
using GraphSdkDemo.SharePoint.Api.Contracts.File;
using GraphSdkDemo.SharePoint.Contracts.Enums;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GraphSdkDemo.SharePoint.Api.Controllers
{
    public class FileController
    {
        private readonly IFileService _fileService;
        private readonly ISettingsProvider _settings;

        public FileController(IFileService fileService, ISettingsProvider settings)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [FunctionName("GetDocumentLibraries")]
        public async Task<IActionResult> GetDocumentLibraries(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files/documentlibraries")] HttpRequest req,
            ILogger log)
        {
            var result = await _fileService.GetDocumentLibrariesAsync(_settings.GetSiteId());

            if (result.Status == ConnectionStatus.Success)
            {
                var models = result.DocumentLibraries.Select(DocumentLibraryModelInfo.FromView).ToArray();
                return new OkObjectResult(models);
            }
            else
            {
                return new BadRequestObjectResult(result.ErrorMessage);
            }
        }

        [FunctionName("GetFiles")]
        public async Task<IActionResult> GetFiles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files/documentlibraries/{documentLibraryId}")] HttpRequest req,
            string documentLibraryId, ILogger log)
        {
            var state = new ModelStateDictionary();

            if (string.IsNullOrEmpty(documentLibraryId))
                state.AddModelError(nameof(documentLibraryId), "Missing or empty.");

            if (!state.IsValid)
                return new BadRequestObjectResult(state);

            var result = await _fileService.GetFilesAsync(_settings.GetSiteId(), documentLibraryId);

            if (result.Status == ConnectionStatus.Success)
            {
                var models = result.Files.Select(FileModelInfo.FromView).ToArray();
                return new OkObjectResult(models);
            }
            else
            {
                return new BadRequestObjectResult(result.ErrorMessage);
            }
        }

        [FunctionName("DownloadFile")]
        public async Task<IActionResult> Download(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files/documentlibraries/{documentLibraryId}/driveItems/{driveItemId}")] HttpRequest req, string documentLibraryId, string driveItemId, ILogger log)
        {
            var state = new ModelStateDictionary();

            if (string.IsNullOrEmpty(documentLibraryId))
                state.AddModelError(nameof(documentLibraryId), "Missing or empty.");

            if (string.IsNullOrEmpty(driveItemId))
                state.AddModelError(nameof(driveItemId), "Missing or empty.");

            if (!state.IsValid)
                return new BadRequestObjectResult(state);

            var file = await _fileService.DownloadAsync(_settings.GetSiteId(), documentLibraryId, driveItemId);
            var result = new FileContentResult(file.Content, "application/octet-stream");
            result.FileDownloadName = file.Name;
            return result;


        }

        [FunctionName("DownloadFiles")]
        public async Task<IActionResult> DownloadMultiple(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "files/documentlibraries/{documentLibraryId}/driveItems")] HttpRequest req, string documentLibraryId, ILogger log)
        {
            var body = await (new StreamReader(req.Body)).ReadToEndAsync();
            var inputModel = JsonConvert.DeserializeObject<DownloadFilesInputModel>(body);

            var state = new ModelStateDictionary();

            if (string.IsNullOrEmpty(documentLibraryId))
                state.AddModelError(nameof(documentLibraryId), "Missing or empty.");

            if (inputModel?.DriveItems == null || !inputModel.DriveItems.Any() || inputModel.DriveItems.Any(x => string.IsNullOrEmpty(x)))
                state.AddModelError(nameof(inputModel.DriveItems), "Missing identifiers of files to download.");

            if (!state.IsValid)
                return new BadRequestObjectResult(state);

            var tasks = inputModel.DriveItems.Select(x => _fileService.DownloadAsync(_settings.GetSiteId(), documentLibraryId, x)).ToArray();
            Task.WaitAll(tasks);

            var archiverInputModel = tasks
                .Select(x => new FileInputModel { Name = x.Result.Name, Content = x.Result.Content })
                .ToArray();
            var archive = Archiver.Archive(archiverInputModel);

            var result = new FileContentResult(archive.Content, "application/octet-stream");
            result.FileDownloadName = "file.zip";
            return result;
        }

    }
}
