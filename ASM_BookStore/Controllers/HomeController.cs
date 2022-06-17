using ASM_BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASM_BookStore.Controllers
{
    public class HomeController : Controller
    {
        ASMEntities aSMEntities = new ASMEntities();
        List<Order> lOrder;
        List<OrderInfo> lInfo;
        List<Book> AllBook;
        public ActionResult Index()
        {
            AllBook = new List<Book>();
            AllBook = aSMEntities.Books.ToList();
            try
            {
                ViewBag.AllBook = AllBook;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}