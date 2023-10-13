using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "ADMIN,SUPERADMIN")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryManager;

        public CategoryController(IRepository<Category> categoryManager)
        {
            _categoryManager = categoryManager;
        }

        // GET: Category
        public IActionResult Index()
        {
			try
			{
				List<Category> categories = (List<Category>)_categoryManager.GetAll();

				return View(categories);
			}
			catch
			{
				return Problem("Something went wrong.");
			}
        }

        // GET: Category/Details/5
        public IActionResult Details(int? id)
        {
            if (id is null)
                return NotFound();

			Category? category = _categoryManager.GetById((int)id);

			if (category is null)
				return NotFound();

			return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryManager.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Category? category = _categoryManager.GetById((int) id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                Category? originalCategory = _categoryManager.GetById(category.Id);
                if (originalCategory is null)
                    return NotFound();
                try
                {
                    originalCategory.Name = category.Name;
                    originalCategory.Description = category.Description;
                    _categoryManager.Update(originalCategory);
					return RedirectToAction(nameof(Index));
				}
                catch
                {
					return Problem("Something went wrong.");
				}
            }
            return View(category);
        }

		// POST: Category/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Category? category = _categoryManager.GetById((int)id);

            if (category == null)
                return NotFound();

            try
            {
                _categoryManager.Delete(category);
				return RedirectToAction(nameof(Index));
			}
            catch
            {
				return Problem("Something went wrong.");
			}
           
        }
    }
}
