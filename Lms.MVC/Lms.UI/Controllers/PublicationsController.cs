using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AutoMapper;

using Lms.API.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Filters;
using Lms.MVC.UI.Models.ViewModels.PublicationViewModels;
using Lms.MVC.UI.Utilities.Pagination;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Lms.MVC.UI.Controllers
{
    public class PublicationsController : Controller
    {
        private string Baseurl = "https://localhost:44302/";

        private readonly IMapper mapper;

        private readonly IUoW uow;

        public PublicationsController(IMapper mapper, IUoW uow)
        {
            this.mapper = mapper;
            this.uow = uow;
        }

        [Authorize]
        public async Task<IActionResult> Index(string search, string sortOrder, int page)
        {
            if (search != null)
            {
                page = 1;
            }

            ViewData["CurrentFilter"] = search;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["SubjectSortParm"] = sortOrder == "subject" ? "subject_desc" : "subject";
            ViewData["AuthorSortParm"] = sortOrder == "author" ? "author_desc" : "author";

            List<Publication> publications = new List<Publication>();

            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = string.IsNullOrEmpty(search) ?
                    await client.GetAsync("api/Publications") :
                    await client.GetAsync("api/Publications/search/" + search);

                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var PublicationResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list
                    publications = JsonConvert.DeserializeObject<List<Publication>>(PublicationResponse);

                    switch (sortOrder)
                    {
                        case "title_desc":
                            publications = publications.OrderByDescending(p => p.Title).ToList();
                            break;

                        case "subject":
                            publications = publications.OrderBy(p => p.Subject.Title).ToList();
                            break;

                        case "subject_desc":
                            publications = publications.OrderByDescending(p => p.Subject.Title).ToList();
                            break;

                        case "author":
                            publications = publications
                                .OrderBy(p => p.Authors.FirstOrDefault() != null ?
                                p.Authors.FirstOrDefault().LastName : "").ToList();
                            break;

                        case "author_desc":
                            publications = publications
                                .OrderByDescending(p => p.Authors.FirstOrDefault() != null ?
                                p.Authors.FirstOrDefault().LastName : "").ToList();
                            break;

                        default:
                            publications = publications.OrderBy(p => p.Title).ToList();
                            break;
                    }

                    var paginatedResult = publications.AsQueryable().GetPagination(page, 10);

                    return View(paginatedResult);
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpGet]
        [Authorize]
        [ModelNotNull]
        public async Task<IActionResult> Edit(int? id)
        {
            using (var client = new HttpClient())
            {
                var publication = new Publication();
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res =
                    await client.GetAsync("api/Publications/" + id);

                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var PublicationResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list
                    publication = JsonConvert.DeserializeObject<Publication>(PublicationResponse);

                    return View(publication);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPut]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publication publication)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res =
                    await client.PutAsJsonAsync<Publication>("api/Publications" + id, publication);

                if (Res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var createPublicationViewModel = new CreatePublicationViewModel();
            createPublicationViewModel.Subjects = uow.PublicationRepository.GetSubjects();

            return View(createPublicationViewModel);
        }

        [HttpPost]
        [ModelValid, ModelNotNull]
        public async Task<IActionResult> Create(CreatePublicationViewModel createPublicationViewModel)
        {
            if (createPublicationViewModel.ReleaseDate < createPublicationViewModel.AuthorBirthdate)
            {
                ModelState.AddModelError("AuthorBirthdate", "Publication Date Must Be After Author's Date of Birth.");
                createPublicationViewModel.Subjects = uow.PublicationRepository.GetSubjects();
                return View(createPublicationViewModel);
            }

            createPublicationViewModel.Authors = new List<Author>();
            createPublicationViewModel.Authors.Add(uow.PublicationRepository.CreateAuthor(createPublicationViewModel.AuthorFirstName, createPublicationViewModel.AuthorLastName, createPublicationViewModel.AuthorBirthdate));
            createPublicationViewModel.Subject = uow.PublicationRepository.CreateSubject(createPublicationViewModel.SubjectTitle);

            //model.Author = new Author() { FirstName = model.AuthorFirstName, LastName = model.AuthorFirstName }; //TODO MOVE TO EXTENSION
            //model.Subject = new Subject() { Title = model.SubjectTitle }; //TODO MOVE TO EXTENSION

            mapper.Map<Publication>(createPublicationViewModel);//TODO Fix Mapping issue

            using (var client = new HttpClient())
            {
                // Building request url
                client.BaseAddress = new Uri(Baseurl);

                // Fixing request head
                client.DefaultRequestHeaders.Clear();

                // Define request format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Build Request
                var jsonData = JsonConvert.SerializeObject(createPublicationViewModel);
                var url = Baseurl + "api/Publications/create";

                var response = await client.PostAsJsonAsync(url, jsonData);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Baseurl + $"api/publications/{id}");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Sends request and ensures response
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                // Converts Json response to ViewModel
                var content = await response.Content.ReadAsStringAsync();
                content = content.TrimStart('\"');
                content = content.TrimEnd('\"');
                content = content.Replace("\\", "");
                var model = JsonConvert.DeserializeObject<DeletePublicationViewModel>(content);

                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, Baseurl + $"api/publications/{id}");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}