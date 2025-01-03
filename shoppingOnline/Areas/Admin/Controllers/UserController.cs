﻿using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DataAccess.Data;
using ShoppingOnline.Models;
using ShoppingOnline.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingOnline.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace shoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
            
        }

        //...............................................................................................
        public IActionResult Index() //create index page
        {
            return View();
        }

        //.............................................................this action method to create new Company button
       
        //...............................................................Create action Method to create delete button
       

        #region API CALLS

        [HttpGet]
        public IActionResult GetALL()
        { 
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u=>u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }
            }

            return Json(new { data = objUserList });
        }

        //.............................................................................................//

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
           


            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
