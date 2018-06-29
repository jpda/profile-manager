using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileManager.AppService;
using ProfileManager.Entities;

namespace ProfileManager.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repo;
        private readonly IFaceInfoProvider _faceProvider;
        private readonly IBlobProvider _blobProvider;
        public EmployeeController(IEmployeeRepository repo, IFaceInfoProvider faceProvider, IBlobProvider blobProvider)
        {
            _repo = repo;
            _faceProvider = faceProvider;
            _blobProvider = blobProvider;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var model = await _repo.GetAllEmployeesAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var model = await _repo.GetEmployeeAsync(id);
            if (model.PhotoPath != null)
            {
                model.PhotoPathSas = _blobProvider.GetReadSasForBlob(model.PhotoPath);
            }
            return View(model);
        }

        public async Task<IActionResult> DetailsByEmployeeId(string id)
        {
            var model = await _repo.GetEmployeeByEmployeeIdAsync(id);
            if (model.PhotoPath != null)
            {
                model.PhotoPathSas = _blobProvider.GetReadSasForBlob(model.PhotoPath);
            }
            return View(nameof(Details), model);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // todo: lots of discrete activities here, should split them out to handle failure of any individual tasks in a more robust way (e.g., compensating txns)
        // todo: add proxy model for Employee so we can use things like IFormFile as a property in the model and map back to Employee the entity without leaking HTTP and MVC-specific stuff to the entity
        // todo: reconsider how much context the controller should have around employee creation - e.g., moving this into the employee service rather than the controller
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee e, IFormFile photoFile)
        {
            try
            {
                if (photoFile == null)
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
                var personId = await _faceProvider.CreatePersonInPersonGroupAsync(employee.ImmutableId, employee.CompanyId, $"{employee.FirstName} {employee.LastName}");
                var persistedFaceId = await _faceProvider.AddPersonFaceAsync(personId, e.PhotoBytes);
                employee.PersonGroupPersonId = personId;
                employee.PersistedFaceId = persistedFaceId;
                employee.PhotoPath = await _repo.SaveEmployeePhoto(e);
                await _repo.UpdateEmployeeAsync(employee);
                return RedirectToAction(nameof(Details), new { id = employee.ImmutableId });
            }
            catch
            {
                return View();
            }
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _repo.GetEmployeeAsync(id);
            if (model.PhotoPath != null)
            {
                model.PhotoPathSas = _blobProvider.GetReadSasForBlob(model.PhotoPath);
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee e)
        {
            try
            {
                var storedEmployee = await _repo.GetEmployeeAsync(e.ImmutableId);
                if (storedEmployee.ImmutableId != e.ImmutableId || storedEmployee.CompanyId != e.CompanyId)
                {
                    ModelState.AddModelError(string.Empty, "Something didn't line up.");
                    return RedirectToAction(nameof(Edit), new { id = e.ImmutableId });
                }
                // no partial updates in cosmos yet
                // only update the properties we allow to be updated; fname, lname and department
                storedEmployee.FirstName = e.FirstName;
                storedEmployee.LastName = e.LastName;
                storedEmployee.Department = e.Department;
                await _repo.UpdateEmployeeAsync(storedEmployee);
                return RedirectToAction(nameof(Details), new { id = e.ImmutableId });
            }
            catch
            {
                return View();
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _repo.GetEmployeeAsync(id);
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Employee e)
        {
            try
            {
                // since the model isn't full, and we can't trust people anyway, just refetch the user
                var employee = await _repo.GetEmployeeAsync(e.ImmutableId);
                // todo: be more resilient - e.g., most of these ops should be queued off somewhere else
                await _faceProvider.DeletePersonFromPersonGroupAsync(employee.PersonGroupPersonId);
                await _blobProvider.DeleteBlobAsync(employee.PhotoPath);
                await _repo.DeleteEmployeeAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}