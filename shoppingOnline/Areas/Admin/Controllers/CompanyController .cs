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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index() //create index page
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
               //retreive Update Company from database
            return View(objCompanyList);
        }

        //.............................................................this action method to create new Company button
        public IActionResult Upsert(int? id) // comibine of update and insert in one functinality
        {
            if (id == null || id == 0)
            {
                //to create
                return View(new Company());
            }
            else
            {
                //to update
                Company companyObj = _unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj);
            }
            

        }
        [HttpPost] // create buttons to post and to add new Company in the database
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid) //that will check if model state we have Company object is here if
                                    //that object is valid that means it goes to Company.cs and
                                    //accept automadation if it is required....be pupulited if it is not it is not go to database (if statement is for)
            {
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }

                //what are to do the changes in the database
                _unitOfWork.Save(); //here to save the changes in the database
                TempData["success"] = "Company Created successfully"; //show a message at the top 
                return RedirectToAction("Index"); //(to write the different controller like index, Company or more)

            }
            else
            {
              
                return View(CompanyObj);
            }
            

        }

        //...............................................................Create action Method to create edit button
        

        //...............................................................Create action Method to create delete button
       

        #region API CALLS

        [HttpGet]
        public IActionResult GetALL()
        { List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();


            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
