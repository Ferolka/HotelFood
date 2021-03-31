using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public delegate void RenderSyncDelegate();
    public class TemplateParams
    {
        public RenderAsyncDelegate Action { get; set; }
        public RazorPageBase Page { get; set; }

    }
    public abstract class HotelBasePage<TModel> : RazorPage<TModel>
    {

        public virtual void DefineTemplateSection(string name, RenderAsyncDelegate action)
        {
            this.ViewData[name] = new TemplateParams() { Action = action, Page = this };
        }
        public virtual HtmlString RenderTemplateSection(string name)
        {
            

            var task= RenderTemplateSectionAsync(name);
            
            return task.GetAwaiter().GetResult();
        }
        private  HtmlString RenderTemplateSectionSync(string name)
        {
            //   (this.ViewData[name] as RenderAsyncDelegate)?.Invoke();
            if (this.ViewData.TryGetValue(name, out var executor))
            {
                var typedexecutor = executor as TemplateParams;
                if (typedexecutor == null)
                    return null;
                var orig = typedexecutor.Page.ViewContext;
                try
                {
                    typedexecutor.Page.ViewContext = this.ViewContext;
                    typedexecutor.Action();
                }
                finally
                {
                    typedexecutor.Page.ViewContext = orig;
                }
                return HtmlString.Empty;
            }
            return null;

        }
        private  async Task<HtmlString> RenderTemplateSectionAsync(string name)
        {
            //   (this.ViewData[name] as RenderAsyncDelegate)?.Invoke();
            if (this.ViewData.TryGetValue(name, out object executor))
            {
                var typedexecutor = executor as TemplateParams;
                if (typedexecutor == null)
                    return null;
                var orig = typedexecutor.Page.ViewContext;
                try
                {
                    typedexecutor.Page.ViewContext = this.ViewContext;
                    await typedexecutor.Action();
                }
                finally
                {
                    typedexecutor.Page.ViewContext = orig;
                }
                return HtmlString.Empty;
                //return HtmlString.Empty;
            }
            return null;

        }
        // [RazorInject]
        public override void Write(string value)
        {
            base.Write(value);
        }
        /*
        public override void Write(object value)
        {
            TagHelperOutput tout = value as TagHelperOutput;
            if (false && tout != null)
            {
                var html = tout.Content.ToString();
                html = REGEX_TAGS.Replace(html, "> <");
                html = REGEX_ALL.Replace(html, " ");
                tout.Content.SetHtmlContent(html);
            }
               
            base.Write(value);
            if (false  && value != null)
            {
                var html = value.ToString();
                html = REGEX_TAGS.Replace(html, "> <");
                html = REGEX_ALL.Replace(html, " ");
              //  if (value is MvcHtmlString) value = new MvcHtmlString(html);
               // else 
                    value = html;
            }
           // base.Write(value);
        }
        */
        //public override void WriteLiteral(string  value)
        //{
        //     var  html = REGEX_TAGS.Replace(value, "> <");
        //      html = REGEX_ALL.Replace(html, " ");
        //     base.WriteLiteral(html);
        //   // base.WriteLiteral(value);
        //}

        public override void WriteLiteral(object value)
        {
            if (value != null)
            {
                var html = value.ToString();
                html = REGEX_TAGS.Replace(html, "> <");
                html = REGEX_ALL.Replace(html, " ");
                //if (value is MvcHtmlString) value = new MvcHtmlString(html);
                //else
                //    value = html;
            }
            base.WriteLiteral(value);
        }

        private static readonly Regex REGEX_TAGS = new Regex(@">\s+<", RegexOptions.Compiled);
        // private static readonly Regex REGEX_ALL = new Regex(@"\s+|\t\s+|\n\s+|\r\s+", RegexOptions.Compiled);

        private static readonly Regex REGEX_ALL = new Regex(@"[ ]+", RegexOptions.Compiled);
    }
}
