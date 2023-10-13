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
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "ADMIN,SUPERADMIN")]
    public class PhotoController : Controller
    {
        private readonly PhotoManager _photoManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<Category> _categoryManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        public PhotoController(
            PhotoManager photoRepository, 
            IRepository<Category> categoryRepository, 
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment hostEnvironment
            )
        {
            _photoManager = photoRepository;
            _userManager = userManager;
            _categoryManager = categoryRepository;
            _hostEnvironment = hostEnvironment;
        }

        // GET: PhotoController
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                List<Photo> photos = new List<Photo>();
                if (User.IsInRole("SUPERADMIN"))
                    photos = (List<Photo>) _photoManager.GetAll();
                else if (User.IsInRole("ADMIN"))
                    photos = (List<Photo>) _photoManager.GetAllFiltered(photo => photo.UserId == _userManager.GetUserId(User));
                return View(photos);
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        // GET: PhotoController/Details/photo-slug
        public async Task<IActionResult> Details(string slug)
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
                    PhotoFormModel formModel = new PhotoFormModel { Photo = photo, UserName = user?.UserName ?? "unknown" };
                    return View(formModel);
                }
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

                return View(formModel);
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
                PrepareForValidation(formData);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Create), formData);
                }

                if(formData.ImgFile is not null)
                {
                    string fileName = formData.Photo.ImgPath!;
                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploads, fileName);
                    using (FileStream newFileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formData.ImgFile.CopyTo(newFileStream);
                    }
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
                string originalFile = formData.Photo.ImgPath!;
                PrepareForValidation(formData);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Edit), formData);
                }

                Photo originalPhoto = _photoManager.GetBySlug(slug)!;

                originalPhoto.Categories!.Clear();

                if (formData.ImgFile is not null)
                {
                    string fileName = formData.Photo.ImgPath!;
                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploads, fileName);
                    using (FileStream newFileStream = new FileStream(filePath, FileMode.Create)){
                        formData.ImgFile.CopyTo(newFileStream);
                    }
                    string oldFilePath = Path.Combine(uploads, originalFile);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

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
                originalPhoto.ImgPath = formData.Photo.ImgPath;

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
                string fileName = markedPhoto.ImgPath!;
                string uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploads, fileName);
                System.IO.File.Delete(filePath);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Something went wrong.");
            }
        }

        private void PrepareForValidation(PhotoFormModel formData)
        {
            formData.Photo.Slug = Helper.GetSlugFromString(formData.Photo.Title);

            if (formData.ImgFile is not null)
            {
                formData.Photo.ImgPath = GetUniqueFileName(formData.ImgFile.FileName);
            }
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

        private string GetUniqueFileName(string fileName)
        {

            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                        + "_"
                        + Guid.NewGuid().ToString().Substring(0, 4)
                        + Path.GetExtension(fileName);

        }
    }
}
