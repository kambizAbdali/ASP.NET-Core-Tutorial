using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsRemoved { get; set; } // برای حذف منطقی

        //[NotMapped] // اگر این ویژگی در دیتابیس نباشه
        public decimal Tax { get; set; } // مالیات (محاسبه شده)
    }
}