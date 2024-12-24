using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DataAccess.Data;
using ShoppingOnline.Models;
using ShoppingOnline.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingOnline.Utility;


namespace shoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index() //create index page
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
               //retreive Update product from database
            return View(objProductList);
        }

        //.............................................................this action method to create new product button
        public IActionResult Upsert(int? id) // comibine of update and insert in one functinality
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
              .GetAll().Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()

              }),
            Product = new Product()
            };
            if (id == null || id == 0)
            {
                //to create
                return View(productVM);
            }
            else
            {
                //to update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
            

        }
        [HttpPost] // create buttons to post and to add new product in the database
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid) //that will check if model state we have product object is here if
                                    //that object is valid that means it goes to product.cs and
                                    //accept automadation if it is required....be pupulited if it is not it is not go to database (if statement is for)
            {
                 string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) //delete old images
                    {
                        //Delete the old image
                        var oldImagePath = 
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    //upload new image
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName; //update image url
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                //what are to do the changes in the database
                _unitOfWork.Save(); //here to save the changes in the database
                TempData["success"] = "Product Created successfully"; //show a message at the top 
                return RedirectToAction("Index"); //(to write the different controller like index, product or more)

            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()

                 });
                return View(productVM);
            }
            

        }

        //...............................................................Create action Method to create edit button
        

        //...............................................................Create action Method to create delete button
       

        #region API CALLS

        [HttpGet]
        public IActionResult GetALL()
        { List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath =
                Path.Combine(_webHostEnvironment.WebRootPath, 
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();


            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
