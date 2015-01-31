using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CourseAider.Presentation
{
    public static class BootstrapExtentions
    {
        public static Tag BeginColumn(this HtmlHelper htmlHelper,int size,int? offset=null)
        {
            if (size < 1 || size > 12) throw new ArgumentException("size must be a number from 1 do 12");
            string attr = "col-md-" + size.ToString();
            if (offset.HasValue)
            {
                if (offset < 1 || offset > 12) throw new ArgumentException("offset must be a number from 1 do 12");
                attr += " col-md-offset-" + offset.ToString();
            }
            return BeginTagWithClass(htmlHelper, "div", attr, null);
        }
        public static Tag BeginRow(this HtmlHelper htmlHelper)
        {
            return BeginTagWithClass(htmlHelper, "div", "row", null);
        }
        public static Tag BeginFluidContainer(this HtmlHelper htmlHelper, object htmlAttributes = null, string customTag = null)
        {
            customTag = string.IsNullOrEmpty(customTag) ? "section" : customTag;
            return BeginTagWithClass(htmlHelper, customTag, "container-fluid", htmlAttributes);
        }
        
        public static Tag BeginContainer(this HtmlHelper htmlHelper, object htmlAttributes = null, string customTag = null)
        {
            return BeginTagWithClass(htmlHelper, customTag, "container" , htmlAttributes);
        }

        public static Tag BeginTagWithClass(this HtmlHelper htmlHelper,string tagName,string className, object htmlAttributes = null)
        {
            tagName = string.IsNullOrEmpty(tagName) ? "div" : tagName;
            var tag = new Tag(htmlHelper.ViewContext, tagName);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                var classAttribute = attributes.FirstOrDefault(attr => attr.Key == "class");
                if (!string.IsNullOrWhiteSpace((string)classAttribute.Value))
                {
                    string classText = classAttribute.Value + " " + className;
                    attributes.Remove("class");
                    attributes.Add("class", classText);
                }
                else
                {
                    attributes.Add("class", className);
                }
                tag.OpenTag(attributes);
            }
            else
            {
                var dict = new Dictionary<string, object>();
                dict.Add("class", className);
                tag.OpenTag(dict);
            }
            return tag;
        }
    }
}
