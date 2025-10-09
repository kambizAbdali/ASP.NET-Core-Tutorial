using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpersDemo.TagHelpers
{
    [HtmlTargetElement("email", TagStructure = TagStructure.WithoutEndTag)]
    public class EmailTagHelper : TagHelper
    {
        public string Address { get; set; } = string.Empty;
        public string? Domain { get; set; } = "company.com";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            var email = $"{Address}@{Domain}";
            output.Attributes.SetAttribute("href", $"mailto:{email}");
            output.Attributes.SetAttribute("class", "email-link");
            output.Content.SetContent(email);
        }
    }
}