using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class WedController : Controller
    {

// INJECTIONS

        private WeddingContext dbContext;
        public WedController(WeddingContext context)
        {
            dbContext = context;
        }
        public User ActiveUser
        {
            get{return dbContext.UserTable.Where(u => u.UserId == HttpContext.Session.GetInt32("d")).FirstOrDefault();}
        }


// GET: /Register Render/
        [HttpGet]
        [Route("/")]
        public IActionResult RegisterView()
        {
            return View();
        }


// GET: /Login Render/
        [HttpGet]
        [Route("/login")]
        public IActionResult LoginView()
        {
            return View();
        }


// GET: /Add Wedding Render/
        [HttpGet]
        [Route("/addwedding")]
        public IActionResult AddWedding()
        {
            return View();
        }


// POST: /Register Process/
        [HttpPost]
        [Route("/registerprocess")]
        public IActionResult RegisterProcess(User user)
        {
            if (ModelState.IsValid)
            {
                var dbuser = dbContext.UserTable.FirstOrDefault(u => u.Email == user.Email);
                if(dbContext.UserTable.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");    
                    return View("RegisterView");                
                }
                // password hasher
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                // saving in db
                
                dbContext.Add(user);
                dbContext.SaveChanges();
                // setting stuff in session
                HttpContext.Session.SetInt32("UserID", user.UserId);
                // 
                HttpContext.Session.SetString("Firstname", user.FirstName);
                HttpContext.Session.SetString("Lastname", user.LastName);
                
                return RedirectToAction("Dashboard");
            }
            return View("RegisterView");
        }


// POST: /Login Process/
        [HttpPost]
        [Route("/loginprocess")]
        public IActionResult LoginProcess(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                // checks if the email submitted is already registered in DB
                var dbuser = dbContext.UserTable.FirstOrDefault(u => u.Email == user.Email);
                if(dbuser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password!");    
                    return View("LoginView");                
                }
                // checks if password submitted matches with the DB password
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, dbuser.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password!");
                    return View("LoginView");
                }
                // set id in session
                HttpContext.Session.SetInt32("UserID", dbuser.UserId);
                // set name in session
                HttpContext.Session.SetString("Firstname", dbuser.FirstName);
                HttpContext.Session.SetString("Lastname", dbuser.LastName);
                return RedirectToAction("Dashboard");
            }
            return View("LoginView");
        }


// POST: /Create Wedding Process/
        [HttpPost]
        [Route("/createprocess")]
        public IActionResult CreateProcess(WeddingModel wedform)

        {
            if (ModelState.IsValid)
            {
                if (wedform.Date<=DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Please choose a future date.");
                    return View("AddWedding");
                }
                // getting user from session by ID for form submission????
                wedform.UserId = (int)HttpContext.Session.GetInt32("UserID");

                // saving wedding in db
                dbContext.Add(wedform);
                dbContext.SaveChanges();

                // getting user from session by ID and putting it in the viewbag
                ViewBag.UserID = HttpContext.Session.GetInt32("UserID");
                ViewBag.users = dbContext.UserTable;

                return RedirectToAction("Dashboard");
            }
            return View("AddWedding");
        }


// GET: / Render EDIT WEDDING / (needs work)
        [HttpGet]
        [Route("/edit/{id}")]
        public IActionResult EditWedding(int id)
        {
            return View();
        }


// POST: / EDIT WEDDING process/ (needs work)
        [HttpPost]
        [Route("/editprocess/{id}")]
        public IActionResult EditProcess(int id, WeddingModel wedform)
        {
            // checks user in session
            if(HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction ("Login");
            }
            // checks validations
            if(ModelState.IsValid)
            {
                if (wedform.Date<=DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Please choose a future date.");
                    return View("AddWedding");
                }
                // getting the model
                
            }
            return View();
        }


// GET: /renders the DASHBOARD page/
        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {
            // check user in session
            if(HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login");
            }

            // set user in session in the viewbag
            int? UserID = HttpContext.Session.GetInt32("UserID");
            ViewBag.UserID = HttpContext.Session.GetInt32("UserID");

            // make a list of weddings with all of each's respective guests
            List<WeddingModel> weddingswithguests = dbContext.Wedtable.Include(w => w.guest).ToList();
            // get name from session
            ViewBag.firstname = HttpContext.Session.GetString("Firstname");
            ViewBag.lastname = HttpContext.Session.GetString("Lastname");
            return View(weddingswithguests);
        }


// GET: /Logout and clear session/
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            // clear session
            HttpContext.Session.Clear();
            return View("LoginView");
        }


// GET: /renders the SHOW WEDDING page/
        [HttpGet]
        [Route("/wedding/{id}")]
        public IActionResult ShowWedding(int id)
        {
            // checks user in session
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction ("Login");
            }
            // set the wedding object binded with user and guest model in the viewbag
            WeddingModel currWed = dbContext.Wedtable
                .Include(w => w.guest)
                .ThenInclude(g => g.User)
                .SingleOrDefault(w => w.WedId == id);
            ViewBag.show = currWed;
            return View(currWed);
        }


// GET: /SHOWS RSVP'S
        [HttpGet]
        [Route("/rsvp/{wedId}")]
        public IActionResult RSVP(int wedId)
        {
            // checks user in session
            if(HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction ("Login");
            }
            // an instance of user where it is equal to the user in session
            User currUser = dbContext.UserTable
                .SingleOrDefault(u => u.UserId == HttpContext.Session
                .GetInt32("UserID"));
            // 
            WeddingModel currWed = dbContext.Wedtable
                .Include(w => w.guest)
                .ThenInclude(g => g.User)
                .SingleOrDefault(w => w.WedId == wedId);
            Guest thisguest = dbContext.Guest.Where(w => w.WedId == wedId && w.UserId == currUser.UserId).FirstOrDefault();
            if (thisguest != null)
            {
                currWed.guest.Remove(thisguest);
            }
            else
            {
                Guest newGuest = new Guest
                {
                    UserId = currUser.UserId,
                    WedId = currWed.WedId,
                };
                currWed.guest.Add(newGuest);
            }
            dbContext.SaveChanges();
            return RedirectToAction ("Dashboard");
        }


// GET / Delete Wedding
        [HttpGet]
        [Route("/delete/{id}")]
        public IActionResult DeleteWedding(int id)
        {
            // checks user in session
            if(HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login");
            }
            WeddingModel deletewed = dbContext.Wedtable
                .SingleOrDefault(d => d.WedId == id);
            dbContext.Wedtable.Remove(deletewed);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
