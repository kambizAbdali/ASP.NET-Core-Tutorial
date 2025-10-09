using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpersDemo.TagHelpers
{
    [HtmlTargetElement("last-updated")]
    public class LastUpdatedTagHelper : TagHelper
    {
        public DateTime LastUpdate { get; set; }
        public bool ShowRelativeTime { get; set; } = true;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "last-updated");

            var content = await output.GetChildContentAsync();
            var childContent = content.GetContent();

            var timeText = ShowRelativeTime ? GetRelativeTime(LastUpdate) : LastUpdate.ToString("g");
            output.Content.SetHtmlContent($"{childContent} <small class='text-muted'>(Last updated: {timeText})</small>");
        }

        private string GetRelativeTime(DateTime date)
        {
            var span = DateTime.Now - date;
            if (span.TotalDays > 365) return $"{(int)(span.TotalDays / 365)} years ago";
            if (span.TotalDays > 30) return $"{(int)(span.TotalDays / 30)} months ago";
            if (span.TotalDays > 7) return $"{(int)(span.TotalDays / 7)} weeks ago";
            if (span.TotalDays > 1) return $"{(int)span.TotalDays} days ago";
            if (span.TotalHours > 1) return $"{(int)span.TotalHours} hours ago";
            if (span.TotalMinutes > 1) return $"{(int)span.TotalMinutes} minutes ago";
            return "just now";
        }
    }
}