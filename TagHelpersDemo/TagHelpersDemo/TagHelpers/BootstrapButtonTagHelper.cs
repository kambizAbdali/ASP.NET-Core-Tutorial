using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpersDemo.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "bs-button-color")]
    public class BootstrapButtonTagHelper : TagHelper
    {
        [HtmlAttributeName("bs-button-color")]
        public string ButtonColor { get; set; } = "primary";

        [HtmlAttributeName("bs-button-icon")]
        public string? Icon { get; set; }

        [HtmlAttributeName("bs-button-size")]
        public string? Size { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var cssClass = $"btn btn-{ButtonColor}";
            if (!string.IsNullOrEmpty(Size)) cssClass += $" btn-{Size}";
            output.Attributes.SetAttribute("class", cssClass);

            if (!string.IsNullOrEmpty(Icon))
            {
                var iconHtml = $"<i class='{Icon}'></i> ";
                output.Content.AppendHtml(iconHtml);
            }
        }
    }
}