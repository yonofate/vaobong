using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TemplateSite.Mvc
{
    public static class ExtensionMethods
    {
        public static string InnerText(this HtmlNode node)
        {
            if (node == null)
                return string.Empty;

            return node.InnerText;
        }

        public static string AttributeValue(this HtmlNode node, string name, string def=null)
        {
            if (node == null)
                return string.Empty;

            return node.GetAttributeValue(name, def);
        }
    }
}