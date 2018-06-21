using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Helpers.TagHelpers
{
    /// <summary>
    /// Generates pager for paginated lists
    /// </summary>
    [HtmlTargetElement("pager")]
    public class PagedListTagHelper : TagHelper
    {
        private IUrlHelper helper;
        private readonly IUrlHelperFactory helperFactory;
        private readonly IStringLocalizer<PagedListTagHelper> translate;

        public PagedListTagHelper(IUrlHelperFactory helperFactory, IStringLocalizer<PagedListTagHelper> localizer)
        {
            this.translate = localizer;
            this.helperFactory = helperFactory;
            this.RouteValues = new Dictionary<string, string>(0);
            this.HtmlAttributesFor = new Dictionary<string, object>(0);
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "attributes-for-")]
        public IDictionary<string, object> HtmlAttributesFor { get; set; }

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        [HtmlAttributeName("buttons-count")]
        public int Buttons { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "asp-route-")]
        public IDictionary<string, string> RouteValues { get; }

        public IUrlHelper Url
        {
            get
            {
                if (this.helper == null)
                {
                    this.helper = this.helperFactory.GetUrlHelper(this.ViewContext);
                }

                return this.helper;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (this.TotalPages > 0)
            {
                const string PageRouteValue = "p";
                const string EntriesRouteValue = "r";

                object linkHtmlAttributes = null;
                object itemHtmlAttributes = null;
                object listHtmlAttributes = null;

                if (string.IsNullOrEmpty(this.Action))
                {
                    this.Action = this.ViewContext.RouteData.Values["Action"].ToString();
                }

                if (string.IsNullOrEmpty(this.Controller))
                {
                    this.Controller = this.ViewContext.RouteData.Values["Controller"].ToString();
                }

                if (!int.TryParse(this.RouteValues[PageRouteValue], out int currentPage))
                {
                    currentPage = 1;
                }

                if (!this.RouteValues.ContainsKey(EntriesRouteValue))
                {
                    this.RouteValues[EntriesRouteValue] = "5";
                }

                if (this.HtmlAttributesFor.ContainsKey("a"))
                {
                    linkHtmlAttributes = this.HtmlAttributesFor["a"];
                }

                if (this.HtmlAttributesFor.ContainsKey("li"))
                {
                    itemHtmlAttributes = this.HtmlAttributesFor["li"];
                }

                if (this.HtmlAttributesFor.ContainsKey("ul"))
                {
                    listHtmlAttributes = this.HtmlAttributesFor["ul"];
                }

                var buttons = this.Buttons > 0 ? this.Buttons : 5;
                var lastButton = Math.Min((buttons / 2 + currentPage), this.TotalPages);
                var pages = new HashSet<int>(Enumerable.Range((lastButton <= buttons ? 1 : lastButton - buttons + 1), Math.Min(buttons, this.TotalPages)));

                var firstButtonText = pages.Add(1) ? this.translate["First page"] : "1";
                var lastButtonText = pages.Add(this.TotalPages) ? this.translate["Last page"] : this.TotalPages.ToString();

                var list = new TagBuilder("ul") { TagRenderMode = TagRenderMode.Normal };

                list.AddCssClass("pagination");

                if (listHtmlAttributes != null)
                {
                    list.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(listHtmlAttributes));
                }

                output.TagName = "div";

                foreach (int page in pages.OrderBy(i => i))
                {
                    var link = new TagBuilder("a") { TagRenderMode = TagRenderMode.Normal };
                    var item = new TagBuilder("li") { TagRenderMode = TagRenderMode.Normal };

                    if (page == currentPage)
                    {
                        item.AddCssClass("active");
                        link.AddCssClass("btn disabled");
                        link.MergeAttribute("title", this.translate["Current page"]);
                    }
                    else
                    {
                        this.RouteValues[PageRouteValue] = page.ToString();
                        link.MergeAttribute("href", this.Url.Action(this.Action, this.Controller, this.RouteValues));
                        link.MergeAttribute("title", this.translate["Go to page {0}", page]);
                    }

                    if (itemHtmlAttributes != null)
                    {
                        item.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(itemHtmlAttributes));
                    }

                    if (linkHtmlAttributes != null)
                    {
                        link.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(linkHtmlAttributes));
                    }

                    link.InnerHtml.AppendHtml(page == 1 ? firstButtonText : page == this.TotalPages ? lastButtonText : page.ToString());

                    item.InnerHtml.AppendHtml(link);
                    list.InnerHtml.AppendHtml(item);
                }

                output.Content.SetHtmlContent(list);
                output.Attributes.Clear();
                output.Attributes.SetAttribute("class", "pager");
            }
        }
    }
}
