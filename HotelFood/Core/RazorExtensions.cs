using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace HotelFood.Core
{
   
    public static class RazorExtensions
    {
      
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static DateTime ResetHMS(this DateTime dt)
        {
            DateTime res = new DateTime(dt.Year, dt.Month, dt.Day);
            return res;
        }
        public static long MSDateSince1970(this DateTime dt) {
            return (long)dt.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
        }
        public static string ShortSqlDate(this DateTime dt)
        {
            
            return $"{dt.Year}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
        }
        public static DateTime? SetDefaultIfNotSet(this DateTime? dt, DateTime dflt,bool resetHMS=true)
        {
            if (!dt.HasValue)
                dt = dflt;
            if (resetHMS)
                dt = dt.Value.ResetHMS();
            return dt;
        }
        public static string ShortJSDate(this DateTime dt)
        {

            return $"{dt.Year}-{dt.Month.ToString("D2")}-{dt.Day.ToString("D2")}";
        }
        public static string AbsoluteContent(this IUrlHelper url, string contentPath)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }
        public static DateTime OnlyDateNow()
        {
            return ResetHMS(DateTime.Now);
        }
        public static async Task<IHtmlContent> InvokeComponentAsync<TModel,TResult>(this RazorPage<IEnumerable<TModel>> source, IViewComponentHelper comphelper, string componentname,  Expression<Func<TModel, TResult>> expression, object param, object param1 = null)
        {
           // source.Component.
            return await comphelper.InvokeAsyncEx(componentname, expression,param,param1);
        }
        public static async Task<IHtmlContent> InvokeAsyncEx<TModel, TResult>(this IViewComponentHelper source,string componentname, Expression<Func<TModel, TResult>> expression, object param,object param1=null)
        {
            
            return await source.InvokeAsync(componentname, new {field= GetPropertyName(expression),displayname= GetDisplayName(expression), queryModel=param , param=param1 });
        }
        public static string GetPropertyName<TModel, TResult>(System.Linq.Expressions.Expression<Func<TModel, TResult>> property)
        {
            System.Linq.Expressions.LambdaExpression lambda = (System.Linq.Expressions.LambdaExpression)property;
            System.Linq.Expressions.MemberExpression memberExpression;

            if (lambda.Body is System.Linq.Expressions.UnaryExpression)
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)(lambda.Body);
                memberExpression = (System.Linq.Expressions.MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (System.Linq.Expressions.MemberExpression)(lambda.Body);
            }
            string parent = "";
            if(typeof(TModel)!= memberExpression.Expression.Type)  /// sub property // to do hierahly cycle
            {
                var submemember = memberExpression.Expression as System.Linq.Expressions.MemberExpression;
                if (submemember != null)
                    parent = submemember.Member.Name+".";
            }

            return parent+((PropertyInfo)memberExpression.Member).Name;
        }
        public static string GetDisplayName<TModel, TResult>(System.Linq.Expressions.Expression<Func<TModel, TResult>> property)
        {
            System.Linq.Expressions.LambdaExpression lambda = (System.Linq.Expressions.LambdaExpression)property;
            System.Linq.Expressions.MemberExpression memberExpression;

            if (lambda.Body is System.Linq.Expressions.UnaryExpression)
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)(lambda.Body);
                memberExpression = (System.Linq.Expressions.MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (System.Linq.Expressions.MemberExpression)(lambda.Body);
            }
            var dispnameattr = memberExpression.Member.CustomAttributes.SingleOrDefault(at => at.AttributeType == typeof(DisplayNameAttribute));
            if (dispnameattr != null && dispnameattr.ConstructorArguments.Count>0)
                return dispnameattr.ConstructorArguments[0].Value.ToString();
            return ((PropertyInfo)memberExpression.Member).Name;
        }

        public static IHtmlContent DisplayNameForEx<TModel,TResult>(this IHtmlHelper<IEnumerable<TModel>> src, Expression<Func<TModel, TResult>> expression, string templateName,  object additionalViewData)
        {
            string displayname = GetDisplayName(expression);
            return src.DisplayFor(x => displayname, templateName, additionalViewData);
           
        }

        public static IHtmlContent DisplayListHeaderForEx<TModel, TResult>(this IHtmlHelper<IEnumerable<TModel>> src, Expression<Func<TModel, TResult>> expression,  dynamic additionalViewData=null,string dispname=default)
        {
            string displayname = dispname;
            if(string.IsNullOrEmpty(displayname))
                displayname=GetDisplayName(expression);
            dynamic addviewdata = new { colnumbers = 2 };
            if (additionalViewData != null)
                addviewdata = Merge(addviewdata, additionalViewData);
            return src.DisplayFor(x => displayname, "ListHeader", (object)addviewdata);

        }
        public static SortControlBuilder<TModel> DisplaySortField<TModel, TResult>(this IHtmlHelper<IEnumerable<TModel>> src, Expression<Func<TModel, TResult>> expression)
        {
            var builder = new SortControlBuilder<TModel>(src);
            builder.AddSortField(expression);
            return builder;

        }
        public static SortControlBuilder<TModel> DisplaySortField<TModel, TResult>(this IHtmlHelper<IEnumerable<TModel>> src, Expression<Func<TModel, TResult>> expression, bool isDefault, bool isDefaultAsc)
        {
            var builder = new SortControlBuilder<TModel>(src);
            builder.AddSortField(expression, isDefault, isDefaultAsc);
            return builder;

        }
        private static dynamic Merge(object item1, object item2)
        {
            IDictionary<string, object> result = new ExpandoObject();

            foreach (var property in item1.GetType().GetProperties())
            {
                if (property.CanRead)
                    result[property.Name] = property.GetValue(item1);
            }

            foreach (var property in item2.GetType().GetProperties())
            {
                if (property.CanRead)
                    result[property.Name] = property.GetValue(item2);
            }

            return result;
        }

        public static bool ContainsHTMLElements<TModel>(this IHtmlHelper<TModel> src,  string text)
        {
            //to do
            if (string.IsNullOrEmpty(text))
                return false;
            return text.Contains("<p>") || text.Contains("<html>") || text.Contains("<br>");
        }
    }
}
