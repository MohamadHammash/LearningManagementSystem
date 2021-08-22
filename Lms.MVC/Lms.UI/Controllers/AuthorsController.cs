using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Lms.MVC.UI.Models.ViewModels.AuthorViewModels;
using Lms.MVC.UI.Utilities.Pagination;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Lms.MVC.UI.Controllers
{
    public class AuthorsController : Controller
    {
        private HttpClient httpClient;

        public AuthorsController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44302/");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> Index(string search, string sort, int page)
        {
            // Builds request to API
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Sends request and ensures response
            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            // Converts Json response to ViewModel
            var content = await response.Content.ReadAsStringAsync();
            content = content.TrimStart('\"');
            content = content.TrimEnd('\"');
            content = content.Replace("\\", "");
            var model = JsonConvert.DeserializeObject<IEnumerable<IndexAuthorViewModel>>(content);

            // Checks Search
            if (search != null) page = 1;

            // Search
            if (!String.IsNullOrWhiteSpace(search))
            {
                model = model.Where(a => a.FirstName.ToLower().StartsWith(search.ToLower())
                        || a.LastName.ToLower().Contains(search.ToLower())
                        || a.Age.ToString().Contains(search));
            }

            // Build ViewData
            ViewData["CurrentFilter"] = search;
            ViewData["CurrentSort"] = sort;
            ViewData["FNameSortParam"] = String.IsNullOrEmpty(sort) ? "Name_desc" : "";
            ViewData["LNameSortParam"] = sort == "LName" ? "LName_desc" : "LName";
            ViewData["AgeSortParam"] = sort == "Age" ? "Age_desc" : "Age";
            ViewData["FNameAge"] = sort == "FNAgeA" ? "FNAgeD" : "FNAgeA";
            ViewData["FNameLName"] = sort == "FLName" ? "FLName_desc" : "FLName";
            ViewData["LNameAge"] = sort == "LNAgeA" ? "LNAgeD" : "LNAgeA";
            ViewData["LNameFName"] = sort == "LNFNameA" ? "LNFNameD" : "LNFNameA";
            ViewData["AgeFName"] = sort == "AgeFNameA" ? "AgeFNameD" : "AgeFNameA";
            ViewData["AgeLName"] = sort == "AgeLNameA" ? "AgeLNameD" : "AgeLNameA";

            


            // Sort by order
            
            
                switch (sort)
                {
                    case "Name_desc":
                        model = model.OrderByDescending(a => a.FirstName);
                        break;
                    case "LName":
                        model = model.OrderBy(a => a.LastName);
                        break;
                    case "LName_desc":
                        model = model.OrderByDescending(a => a.LastName);
                        break;
                    case "Age":
                        model = model.OrderBy(a => a.Age);
                        break;
                    case "Age_desc":
                        model = model.OrderByDescending(a => a.Age);
                        break;
                    case "FNAgeD":// First Name then Age Descending

                        model = model.OrderBy(a => a.FirstName)
                        .ThenByDescending(a => a.Age);
                        break;
                    case "FNAgeA": // First Name then Age Ascending

                        model = model.OrderBy(a => a.FirstName)
                        .ThenBy(a => a.Age);
                        break;
                    case "FLName_desc":

                        model = model.OrderBy(a => a.FirstName)
                        .ThenByDescending(a => a.LastName);
                        break;

                    case "FLName":
                        model = model.OrderBy(a => a.FirstName)
                        .ThenBy(a => a.LastName);
                        break;

                    case "LNAgeD":
                        model = model.OrderBy(a => a.LastName)
                        .ThenByDescending(a => a.Age);
                        break;

                    case "LNAgeA":
                        model = model.OrderBy(a => a.LastName)
                        .ThenBy(a => a.Age);
                        break;

                    case "LNFNameD":
                        model = model.OrderBy(a => a.LastName)
                        .ThenByDescending(a => a.FirstName);
                        break;

                    case "LNFNameA":
                        model = model.OrderBy(a => a.LastName)
                        .ThenBy(a => a.FirstName);
                        break;

                    case "AgeFNameD":
                        model = model.OrderBy(a => a.Age)
                        .ThenByDescending(a => a.FirstName);
                        break;

                    case "AgeFNameA":
                        model = model.OrderBy(a => a.Age)
                        .ThenBy(a => a.FirstName);
                        break;

                    case "AgeLNameD":
                        model = model.OrderBy(a => a.Age)
                        .ThenByDescending(a => a.LastName);
                        break;

                    case "AgeLNameA":
                        model = model.OrderBy(a => a.Age)
                        .ThenBy(a => a.LastName);
                        break;

                    default:
                        model = model.OrderBy(a => a.FirstName);
                        break;
                }
            
            
            
            // Paginated Results
            var paginatedResult = model.AsQueryable().GetPagination(page, 10);

            return View(paginatedResult);
        }
    }
}