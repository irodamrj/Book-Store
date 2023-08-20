using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

			return View(productList);
		}
		public IActionResult Upsert(int? id)
		{

			//ViewData["CategoryList"] = CategoryList;
			ProductVM productVM = new ProductVM
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				Product = new Product(),
			};
			if (id == null || id == 0)
				return View(productVM);
			else
			{
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}

		}

		[HttpPost]
		public IActionResult Upsert(ProductVM obj, IFormFile? file)
		{

			if (obj.Product.Title == "test")
			{
				//if no key value, its gonna be ModelOnly error
				ModelState.AddModelError("", "Invalid value for field Title");
			}
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\products");
					if (string.IsNullOrEmpty(obj.Product.ImgURL))
					{
						//delete old img
						var oldImgPath = Path.Combine(wwwRootPath, obj.Product.ImgURL.TrimStart('\\'));
						if (System.IO.File.Exists(oldImgPath))
						{
							System.IO.File.Delete(oldImgPath);
						}
					}
					using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					obj.Product.ImgURL = @"\images\products\" + filename;
				}

				if (obj.Product.Id == 0)
				{
					_unitOfWork.Product.Add(obj.Product);
				}
				else
				{
					_unitOfWork.Product.Update(obj.Product);
				}


				_unitOfWork.Save();
				TempData["success"] = "Product created successfully";
				//controller can be omitted since this controller is in the same controller
				return RedirectToAction("Index", "Product");
			}
			else
			{
				//create dropdown even if a error occurs
				obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				});
				return View(obj);
			}

		}

		//public IActionResult Edit(int? id)
		//{
		//	if (id == null || id == 0)
		//		return NotFound();
		//	//find only works with id
		//	//Category category = _context.Categories.Find(id);
		//	Product? product = _unitOfWork.Product.Get(c => c.Id == id);
		//	//Category? category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
		//	if (product == null)
		//		return NotFound();

		//	return View(product);
		//}

		//[HttpPost]
		//public IActionResult Edit(Product obj)
		//{

		//	if (ModelState.IsValid)
		//	{
		//		_unitOfWork.Product.Update(obj);
		//		_unitOfWork.Save();
		//		//controller can be omitted since this controller is in the same controller
		//		TempData["success"] = "Product updated successfully";
		//		return RedirectToAction("Index", "Product");
		//	}
		//	return View();
		//}


		//public IActionResult Delete(int? id)
		//{
		//	if (id == null || id == 0)
		//		return NotFound();
		//	//find only works with id
		//	//Category category = _context.Categories.Find(id);
		//	Product? product = _unitOfWork.Product.Get(c => c.Id == id);
		//	//Category? category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
		//	if (product == null)
		//		return NotFound();

		//	return View(product);
		//}

		//[HttpPost, ActionName("Delete")]
		//public IActionResult DeletePOST(int? id)
		//{
		//	Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
		//	if (obj == null)
		//		return NotFound();
		//	_unitOfWork.Product.Remove(obj);
		//	_unitOfWork.Save();
		//	TempData["success"] = "Product deleted successfully";
		//	return RedirectToAction("Index", "Product");
		//}
		#region API Call
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = productList });
		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
			if (obj == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImgURL.TrimStart('\\'));
			if (System.IO.File.Exists(oldImgPath))
			{
				System.IO.File.Delete(oldImgPath);
			}

			_unitOfWork.Product.Remove(obj);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete successfull" });
		}
		#endregion

	}
}
