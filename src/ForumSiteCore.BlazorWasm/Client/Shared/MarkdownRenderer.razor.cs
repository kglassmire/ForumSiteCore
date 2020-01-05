using Ganss.XSS;
using Markdig;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Shared
{
    public partial class MarkdownRenderer : ComponentBase
    {
        private string _markdown;

        [Inject] public IHtmlSanitizer HtmlSanitizer { get; set; }

        [Parameter]
        public string Markdown
        {
            get => _markdown;
            set
            {
                _markdown = value;
                HtmlOutput = ConvertStringToMarkupString(_markdown);
            }
        }

        public MarkupString HtmlOutput { get; private set; }

        private MarkupString ConvertStringToMarkupString(string value)
        {
            if (!string.IsNullOrWhiteSpace(_markdown))
            {
                var html = Markdig.Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());                
                var sanitizedHtml = HtmlSanitizer.Sanitize(html);
                return new MarkupString(sanitizedHtml);
            }

            return new MarkupString();
        }
    }
}
