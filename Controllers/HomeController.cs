using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bankaccounts.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace bankaccounts.Controllers
{
    public class HomeController : Controller
    {
        private UserContext dbContext;
        //injecting context service
        public HomeController(UserContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

       [HttpGet("login")]
       public IActionResult Login(){
           return View("Login");
       }

       [HttpPost("register")]
       public IActionResult Register(User user){
           if(ModelState.IsValid){
               User exists = dbContext.users.FirstOrDefault(u => u.Email == user.Email);

               if(exists != null){
                    ModelState.AddModelError("Email", "An account with this email already exists");
                    return View("Index");
                }
                else{
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Add(user);
                    //    Console.WriteLine("USER ADDED============= " + user.FirstName);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("GetAccount", new {userId = user.UserId});
                    //GetAccount here is method/function name - not Route name
                }
           }
           else{
               return View("Index");
           }
       }

       [HttpGet("success")]
       public IActionResult GetAccount(){
           int? id = HttpContext.Session.GetInt32("UserId");
           User user = dbContext.users.FirstOrDefault(u => u.UserId == id);
           TempData["name"] = user.FirstName;
            decimal balance = dbContext.transactions.Where(t => t.UserId == user.UserId).Sum(t => (decimal)t.Amount);
            TempData["balance"] = balance.ToString("0.##");

            List<Transaction> transactions = dbContext.transactions.Where(t => t.UserId == user.UserId).ToList();
            ViewBag.transactions = transactions;
           return View("Account");
       }

        [HttpPost("success")]
        public IActionResult ProcessLogin(LoginUser loginUser){
            if(ModelState.IsValid){
                User userInDb = dbContext.users.FirstOrDefault(u => u.Email == loginUser.Email);
                if(userInDb == null){
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.Password);
                if(result == 0){
                    ModelState.AddModelError("Password", "Incorrect Password");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                User user = dbContext.users.FirstOrDefault(u => u.UserId == userInDb.UserId);
                TempData["name"] = user.FirstName;
                decimal balance = dbContext.transactions.Where(t => t.UserId == userInDb.UserId).Sum(t => (decimal)t.Amount);
                TempData["balance"] = balance.ToString("0.##");
                return RedirectToAction("GetAccount", new {userId = user.UserId}); //action here is the Method or Function GetAccount()
            }
            else{
                return View("Login");
            }
        }

        [HttpPost("posttransaction")]
        public IActionResult PostTransaction(decimal Amount){
            int? userid = HttpContext.Session.GetInt32("UserId");
            User a_user = dbContext.users.FirstOrDefault(u => u.UserId == userid);
            TempData["name"] = a_user.FirstName;
            decimal balance = dbContext.transactions.Where(t => t.UserId == userid).Sum(t => (decimal)t.Amount);
            TempData["balance"] = balance.ToString("0.##");

            
            if(ModelState.IsValid){
                decimal diff = (decimal)Amount + balance;
                if(diff < 0){
                    ModelState.AddModelError("Amount", "You can't withdraw more than current balance");
                    return View("Account");
                }
                else{
                    Transaction transaction = new Transaction{
                        Amount = (decimal)Amount,
                        UserId = a_user.UserId
                    };
                    a_user.Balance = diff;
                    TempData["balance"] = a_user.Balance.ToString("0.##");
                    dbContext.transactions.Add(transaction);
                    dbContext.SaveChanges();

                    return RedirectToAction("GetAccount", new {userId = a_user.UserId});

                }
            }
            else{
                return View("Account");
            }
        }

       [HttpGet("logout")]
       public IActionResult Logout(){
           HttpContext.Session.Clear();
           return View("Index");
       }
    }
}
