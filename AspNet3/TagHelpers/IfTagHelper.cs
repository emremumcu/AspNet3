using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPSNufusOlayEs.TagHelpers
{
    /// <summary>
    /// This tag helper renders its content only if the itis value is true.
    /// ex: <if itis="true"><p>True</p></if>
    /// ex: <if itis="1 == 1"><p>True</p></if>
    /// https://andrewlock.net/creating-an-if-tag-helper-to-conditionally-render-content/
    /// </summary>
    [HtmlTargetElement("if", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        [HtmlAttributeName("is")]
        public bool Include { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Always strip the outer tag name as we never want <if> to render
            output.TagName = null; 

            if (Include) return;
            else output.SuppressOutput();
        }
    }
}
