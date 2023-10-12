using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using net_il_mio_fotoalbum.Utility;
using Microsoft.SqlServer.Server;

namespace net_il_mio_fotoalbum.Controllers
{
    [Route("Admin")]
    [Authorize(Roles = "ADMIN,SUPERADMIN")]
    public class PhotoController : Controller
    {
        private readonly PhotoManager _photoManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<Category> _categoryManager;
        public PhotoController(PhotoManager photoRepository, IRepository<Category> categoryRepository, UserManager<IdentityUser> userManager)
        {
            _photoManager = photoRepository;
            _userManager = userManager;
            _categoryManager = categoryRepository;
        }

        // GET: PhotoController
        [HttpGet("Index")]
        public ActionResult Index()
        {
            try
            {
                List<Photo> photos = new List<Photo>();
                if (User.IsInRole("SUPERADMIN"))
                    photos = (List<Photo>) _photoManager.GetAll();
                else if (User.IsInRole("ADMIN"))
                    photos = (List<Photo>) _photoManager.GetAllFiltered(photo => photo.UserId == _userManager.GetUserId(User));
                return View("Index", photos);
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // GET: PhotoController/Details/photo-slug
        public ActionResult Details(string slug)
        {
            try
            {
                Photo? photo = _photoManager.GetBySlug(slug);

                if (photo is null)
                {
                    return NotFound("Can't find the photo.");
                }
                if (photo.UserId == _userManager.GetUserId(User) || User.IsInRole("SUPERADMIN"))
                    return View("Details", photo);
                else
                    return Unauthorized();

            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // GET: PhotoController/Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                PhotoFormModel formModel = new PhotoFormModel {
                    Photo = new Photo(),
                    UserName = _userManager.GetUserName(User)
                };
                formModel.Photo.UserId = _userManager.GetUserId(User);
                PrepareFormModel( formModel );

                return View("Create", formModel);
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // POST: PhotoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhotoFormModel formData)
        {
            try
            {
                if (formData.Photo.UserId != _userManager.GetUserId(User))
                {
                    return Unauthorized();
                }

                ModelState.Clear();
                PrepareForValidation(formData.Photo);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Create), formData);
                }
                
                formData.Photo.Categories = new List<Category>();
                if(formData.SelectedCategoriesId != null)
                {
                    foreach(string selectedId in formData.SelectedCategoriesId)
                    {
                        int selectedCategoryId = int.Parse(selectedId);
                        Category? fetchedCategory = _categoryManager.GetById(selectedCategoryId);
                        if (fetchedCategory != null)
                            formData.Photo.Categories.Add(fetchedCategory);
                    }
                }

                _photoManager.Add(formData.Photo);
                return RedirectToAction(nameof(Details), new { slug = formData.Photo.Slug });
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // GET: PhotoController/Edit/photo-slug
        public async Task<IActionResult> Edit(string slug)
        {
            try
            {
                Photo? photo = _photoManager.GetBySlug(slug);

                if (photo is null)
                {
                    return NotFound("Can't find the photo.");
                }
                if (photo.UserId == _userManager.GetUserId(User) || User.IsInRole("SUPERADMIN"))
                {
                    IdentityUser? user = await _userManager.FindByIdAsync(photo.UserId!);
                    PhotoFormModel formModel = new PhotoFormModel { Photo = photo, UserName = user!.UserName };
                    PrepareFormModel(formModel);

                    return View(nameof(Edit), formModel);
                }
                else
                    return Unauthorized();

            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // POST: PhotoController/Edit/photo-slug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string slug, PhotoFormModel formData)
        {
            try
            {
                if (!(formData.Photo.UserId == _userManager.GetUserId(User) || User.IsInRole("SUPERADMIN")))
                {
                    return Unauthorized();
                }
                
                ModelState.Clear();
                PrepareForValidation(formData.Photo);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Edit), formData);
                }

                Photo originalPhoto = _photoManager.GetBySlug(slug)!;

                originalPhoto.Categories!.Clear();

                if (formData.SelectedCategoriesId != null)
                {
                    foreach (string id in formData.SelectedCategoriesId)
                    {
                        int selectedCategoryId = int.Parse(id);
                        Category fetchedCategory = _categoryManager.GetById(selectedCategoryId)!;
                        originalPhoto.Categories.Add(fetchedCategory);
                    }
                }

                originalPhoto.Title = formData.Photo.Title;
                originalPhoto.Slug = formData.Photo.Slug;
                originalPhoto.Description = formData.Photo.Description;
                originalPhoto.Visibility = formData.Photo.Visibility;

                _photoManager.Update(originalPhoto);

                return RedirectToAction(nameof(Details), new {slug = formData.Photo.Slug});
            }
            catch
            {
                return Problem("Something went wrong.");
            }

        }

        // POST: PhotoController/Delete/photo-slug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string slug)
        {
            try
            {
                Photo? markedPhoto = _photoManager.GetBySlug(slug);
                if (markedPhoto == null) 
                {
                    return NotFound("Can't find the photo.");
                }
                if (!(markedPhoto.UserId == _userManager.GetUserId(User)) && !User.IsInRole("SUPERADMIN"))
                {
                    return Unauthorized();
                }
                _photoManager.Delete(markedPhoto);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        private void PrepareForValidation(Photo photo)
        {
            photo.Slug = Helper.GetSlugFromString(photo.Title);
        }
        private void PrepareFormModel(PhotoFormModel formData)
        {
            // CATEGORIES LIST
            List<SelectListItem> selectCategories = new List<SelectListItem>();
            List<Category> categories = (List<Category>) _categoryManager.GetAll();
            foreach (Category category in categories)
            {
                selectCategories.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
            }
            formData.Categories = selectCategories;
        }
    }
}
