﻿using Model.EF;
using Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ProductDao
    {
        OnlineShopDbContext db = null;
        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }

        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        //public List<string> ListName(string keyword)
        //{
        //    return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        //}
        public IEnumerable<Product> ListAllPagingg(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Code.Contains(searchString));
            }

            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.ViewCount).Take(top).ToList();
        }
        
        public List<Product> ListRelatedProducts(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }
        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }
        public bool Delete(int id)
        {
            try
            {
                var pro = db.Products.Find(id);
                db.Products.Remove(pro);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}