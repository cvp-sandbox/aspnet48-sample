using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EventRegistrationSystemCore.TagHelpers;



[HtmlTargetElement("legacy-link")]
public class LegacyLink : TagHelper
{
    public required string Href { get; set; }

    public required string Class { get; set; }

    public string? Content { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a"; // changes <legacy-link> to <a>
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.SetAttribute("href", Href);

        if (!string.IsNullOrWhiteSpace(Class))
        {
            output.Attributes.SetAttribute("class", Class);
        }

        // Use content if provided, otherwise fallback to child content
        if (!string.IsNullOrEmpty(Content))
        {
            output.Content.SetHtmlContent(Content);
        }
    }
}
