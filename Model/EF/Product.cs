namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Product")]
    public partial class Product
    {
        public long ID { get; set; }

        [StringLength(250)]
        [Display(Name ="Tên sản phẩm")]
        public string Name { get; set; }

        [StringLength(10)]
        [Display(Name = "Mã sản phẩm")]
        public string Code { get; set; }

        [StringLength(250)]
        [Display(Name = "Tiêu đề")]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [StringLength(250)]
        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }

        [Column(TypeName = "xml")]
        public string MoreImages { get; set; }
        [Display(Name = "Giá")]
        public decimal? Price { get; set; }
        [Display(Name = "Giá khuyến mãi")]
        public decimal? PromotionPrice { get; set; }

        public bool? IncludedVAT { get; set; }
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Display(Name = "Thương hiệu")]
        public long? CategoryID { get; set; }

        [Column(TypeName = "ntext")]
        public string Detail { get; set; }

        public int? Warranty { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [StringLength(250)]
        public string MetaKeywords { get; set; }

        [StringLength(250)]
        public string MetaDescriptions { get; set; }

        public bool? Status { get; set; }

        public DateTime? TopHot { get; set; }

        public int? ViewCount { get; set; }

        public double TotalMoney()
        {
            var list = new List<Product>();
            var total = list.Sum(s => s.Price * s.Quantity);
            return (double)total;
        }
    }
}
