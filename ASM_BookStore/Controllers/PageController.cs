using ASM_BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ASM_BookStore.Controllers
{
    public class PageController : Controller
    {
        private Customer modelCus = new Customer();
        private Account modelAcc = new Account();
        private Order modelOrd = new Order();
        // GET: Page
        public ActionResult Login()
        {
            /* Check if user logged in */
            if (Session["User"] == null)
            {
                //ViewBag.error = TempData["Error"];
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        int Orderquality(int idOrder)
        {
            using (ASMEntities db = new ASMEntities())
            {
                return db.OrderInfoes.Where(x => x.orderInfo_order == idOrder).Count();
            }
        }
        List<Order> list = new List<Order>();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                using (ASMEntities db = new ASMEntities())
                {
                    var account = db.Accounts.Where(x => x.account_username.Equals(username) && x.account_password.Equals(password)).SingleOrDefault();

                    /* Add user if account exists */
                    if (account != null)
                    {
                        int tmp;
                        var cus = db.Customers.Where(x => x.customer_account == account.account_ID).FirstOrDefault();
                        list = db.Orders.Where(x => x.order_customer.Equals(account.account_ID)).ToList(); 
                        tmp = Convert.ToInt32(list[0].order_ID);
                        System.Diagnostics.Debug.WriteLine(Orderquality(tmp));

                        Session.Add("OrderQuality", Orderquality(tmp));
                        Session.Add("CustomerID", account.account_ID);
                        Session.Add("Customer", cus);
                        Session.Add("User", account);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            /* Request input again if account doesn't exist */
            //TempData["Error"] = "Username or password is incorrect!";
            return RedirectToAction("Login", "Page");
        }

        public ActionResult Logout()
        {
            Session.RemoveAll(); // remove all sessions
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChangeInformation()
        {
            using (ASMEntities db = new ASMEntities())
            {
                Customer modelcus = db.Customers.Find(Session["CustomerID"]);
                return View(modelcus);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInformation(string name, string address, string phone, DateTime birthday, string email, int gender)
        {
            using (ASMEntities db = new ASMEntities())
            {
                int id = Convert.ToInt32(Session["CustomerID"]);
                var update = (from u in db.Customers where u.customer_ID == id select u).Single();
                update.customer_email = email;
                update.customer_gender = Convert.ToByte(gender);
                update.customer_name = name;
                update.customer_address = address;
                update.customer_phone = phone;
                update.customer_birthday = birthday;
                db.SaveChanges();
                Customer modelcus = db.Customers.Find(Session["CustomerID"]);
                return View(modelcus);
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        int FindID(string tmp)
        {
            using (ASMEntities db = new ASMEntities())
            {
                return Convert.ToInt32(db.Accounts.Where(x => x.account_username.Equals(tmp)).Select(x => x.account_ID).Max());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string name, string email, DateTime birthday, string password, string confirmPassword, string gender, string phone, string address, string Username)
        {
            using (ASMEntities db = new ASMEntities())
            {
                try
                {
                    if (Session["User"] == null)
                    {
                        if (ModelState.IsValid)
                        {
                            if (db.Customers.FirstOrDefault(s => s.customer_email.Equals(email)) == null)
                            {
                                modelAcc.account_username = Username;
                                modelAcc.account_password = password;
                                modelAcc.account_status = 1;
                                modelAcc.account_role = 2;
                                db.Accounts.Add(modelAcc);
                                db.SaveChanges();
                                modelCus.customer_name = name;
                                modelCus.customer_gender = Convert.ToByte(gender);
                                modelCus.customer_birthday = birthday;
                                modelCus.customer_address = address;
                                modelCus.customer_email = email;
                                modelCus.customer_phone = phone;
                                modelCus.customer_status = 1;
                                modelCus.customer_account = FindID(Username);
                                db.Customers.Add(modelCus);
                                db.SaveChanges();
                                Session.Add("CustomerID", modelCus.customer_account);
                                RedirectToAction("Login", "Page");
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("=======" + ex + "===========");
                }
            }
            return View();
        }
    }
}
