using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.UI.Extensions
{
    [HtmlTargetElement("partial-view")]
    public class PartialViewTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ScriptKey = "PartialViewTagHelper_ScriptAdded";

        public PartialViewTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("url")]
        public string Url { get; set; }

        [HtmlAttributeName("id")]
        public string Id { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "partial-view");
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("url", Url);
            output.TagMode = TagMode.StartTagAndEndTag;

            var scriptAdded = _httpContextAccessor.HttpContext.Items.ContainsKey(ScriptKey);
            if (!scriptAdded)
            {
                var script = $@"
                <script>
                document.addEventListener('DOMContentLoaded', function() {{
                {{
                    var elements = document.getElementsByClassName('partial-view');
                    Array.from(elements).forEach(function(el) {{
                        var url = el.getAttribute('url');
                        var id = el.getAttribute('id');
                        fetch(url)
                            .then(response => response.text())
                            .then(html => {{
                                var container = document.getElementById(id);
                                container.innerHTML = html;
                                var scripts = container.getElementsByTagName('script');
                                for (var i = 0; i < scripts.length; i++)
                                {{
                                    var script = scripts[i];
                                    var scriptTag = document.createElement('script');
                                    if (script.src)
                                    {{
                                        scriptTag.src = script.src;
                                    }}
                                    else
                                    {{
                                        scriptTag.textContent = script.textContent;
                                    }}
                                    document.head.appendChild(scriptTag).parentNode.removeChild(scriptTag);
                                }}
                            }});
                    }});
                }}
                }});
                </script>";

                output.PostContent.AppendHtml(script);
                _httpContextAccessor.HttpContext.Items[ScriptKey] = true;
            }
        }
    }
}
