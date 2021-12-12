using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.TagHelpers
{
    /// <summary>
    /// Applies to all html elements having a Condition attribute.
    /// If the specified condition is not true, then the element is not rendered at all.
    /// ex: <p Condition="false" >This is a paragraph but not rendered since condition is false</p>
    /// </summary>
    [HtmlTargetElement(Attributes = nameof(Condition))]
    public class ConditionTagHelper : TagHelper
    {
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Condition)
            {
                output.SuppressOutput();
            }
        }
    }
}
