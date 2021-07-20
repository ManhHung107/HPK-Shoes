using Model.Dao;
using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class JordanController : Controller
    {
        // GET: Jordan
        OnlineShopDbContext data = new OnlineShopDbContext();
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 12;

            return View(data.Products.Where(x => x.CategoryID == 3).ToList().OrderBy(n => n.ID).ToPagedList(pageNumber, pageSize));
        }
    }
}