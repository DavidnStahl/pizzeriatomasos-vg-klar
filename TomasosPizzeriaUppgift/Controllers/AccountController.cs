﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeriaUppgift.Models;
using TomasosPizzeriaUppgift.Services;
using TomasosPizzeriaUppgift.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeriaUppgift.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Kund model)
        {
            var validUsername = AccountService.Instance.CheckUserNameIsValid(model, Request);
            if (ModelState.IsValid && validUsername == true)
            {
                var result = RoleAdminService.Instance.Identity("create",new LoginViewModel(),model,userManager,signInManager,Request,Response,User, roleManager);
                if (result == true) { return RedirectToAction("Index", "Home"); }
                return View("Register", model);
              
            }
            else if(ModelState.IsValid && validUsername == false)
            {
                ViewBag.Data = "Användarnamn Upptaget";
                return View(model);
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {           
            if (ModelState.IsValid)
            {
                var result = RoleAdminService.Instance.Identity("signin", model, new Kund(), userManager, signInManager, Request, Response,User,roleManager);
                if (result == true) { return RedirectToAction("Index", "Home"); }
                ViewBag.Error = "Inloggning Misslyckades";
                return View(model);
            }
            ViewBag.Error = "Inloggning Misslyckades";
            return View(model);
        }        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [Authorize]
        [HttpGet]

        public ActionResult Update()
        {
            var customer = AccountService.Instance.GetInloggedCustomerInfo(Request);
            ViewBag.Message = "Din personliga information";
            return View(customer);
        }   
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(Kund model)
        {
        var valid = AccountService.Instance.CheckUserNameIsValid(model, Request);
        var customer = AccountService.Instance.GetInloggedCustomerInfo(Request);
        if (ModelState.IsValid && valid == true)
        {             
                var result = RoleAdminService.Instance.Identity("update", new LoginViewModel(), model, userManager, signInManager, Request, Response,User,roleManager);
                if (result == true) { return RedirectToAction("Index", "Home"); }               
                return View(nameof(Update), customer);
            }
        else if (ModelState.IsValid && valid == false)
        {
            ViewBag.Message = "Användarnamn Upptaget";
            return View(nameof(Update), customer);
        }        
            return View(nameof(Update));        
        }
    }
}
