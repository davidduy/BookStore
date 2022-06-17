using ASM_BookStore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASM_BookStore.Controllers
{
    public class ShopController : Controller
    {
        ASMEntities aSMEntities = new ASMEntities();
        List<Order> lOrder;
        List<OrderInfo> lInfo;
        List<Book> lBooks;
        // GET: Shop
        public ActionResult Cart()
        {
            try
            {
                lInfo = new List<OrderInfo>();
                lOrder = new List<Order>();
                lBooks = new List<Book>();

                if (Session["User"] != null)
                {
                    int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                    lOrder = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).ToList();

                    if (lOrder.Count != 0)
                    {
                        int idOrder = lOrder.Find(x => x.order_customer == id).order_ID;

                        lInfo = aSMEntities.OrderInfoes.Where(x => x.orderInfo_order == idOrder).ToList();
                        foreach (OrderInfo i in lInfo)
                        {
                            Book books = aSMEntities.Books.Where(x => x.book_ID == i.orderInfo_book).FirstOrDefault();
                            lBooks.Add(books);
                        }

                        ViewBag.LO = lOrder;
                        ViewBag.LOI = lInfo;
                        ViewBag.Books = lBooks;
                        return View();
                    }
                    else return RedirectToAction("Category", "Shop");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return RedirectToAction("Login", "Page");
        }
        List<Order> list = new List<Order>();
        void Orderquality()
        {
            using (ASMEntities db = new ASMEntities())
            {
                int tmp;
                int id = Convert.ToInt32(Session["CustomerID"]);
                var cus = db.Customers.Where(x => x.customer_account == id).FirstOrDefault();
                list = db.Orders.Where(x => x.order_customer.Equals(id)).ToList();
                tmp = Convert.ToInt32(list[0].order_ID);
                int count = db.OrderInfoes.Where(x => x.orderInfo_order == tmp).Count();
                System.Diagnostics.Debug.WriteLine(count);

                Session.Add("OrderQuality", count);
            }
        }

        
        List<Book> AllBook;
        public ActionResult AddCart(int idBook)
        {
            lInfo = new List<OrderInfo>();
            lOrder = new List<Order>();
            lBooks = new List<Book>();
            AllBook = new List<Book>();
            AllBook = aSMEntities.Books.ToList();
            try
            {
                if (Session["User"] != null)
                {
                    int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                    var hasId = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).FirstOrDefault();
                    if (hasId != null)
                    {
                        Book b = AllBook.Find(x => x.book_ID == idBook);
                        OrderInfo obj = new OrderInfo();
                        obj.orderInfo_book = b.book_ID;
                        obj.orderInfo_order = hasId.order_ID;
                        obj.orderInfo_quantity = 1;
                        aSMEntities.OrderInfoes.Add(obj);
                        aSMEntities.SaveChanges();
                    }
                    else
                    {
                        Order o = new Order();
                        o.order_staff = 2;
                        o.order_status = 1;
                        o.order_total = 0;
                        o.order_date = DateTime.Now.Date;
                        o.order_customer = id;
                        aSMEntities.Orders.Add(o);
                        aSMEntities.SaveChanges();

                        var newId = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).FirstOrDefault();
                        Book b = AllBook.Find(x => x.book_ID == idBook);
                        OrderInfo obj = new OrderInfo();
                        obj.orderInfo_book = b.book_ID;
                        obj.orderInfo_order = newId.order_ID;
                        obj.orderInfo_quantity = 1;
                        aSMEntities.OrderInfoes.Add(obj);
                        aSMEntities.SaveChanges();
                        Orderquality();
                    }
                    update(1, idBook);
                }
                else return RedirectToAction("Login", "Page");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            Orderquality();
            return RedirectToAction("Cart", "Shop");
        }

        public void update(int quantity, int IdBook)
        {
            lInfo = new List<OrderInfo>();
            lOrder = new List<Order>();
            lBooks = new List<Book>();
            AllBook = new List<Book>();
            AllBook = aSMEntities.Books.ToList();

            try
            {
                int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                lOrder = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).ToList();
                if (lOrder.Count != 0)
                {
                    Order idOrder = lOrder.Find(x => x.order_customer == id);
                    float price = AllBook.Find(x => x.book_ID == IdBook).book_price;

                    lInfo = aSMEntities.OrderInfoes.Where(x => x.orderInfo_order == idOrder.order_ID).ToList();
                    OrderInfo orderInf;
                    for (int i = 1; i <= quantity; i++)
                    {
                        orderInf = new OrderInfo();
                        orderInf = aSMEntities.OrderInfoes.Where(x => x.orderInfo_book == IdBook).FirstOrDefault();
                        System.Diagnostics.Debug.WriteLine(orderInf.orderInfo_book);
                        if (orderInf != null)
                        {
                            idOrder.order_total += (quantity * price);
                            orderInf.orderInfo_quantity = quantity;
                            aSMEntities.OrderInfoes.AddOrUpdate(orderInf);
                            aSMEntities.Orders.AddOrUpdate(idOrder);
                            aSMEntities.SaveChanges();

                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult UpdateCart(int quantity, int IdBook)
        {
            update(quantity, IdBook);
            return RedirectToAction("Cart", "Shop");
        }

        public ActionResult DeleteOrder(int quantity, int IdBook)
        {
            lInfo = new List<OrderInfo>();
            lOrder = new List<Order>();
            lBooks = new List<Book>();
            AllBook = new List<Book>();
            AllBook = aSMEntities.Books.ToList();
            try
            {
                int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                lOrder = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).ToList();

                int idOrder = lOrder.Find(x => x.order_customer == id).order_ID;

                lInfo = aSMEntities.OrderInfoes.Where(x => x.orderInfo_order == idOrder).ToList();
                foreach (var inf in lInfo)
                {
                    OrderInfo orderInf = aSMEntities.OrderInfoes.Where(x => x.orderInfo_book == IdBook).FirstOrDefault();
                    if (orderInf != null)
                    {
                        float price = AllBook.Find(x => x.book_ID == IdBook).book_price;
                        float vo = price * orderInf.orderInfo_quantity;

                        Order ord = lOrder.Find(x => x.order_customer == id);
                        ord.order_total -= vo;
                        aSMEntities.OrderInfoes.Remove(orderInf);
                        aSMEntities.Orders.AddOrUpdate(ord);
                        aSMEntities.SaveChanges();
                        Orderquality();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Cart", "Shop");
        }

        public ActionResult Category()
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

        public ActionResult Buy(int IdOrder)
        {
            try
            {
                int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                lOrder = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 1).ToList();
                if (lOrder != null)
                {
                    OrderInfo quality = aSMEntities.OrderInfoes.Where(x => x.orderInfo_order.Equals(IdOrder)).FirstOrDefault();
                    Order ord = lOrder.Find(x => x.order_ID == IdOrder && x.order_status == 1);
                    if (ord != null)
                    {

                        ord.order_status = 0;
                        aSMEntities.Orders.AddOrUpdate(ord);
                        aSMEntities.SaveChanges();
                    }
                    else RedirectToAction("Category", "Shop");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Confirmation", "Shop");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        List<Order> AllOrder;
        public ActionResult Confirmation()
        {
            try
            {
                lInfo = new List<OrderInfo>();
                lOrder = new List<Order>();
                lBooks = new List<Book>();

                if (Session["User"] != null)
                {
                    int id = (Session["Customer"] as ASM_BookStore.Models.Customer).customer_ID;
                    lOrder = aSMEntities.Orders.Where(x => x.order_customer == id && x.order_status == 0).ToList();
                    if (lOrder.Count != 0)
                    {
                        int idOrder = lOrder.Find(x => x.order_customer == id).order_ID;

                        lInfo = aSMEntities.OrderInfoes.Where(x => x.orderInfo_order == idOrder).ToList();
                        foreach (OrderInfo i in lInfo)
                        {
                            Book books = aSMEntities.Books.Where(x => x.book_ID == i.orderInfo_book).FirstOrDefault();
                            lBooks.Add(books);
                        }

                        ViewBag.LO = lOrder;
                        ViewBag.LOI = lInfo;
                        ViewBag.Books = lBooks;
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return RedirectToAction("Login", "Page");
        }
        Book model = new Book();
        public ActionResult Single_product(int idBook)
        {
            using (ASMEntities db = new ASMEntities())
            {
                model = db.Books.Where(x => x.book_ID.Equals(idBook)).FirstOrDefault();
                return View(model);
            }
        }
    }
}