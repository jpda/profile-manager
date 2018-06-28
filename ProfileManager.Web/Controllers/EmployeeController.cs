using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileManager.AppService;
using ProfileManager.Entities;

namespace ProfileManager.Web.Controllers
{
    public class EmployeeController : Controller
    {
        IEmployeeRepository _repo;
        IFaceInfoProvider _faceProvider;
        public EmployeeController(IEmployeeRepository repo, IFaceInfoProvider faceProvider)
        {
            _repo = repo;
            _faceProvider = faceProvider;
        }

        // GET: Employee
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var model = await _repo.GetAllEmployeesAsync();
            return View(model);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var model = await _repo.GetEmployeeAsync(id);
            return View(model);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // todo: lots of discrete activities here, should split them out to handle failure of specific tasks in a more robust way (e.g., compensating txns)
        // todo: add proxy model for Employee so we can use things like IFormFile as a property in the model and map back to Employee the entity without leaking HTTP and MVC-specific stuff to the entity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee e, IFormFile photoFile)
        {
            try
            {
                if(photoFile == null)
                {
                    ModelState.AddModelError(string.Empty, "No photo uploaded.");
                }
                using (var stream = photoFile.OpenReadStream())
                {
                    // copying to a memory stream ensures the whole file stream is copied
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        e.PhotoBytes = memoryStream.ToArray();
                    }

                }
                var facesInSubmittedPhoto = await _faceProvider.DetectFacesFromPhotoAsync(e.PhotoBytes);
                if (facesInSubmittedPhoto.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "No faces in photo.");
                    return View();
                }

                if (facesInSubmittedPhoto.Count > 1)
                {
                    ModelState.AddModelError(string.Empty, "Too many faces in photo.");
                    return View();
                }

                if (!ModelState.IsValid) return View();
                var employee = await _repo.CreateEmployeeAsync(e);
                //var personId = await _faceProvider.CreatePersonInPersonGroupAsync(employee.ImmutableId, employee.CompanyId, $"{employee.FirstName} {employee.LastName}");
                //var persistedFaceId = await _faceProvider.AddPersonFaceAsync(personId, e.PhotoBytes);
                //employee.PersonGroupPersonId = personId;
                //employee.PersistedFaceId = persistedFaceId;
                //await _repo.UpdateEmployeeAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _repo.GetEmployeeAsync(id);
            return View(model);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}