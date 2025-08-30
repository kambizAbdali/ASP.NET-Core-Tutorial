using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueConverterDemo.Core.Models;

namespace Sequences.Infrastructure.ValueConverters
{
    // Converts the ShippingPreference enum to and from a string representation in the database.
    public class ShippingPreferenceConverter : ValueConverter<ShippingPreference, string>
    {
        // Sets up the conversion logic when the converter is initialized.
        public ShippingPreferenceConverter() : base(
            // Converts the ShippingPreference enum to its string representation.
            preference => preference.ToString(),

            // Converts the string representation back to the ShippingPreference enum.
            stringValue => (ShippingPreference)Enum.Parse(typeof(ShippingPreference), stringValue))
        { }
    }
}
