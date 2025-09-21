using ConcurrencyExample.Core.Entities;
using ConcurrencyExample.Core.Interfaces;
using ConcurrencyExample.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // مدیریت خطا
                var entry = ex.Entries.Single();
                var databaseValues = await entry.GetDatabaseValuesAsync();

                if (databaseValues == null)
                {
                    throw new DbUpdateException("Product has been deleted by another user.");
                }

                var originalValues = entry.OriginalValues.Clone();
                var currentValues = entry.CurrentValues.Clone();

                // مقادیر از پایگاه داده
                var databaseName = databaseValues.GetValue<string>("Name");

                // مقادیر کاربر
                var originalName = originalValues.GetValue<string>("Name");
                var currentName = currentValues.GetValue<string>("Name");


                // **استراتژی های حل تعارض:**

                // 1. Last-In-Wins (اولویت به آخرین تغییر):
                // در این حالت، تغییرات شما بر روی داده های جدید پایگاه داده اعمال می شود.
                // این ساده ترین راه حل است، اما ممکن است باعث از دست رفتن داده ها شود.
                //entry.OriginalValues.SetValues(databaseValues); // مقادیر Original را به مقادیر DB ست میکنیم
                //await _context.SaveChangesAsync();


                // 2. Client Wins (اولویت با کلاینت):
                //  مقادیر تغییر یافته کلاینت رو  دوباره به دیتابیس میفرستیم و امیدواریم مشکلی پیش نیاد.
                // البته باز هم احتمال  DbUpdateConcurrencyException  هست که باید دوباره هندل بشه
                //product.RowVersion = (byte[])databaseValues["RowVersion"];
                //_context.Entry(product).State = EntityState.Modified;
                //await _context.SaveChangesAsync();


                //3. Merge (ادغام):
                // در این حالت، شما سعی می کنید تغییرات خود را با داده های جدید پایگاه داده ادغام کنید.
                // این راه حل پیچیده تر است، اما احتمال از دست رفتن داده ها را کاهش می دهد.
                // مثال: اگر نام محصول توسط کاربر دیگری تغییر کرده باشد، شما می توانید
                // نام جدید را با نام قدیمی ترکیب کنید یا یک پیام به کاربر نمایش دهید
                // تا او تصمیم بگیرد که کدام نام را انتخاب کند.

                // در این مثال، نام محصول را با نام جدید و یک نشانگر ادغام می کنیم:
                //product.Name = databaseName + " (Conflict Resolved)";
                //_context.Entry(product).State = EntityState.Modified;
                //await _context.SaveChangesAsync();


                // 4.  Throw Exception (پرتاب استثنا):
                //   به کلاینت اطلاع میدیم که داده تغییر کرده است.
                //  این کار را با پرتاب کردن دوباره ی  DbUpdateConcurrencyException   انجام میدهیم
                throw ex;


                // 5. Reload and Let Client Handle (بارگیری مجدد و واگذاری به کلاینت):
                //   ما داده ها را دوباره از دیتابیس دریافت میکنیم و به کلاینت اجازه میدهیم تصمیم بگیرد.
                //   این مورد نیاز به این دارد که داده های  databaseValues   به کلاینت برگردانده شوند.
                //   و کلاینت هم UI مناسب برای حل کانفلیکت را داشته باشد.
                //   entry.Reload();
                //   return databaseValues; // (این خط نیاز دارد که نوع خروجی تابع را تغییر دهیم)

                // در نهایت، اگر هیچ یک از راه حل های بالا مناسب نبود، می توانید یک استثنای سفارشی پرتاب کنید
                // با اطلاعات بیشتر در مورد تعارض.
                //throw new ConcurrencyException("Concurrency conflict occurred while updating product.", originalValues, databaseValues);

            }
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // مثال استفاده از BeginTransaction و Commit
        public async Task PerformTransactionalOperationAsync(Product product1, Product product2)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // عملیات اول: اضافه کردن محصول 1
                    _context.Products.Add(product1);
                    await _context.SaveChangesAsync();

                    // عملیات دوم: اضافه کردن محصول 2
                    _context.Products.Add(product2);
                    await _context.SaveChangesAsync();

                    // اگر همه چیز خوب پیش رفت، تراکنش را Commit می کنیم
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // اگر خطایی رخ داد، تراکنش را Rollback می کنیم
                    await transaction.RollbackAsync();
                    throw new Exception("Transaction failed. All changes rolled back.", ex);
                }
            }
        }

        // مثال استفاده از SaveChanges برای چندین Entity
        public async Task PerformMultipleEntityChangesAsync(Product product1, Product product2)
        {
            try
            {
                // گرفتن ترَکِ موجودیت ها
                var entry1 = _context.Entry(product1);
                var entry2 = _context.Entry(product2);

                // بررسی وضعیت موجودیت ها: اگر Detached باشند، باید آنها را Attach کنیم
                if (entry1.State == EntityState.Detached)
                {
                    _context.Products.Attach(product1);
                }
                if (entry2.State == EntityState.Detached)
                {
                    _context.Products.Attach(product2);
                }

                // تغییرات برای محصول 1
                entry1.State = EntityState.Modified;

                // تغییرات برای محصول 2
                entry2.State = EntityState.Modified;


                // ذخیره تغییرات برای هر دو Entity به صورت یکجا
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // اگر خطایی رخ داد، تمام تغییرات Rollback می شوند
                // (این رفتار پیش فرض Entity Framework است)
                throw new Exception("Failed to save changes for multiple entities. All changes rolled back.", ex);
            }
        }

    }
}