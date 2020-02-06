using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeriaUppgift.Interface;
using Microsoft.EntityFrameworkCore;

using TomasosPizzeriaUppgift.ViewModels;

namespace TomasosPizzeriaUppgift.Models.IdentityLogic
{
    public class IdentityUserLogic : IIdentity
    {

        public async Task<IdentityResult> CreateUserIdentity(Kund model, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, HttpRequest request, HttpResponse response, RoleManager<IdentityRole> roleManager)
        {
            var user = new IdentityUser { UserName = model.AnvandarNamn, NormalizedUserName = model.AnvandarNamn };
            var result = await userManager.CreateAsync(user, model.Losenord);

            if (result.Succeeded)
            {
                var role = roleManager.Roles.FirstOrDefault(r => r.Name == "RegularUser");
                await userManager.AddToRoleAsync(user, role.Name);
                var loginmodel = new LoginViewModel();
                loginmodel.Username = model.AnvandarNamn;
                loginmodel.Password = model.Namn;
                Services.AccountService.Instance.SaveUser(model);
                Services.AccountService.Instance.SetCustomerCache(loginmodel, request, response);

                await signInManager.SignInAsync(user, isPersistent: false);
                return result;
            }

            return result;
        }
        public async Task<UsersViewModel> GetAllUsers(List<Kund> customers, RoleManager<IdentityRole> roleManager, string roleToSearch, UserManager<IdentityUser> userManager)
        {
            var usersregitrered = new List<IdentityUser>();
            using (TomasosContext db = new TomasosContext())
            {
                usersregitrered = db.Users.ToList();
            }
            var userViewModel = new UsersViewModel();
            foreach (var customer in customers)
            {
                var userInfo = new UserRole();
                var user = usersregitrered.FirstOrDefault(r => r.UserName == customer.AnvandarNamn);
                var role = await userManager.GetRolesAsync(user);
                var rolename = roleManager.FindByNameAsync(role[0]);
                userInfo.RoleName = role[0];
                userInfo.Username = user.UserName;
                userInfo.RoleID = rolename.Result.Id;
                userInfo.Adress = customer.Gatuadress;
                userInfo.Name = customer.Namn;
                userInfo.UserID = user.Id;
                if (userInfo.RoleName == roleToSearch || roleToSearch == "All")
                {
                    userViewModel.Customers.Add(userInfo);
                }               
            }
            var list = roleManager.Roles;
            foreach (var item in list)
            {
                userViewModel.Roles.Add(item.Name);
            }
            return userViewModel;
        }
        public async Task<UpdateRoleViewModel> GetUserIdentityByUsername(string userName, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var user = await userManager.FindByNameAsync(userName);
            
            var model = new UpdateRoleViewModel();
            model.UserID = user.Id;
            var identityrole = await roleManager.FindByIdAsync(user.Id);
            model.RoleName = identityrole.Name;
            model.Username = user.UserName;
            var roles = roleManager.Roles;
            foreach (var item in roles)
            {
                model.Roles.Add(item.Name);
            }
            return model;
        }

        public async Task<SignInResult> SignInIdentity(LoginViewModel model, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, HttpRequest request, HttpResponse response)
        {

            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password,
                                           model.RememberMe, false);

            if (result.Succeeded)
            {
                Services.AccountService.Instance.SetCustomerCache(model, request, response);
                return result;
            }

            return result;
        }
        public async Task<IdentityResult> UpdateUserIdentity(Kund model, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, HttpRequest request, HttpResponse response, System.Security.Claims.ClaimsPrincipal user)
        {
            var identityuser = await userManager.GetUserAsync(user);
            identityuser.UserName = model.AnvandarNamn;
            await userManager.UpdateNormalizedUserNameAsync(identityuser);
            var customer = Services.AccountService.Instance.GetInloggedCustomerInfo(request);
            var result = await userManager.ChangePasswordAsync(identityuser, customer.Losenord, model.Losenord);
            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(identityuser);
                Services.AccountService.Instance.UpdateUser(model, request);
                return result;
            }
            return result;
        }
        public void UpdateRoleForUser(string changeRoleTo, string id, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var userToDelete = db.UserRoles.FirstOrDefault(r => r.UserId == id);
                db.UserRoles.Remove(userToDelete);
                db.SaveChanges();
                var userRoles = db.Roles.FirstOrDefault(r => r.Name == changeRoleTo);
                userToDelete.RoleId = userRoles.Id;
                userToDelete.UserId = id;
                db.UserRoles.Add(userToDelete);
                db.SaveChanges();


            }
        }
        public async Task<IdentityResult> CreateRole(RoleManager<IdentityRole> roleManager, CreateRoleViewModel model)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = model.RoleName
            };

            IdentityResult result = await roleManager.CreateAsync(identityRole);

            return result;
        }

        public bool CheckIfUserISPremiumUser(RoleManager<IdentityRole> roleManager, System.Security.Claims.ClaimsPrincipal user)
        {
            return user.IsInRole("PremiumUser");  
        }

       
    }
}
