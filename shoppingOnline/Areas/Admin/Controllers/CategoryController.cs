using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DataAccess.Data;
using ShoppingOnline.Models;
using ShoppingOnline.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ShoppingOnline.Utility;
using Microsoft.AspNetCore.Authorization;


namespace shoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //[Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index() //create index page
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();  //retreive category from database
            return View(objCategoryList);
        }

        //.............................................................this action method to create new category button
        //[Authorize(Roles = SD.Role_Admin)]
        public IActionResult Create() // 
        {
            return View();

        }
        [HttpPost] // create buttons to post and to add new category in the database
        //[Authorize(Roles = SD.Role_Admin)]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name.");
            }

            if (ModelState.IsValid) //that will check if model state we have category object is here if
                                    //that object is valid that means it goes to category.cs and
                                    //accept automadation if it is required....be pupulited if it is not it is not go to database (if statement is for)
            {

                _unitOfWork.Category.Add(obj); //what are to do the changes in the database
                _unitOfWork.Save(); //here to save the changes in the database
                TempData["success"] = "Category Created successfully"; //show a message at the top 
                return RedirectToAction("Index"); //(to write the different controller like index, category or more)

            }
            return View();

        }

        //...............................................................Create action Method to create edit button
        public IActionResult Edit(int? id) // if we want to build edit page and edit button
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //Category? categoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();

            }
            return View(categoryFromDb);
        }
        [HttpPost] // create buttons to post and to update  category in the database
        public IActionResult Edit(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name.");
            //}

            if (ModelState.IsValid) //that will check if model state we have category object is here if
                                    //that object is valid that means it goes to category.cs and
                                    //accept automadation if it is required....be pupulited if it is not it is not go to database (if statement is for)
            {

                _unitOfWork.Category.Update(obj); //what are to do the changes in the database
                                                  //what are to update the changes in the database
                _unitOfWork.Save();  //here to save the changes in the database
                TempData["success"] = "Category Updated successfully";
                return RedirectToAction("Index"); //(to write the different controller like index, category or more)

            }
            return View();

        }

        //...............................................................Create action Method to create delete button
        public IActionResult Delete(int? id) // if we want to build edit page and edit button
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();

            }
            return View(categoryFromDb); //retreive category
        }
        [HttpPost, ActionName("Delete")]  // create buttons to post and to update  category in the database
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);  //to delete first to find the category
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted successfully";
            return RedirectToAction("Index");


        }
    }
}
