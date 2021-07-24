using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Dao;
using Model.EF;
using PagedList;
using PagedList.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        OnlineShopDbContext data = new OnlineShopDbContext();
        // GET: Product
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 12;
            return View(data.Products.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Search(string searchString, int page = 1, int pageSize = 12)
        {
            var dao = new ProductDao();
            var model = dao.ListAllPagingg(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }
        public ActionResult Detail(long id)
        {
            var product = new ProductDao().ViewDetail(id);
            return View(product);
        }
    }
}