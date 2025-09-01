using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConverterDemo.Infrastructure.ValueConverters
{
    public class BoolToStrConverter : ValueConverter<bool, string>
    {
        public BoolToStrConverter(string trueValue="True", string falseValue="False")
            : base(
                v => v ? trueValue : falseValue,
                s => string.Equals(s, trueValue, StringComparison.OrdinalIgnoreCase))
        {
        }
    }
}
