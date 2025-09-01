using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConverterDemo.Infrastructure.ValueConverters
{
    public class Base64StringConverter : ValueConverter<string, string>
    {
        public Base64StringConverter()
            : base(
                v => Encode(v),   // مدل -> دیتابیس
                s => Decode(s))   // دیتابیس -> مدل
        { }

        private static string Encode(string input)
        {
            if (input == null) return null;
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        private static string Decode(string input)
        {
            if (input == null) return null;
            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}