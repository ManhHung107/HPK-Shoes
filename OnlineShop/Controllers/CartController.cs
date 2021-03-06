using Model.Dao;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Model.EF;
using Common;
using System.Configuration;
using System.IO;
using OnlineShop.Common;
using Newtonsoft.Json.Linq;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private const string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult AddItem(long productId, int quantity, string strUrl)
        {
            var product = new ProductDao().ViewDetail(productId);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.ID == productId))
                {

                    foreach (var item in list)
                    {
                        if (item.Product.ID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                    return Redirect(strUrl);
                }
                
                Session[CartSession] = list;
            }
            else
            {
                
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                
                
                Session[CartSession] = list;
                return Redirect(strUrl);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(string shipName,string mobile,string address,string email)
        {
            var order = new Order();
           
            order.CreateDate = DateTime.Now;
            order.ShipAddress = address;
            order.ShipMobile = mobile;
            order.ShipName = shipName;
            order.ShipEmail = email;

            try
            {
                var id = new OrderDao().Insert(order);
                var cart = (List<CartItem>)Session[CartSession];
                var detailDao = new Model.Dao.OrderDetailDao();
                decimal total = 0;
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = id;
                    orderDetail.Price = item.Product.Price;
                    orderDetail.Quantity = item.Quantity;
                    detailDao.Insert(orderDetail);

                    total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                }
                string content = System.IO.File.ReadAllText(Server.MapPath("~/assets/client/template/neworder.html"));
                content = content.Replace("{{CustomerName}}", shipName);
                content = content.Replace("{{Phone}}", mobile);
                content = content.Replace("{{Email}}", email);
                content = content.Replace("{{Address}}", address);
                content = content.Replace("{{Total}}", total.ToString("N0"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new MailHelper().SendMail(email, "Đơn hàng mới từ HPK SHOES", content);
                new MailHelper().SendMail(toEmail, "Đơn hàng mới từ HPK SHOES", content);
            }
            catch (Exception ex)
            {
                //ghi log
                return Redirect("/loi-thanh-toan");
            }
            Session[CartSession] = null;
            return Redirect("/hoan-thanh");
        }
        //public ActionResult MomoPayment ()
        //{
        //    var cart = (List<CartItem>)Session[CartSession];
        //    string endpoint = ConfigurationManager.AppSettings["endpoint"].ToString();
        //    string partnercode = ConfigurationManager.AppSettings["partnercode"].ToString();
        //    string accesskey = ConfigurationManager.AppSettings["accesskey"].ToString();
        //    string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
        //    string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //    string returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();
        //    string notifyurl = ConfigurationManager.AppSettings["notifyurl"].ToString();

        //    string amount = cart.Sum(n => n.Product.Price.GetValueOrDefault(0) * n.Quantity).ToString();
        //    string orderid = Guid.NewGuid().ToString();
        //    string requestId = Guid.NewGuid().ToString();
        //    string extraData = "";

        //    string rawHash = "partnercode=" + partnercode + "&accesskey=" + accesskey + "&requestId=" + requestId + "&amount=" + amount + "&orderid=" + orderid +
        //        "&orderInfo" + orderInfo + "&returnUrl" + returnUrl + "&notifyurl" + notifyurl + "&extraData" + extraData;
        //    string signature = MoMoSecurity.SignSHA256(rawHash, serectkey);
        //    JObject message = new Object
        //    {
        //        {"partnercode", partnercode },
        //        {"accesskey" ,accesskey},
        //        {"requestId",requestId },
        //        {"amount",amount },
        //        {"orderid",orderid },
        //        {"orderInfo",orderInfo },
        //        {"returnUrl",returnUrl },
        //        {"notifyurl",notifyurl },
        //        {"requestType","captureMoMoWallet" },
        //        {"signature", signature }
        //    };
        //    string reponseFromMoMo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
        //    JObject jmessage = JObject.Parse(reponseFromMoMo);
        //    return Redirect(jmessage.GetValue().ToString());

        //}
        //public ActionResult ReturnUrl()
        //{

        //}
        public ActionResult Success()
        {
            return View();
        }
    }
}