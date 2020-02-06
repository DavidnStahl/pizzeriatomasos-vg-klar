﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TomasosPizzeriaUppgift.Models;
using TomasosPizzeriaUppgift.ViewModels;
using TomasosPizzeriaUppgift.Services;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizzeriaUppgift.Controllers
{
    public class DishesAdminController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Menu()
        {
            var model = MenuService.Instance.GetMenuInfo();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteDish(int id)
        {
            Services.DishesAdminService.Instance.DeleteDish(id);
            var model = MenuService.Instance.GetMenuInfo();
            return View("Menu", model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewDish()
        {
            var model = MenuService.Instance.GetMenuInfo();
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewDish(MenuPage model)
        {
            model = DishesAdminService.Instance.CheckMatrattsValidation(model);
            var newModel = model.NewDish;
            if (model.NewDish.Matratt.MatrattNamn.Length > 1 && model.NewDish.SelectedListItem.Count != 0 && model.NewDish.Matratt.MatrattTyp != 0 && model.MatrattsnamnTaken == false)
            {
                DishesAdminService.Instance.CreateDish(model.NewDish);
                return RedirectToAction("Menu");
            }
            model = MenuService.Instance.GetMenuInfo();
            model = DishesAdminService.Instance.SetValidtion(model, newModel);
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddIngrediens()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AddIngrediens(Produkt produkt)
        {
            var result = DishesAdminService.Instance.AddIngrediens(produkt);
            if (result == true)
            {
                return RedirectToAction("Menu");
            }
            ViewBag.Message = "true";
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveIngrediens(int id)
        {
            DishesAdminService.Instance.RemoveIngrediens(id);
            return RedirectToAction("Menu");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditDish(int id)
        {
            var model = DishesAdminService.Instance.GetDishToUpdate(id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult EditDish(UpdateDishViewModel model)
        {
            var oldmodel = DishesAdminService.Instance.GetDishToUpdate(model.id);
            if (ModelState.IsValid)
            {
                DishesAdminService.Instance.UpdateDish(model);
                return RedirectToAction("Menu");
            }
            return View(oldmodel);
        }
    }
}
