using Ecom.Models.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Ecom.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        [HttpGet]
        public ActionResult Index()
        {
            EcomEntities1 db = new EcomEntities1();
            var data = db.Products.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Index(Product pp)
        {
            var s = Request["Search"];
            EcomEntities1 db = new EcomEntities1();
            var data = (from p in db.Products where p.Name == s select p).ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product n)
        {
            if (ModelState.IsValid)
            {
                EcomEntities1 db = new EcomEntities1();
                db.Products.Add(n);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            EcomEntities1 db = new EcomEntities1();
            var data = (from n in db.Products where n.Id == id select n).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Update(Product nn)
        {
            if (ModelState.IsValid)
            {
                EcomEntities1 db = new EcomEntities1();
                var data = (from n in db.Products where n.Id == nn.Id select n).FirstOrDefault();
                db.Entry(data).CurrentValues.SetValues(nn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            EcomEntities1 db = new EcomEntities1();
            var data = (from n in db.Products where n.Id == id select n).FirstOrDefault();
            db.Products.Remove(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult AddToCart(int id)
        {
            EcomEntities1 db = new EcomEntities1();
            var data = (from n in db.Products where n.Id == id select n).FirstOrDefault();
            if(Session["cart"] == null)
            {
                List<Product> products = new List<Product>();
                products.Add(data);
                string cart = new JavaScriptSerializer().Serialize(products);
                Session["cart"] = cart;
            }
            else
            {
                var cartJson = Session["cart"].ToString();
                var cartList = new JavaScriptSerializer().Deserialize<List<Product>>(cartJson);
                cartList.Add(data);
                string cart = new JavaScriptSerializer().Serialize(cartList);
                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Viewcart()
        {
            if(Session["cart"] == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var cartJson = Session["cart"].ToString();
                var cartList = new JavaScriptSerializer().Deserialize<List<Product>>(cartJson);

                return View(cartList);
            }
            
        }
        public ActionResult Order()
        {
            var cartJson = Session["cart"].ToString();
            var cartList = new JavaScriptSerializer().Deserialize<List<Product>>(cartJson);
            foreach (Product p in cartList)
            {
                InsertCustomer(p.Name, p.Price, p.Id);
            }
            Session["cart"] = null;
            return RedirectToAction("Index");
        }
        private void InsertCustomer(string pname, string pprice, int pid)
        {
            string connString = "Data Source=DESKTOP-5HPTFOF;Initial Catalog=Ecom;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            string query = "insert into [Order] (Pname, Pprice, Pid) values (@pname, @pprice, @pid)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@pname", pname);
            cmd.Parameters.AddWithValue("@pprice", pprice);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.ExecuteNonQuery();        
        }
        public ActionResult ClearCart()
        {
            Session["cart"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult ViewOrders()
        {
            EcomEntities2 db = new EcomEntities2();
            var data = db.Orders.ToList();
            return View(data);
        }
    }
}