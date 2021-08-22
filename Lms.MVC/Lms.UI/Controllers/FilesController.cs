using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Filters;
using Lms.MVC.UI.Utilities.FileHandler;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Lms.MVC.UI.Controllers
{
    public class FilesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IMapper mapper;

        private readonly IUoW uoW;

        private readonly long fileSizeLimit;

        private readonly ILogger<FilesController> logger;

        // Get the default form options so that we can use them to set the default limits for
        // request body data.
        private static readonly FormOptions defaultFormOptions = new FormOptions();

        private readonly string[] permittedExtensions = { ".pdf", ".png" };

        public FilesController(IUoW uoW, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<FilesController> logger, IConfiguration config)
        {
            this.uoW = uoW;
            this.mapper = mapper;
            this.userManager = userManager;
            this.logger = logger;
            fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        public async Task<IActionResult> DownloadFile(int? id)
        {
            if (id == null)
            {
                return View();
            }
            var requestFile = await uoW.FileRepository.GetFileByIdAsync((int)id);
            if (requestFile == null)
            {
                return View();
            }

            // Don't display the untrusted file name in the UI. HTML-encode the value.
            return File(requestFile.Content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(requestFile.UntrustedName));
        }

        // Add confirmation
        public async Task<IActionResult> DeleteFile(int? id, string CMAType, string originUrl, string userId)
        {
            if (CMAType == "activity") CMAType = "Activitie";
            if (id == null)
            {
                return RedirectToAction("Index", CMAType + "s");
            }

            var RemoveFile = await uoW.FileRepository.GetFileByIdAsync((int)id);

            if (RemoveFile != null && uoW.UserRepository.GetAllFilesByUserId(userId).Result.Contains(RemoveFile))
            {
                uoW.FileRepository.Remove(RemoveFile);
                await uoW.CompleteAsync();
            }

            return Redirect(originUrl);
        }

        // The following upload methods:
        //
        // 1. Disable the form value model binding to take control of handling potentially large files.
        //
        // 2. Typically, antiforgery tokens are sent in request body. Since we don't want to read
        // the request body early, the tokens are sent via headers. The antiforgery token filter
        // first looks for tokens in the request header and then falls back to reading the body.

        [HttpPost]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDatabase()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");

                // Log error

                return BadRequest(ModelState);
            }

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();
            var trustedFileNameForDisplay = string.Empty;
            var untrustedFileNameForStorage = string.Empty;
            var streamedFileContent = Array.Empty<byte>();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        untrustedFileNameForStorage = contentDisposition.FileName.Value;

                        // Don't trust the file name sent by the client. To display the file name,
                        // HTML-encode the value.
                        trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);

                        streamedFileContent =
                            await FileHelpers.ProcessStreamedFile(section, contentDisposition,
                                ModelState, permittedExtensions, fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else if (MultipartRequestHelper
                        .HasFormDataContentDisposition(contentDisposition))
                    {
                        // Don't limit the key name length because the multipart headers length
                        // limit is already in effect.
                        var key = HeaderUtilities
                            .RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);

                        if (encoding == null)
                        {
                            ModelState.AddModelError("File",
                                $"The request couldn't be processed (Error 2).");

                            // Log error

                            return BadRequest(ModelState);
                        }

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();

                            if (string.Equals(value, "undefined",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount >
                                defaultFormOptions.ValueCountLimit)
                            {
                                // Form key count limit of _defaultFormOptions.ValueCountLimit is exceeded.
                                ModelState.AddModelError("File",
                                    $"The request couldn't be processed (Error 3).");

                                // Log error

                                return BadRequest(ModelState);
                            }
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and read the headers
                // for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to the model
            var formData = new FormData();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);
            var bindingSuccessful = await TryUpdateModelAsync(formData, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                ModelState.AddModelError("File",
                    "The request couldn't be processed (Error 5).");

                // Log error

                return BadRequest(ModelState);
            }

            // **WARNING!** In the following example, the file is saved without scanning the file's
            // contents. In most production scenarios, an anti-virus/anti-malware scanner API is
            // used on the file before making the file available for download or for use by other
            // systems. For more information, see the topic that accompanies this sample app.

            var file = new Core.Entities.ApplicationFile()
            {
                Content = streamedFileContent,
                UntrustedName = untrustedFileNameForStorage,
                Description = formData.Description,
                Size = streamedFileContent.Length,
                UploadDT = DateTime.UtcNow
            };
            if (User.IsInRole("Student") && formData.CMAType=="activity")
            {
                file.Assignment = true;
            }
            else
            {
                file.Assignment = false;
            }

            // This needs to be updated for other types.. should be able to be done in a way that
            // the program knows that type it comes from
            var cmaType = formData.CMAType.ToLower();
            if (cmaType == "course")
            {
                var course = uoW.CourseRepository.GetCourseWithFilesAsync(formData.CMAId).Result;
                course.Files.Add(file);
            }
            else if (cmaType == "module")
            {
                var module = uoW.ModuleRepository.GetModuleWithFilesAsync(formData.CMAId).Result;
                module.Files.Add(file);
            }
            else if (cmaType == "activity")
            {
                var activity = uoW.ActivityRepository.GetActivityWithFilesAsync(formData.CMAId).Result;
                activity.Files.Add(file);
            }
            var user = uoW.UserRepository.GetUserWithFilesByIdAsync(formData.UserId).Result;
            user.Files.Add(file);

            await uoW.FileRepository.AddAsync(file);
            await uoW.CompleteAsync();

            return Created(nameof(FilesController), null);
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }

    public class FormData
    {
        public string Description { get; set; }

        public int CMAId { get; set; }

        public string UserId { get; set; }

        public string CMAType { get; set; }

        public bool Assignment { get; set; }
    }
}