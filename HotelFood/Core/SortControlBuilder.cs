
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public class SortField
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string SortType { get; set; }

        public bool IsDefault { get; set; }
        
    }
    public class SortFieldModel
    {
        public SortFieldModel(QueryModel queryModel)
        {
            FieldList = new List<SortField>();
            QueryModel = queryModel;
        }
        public List<SortField> FieldList { get; private set; }

        public QueryModel QueryModel { get; private set; }
    }
    public class SortControlBuilder<TModel>
    {
        IHtmlHelper<IEnumerable<TModel>> htmlhelper;
        public SortControlBuilder(IHtmlHelper<IEnumerable<TModel>> _htmlhelper)
        {
            htmlhelper = _htmlhelper;
        }
        public List<ExpressionHolderBase> FieldList=new List<ExpressionHolderBase>();
        public SortControlBuilder<TModel> AddSortField<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            FieldList.Add(new ExpressionHolder<TModel, TResult>(expression));
            return this;
        }
        public SortControlBuilder<TModel> AddSortField<TResult>(Expression<Func<TModel, TResult>> expression, bool isDefault, bool isDefaultAsc)
        {
            FieldList.Add(new ExpressionHolder<TModel, TResult>(expression, isDefault, isDefaultAsc));
            return this;
        }
        public IHtmlContent Display(QueryModel querymodel=null)
        {
            var model = new SortFieldModel(querymodel);
            FieldList.ForEach(f => {
                model.FieldList.Add(new SortField() { FieldName = f.GetFieldName(), DisplayName = f.GetDisplayName(), SortType = "asc",IsDefault=(f.IsDefault && f.IsDefaultAsc) });
                model.FieldList.Add(new SortField() { FieldName = f.GetFieldName(), DisplayName = f.GetDisplayName(), SortType = "desc", IsDefault = (f.IsDefault && !f.IsDefaultAsc) });
                }
            );
            return htmlhelper.DisplayFor(x=> model, "SortControl");
        }
    }
    public abstract class ExpressionHolderBase
    {
        public string SortType { get; set; }
        public abstract string GetFieldName();
        public abstract string GetDisplayName();
        public bool IsDefault { get; set; }

        public bool IsDefaultAsc { get; set; }
    }
    public class ExpressionHolder<TModel, TResult>: ExpressionHolderBase
    {
        public  ExpressionHolder(Expression<Func<TModel, TResult>> expr)
        {
            Expr = expr;
        }
        public ExpressionHolder(Expression<Func<TModel, TResult>> expr,bool isDefault,bool isDefaultAsc):this(expr)
        {
            IsDefault = isDefault;
            IsDefaultAsc = isDefaultAsc;
        }

        public Expression<Func<TModel, TResult>> Expr { get; set; }
        public  override string GetFieldName()
        {
            return RazorExtensions.GetPropertyName(Expr);
        }
        public override string GetDisplayName()
        {
            return RazorExtensions.GetDisplayName(Expr); ;
        }
    }

}
