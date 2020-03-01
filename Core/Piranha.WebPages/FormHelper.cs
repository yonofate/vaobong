/*
 * Copyright (c) 2011-2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.WebPages.Html;

namespace Piranha.WebPages
{
	/// <summary>
	/// The Piranha form helpers for model binder.
	/// </summary>
	/// <typeparam name="TModel">The model type</typeparam>
	public class FormHelper<TModel>
	{
		#region Members
		private ContentPage<TModel> Page;
		#endregion

		/// <summary>
		/// Default constructor. Creates a new form helper.
		/// </summary>
		/// <param name="page">The current page.</param>
		public FormHelper(ContentPage<TModel> page) {
			Page = page;
		}

		/// <summary>
		/// Renders the hidden form action field.
		/// </summary>
		/// <param name="action">The name of the method to invoke</param>
		/// <returns>The hidden field</returns>
		public IHtmlString Action(string action) {
			return Page.Html.Hidden("piranha_form_action", action);
		}

		/// <summary>
		/// Renders a label for the the specified model property
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="labelText">The label text</param>
		/// <returns>The label</returns>
		public IHtmlString LabelFor<TProperty>(Expression<Func<TModel, TProperty>> expr, string labelText) {
			return Page.Html.Label(labelText, expr.Body.ToString());
		}

		/// <summary>
		/// Renders a label for the the specified model property
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="labelText">The label text</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The label</returns>
		public IHtmlString LabelFor<TProperty>(Expression<Func<TModel, TProperty>> expr, string labelText, object htmlAttributes) {
			return Page.Html.Label(labelText, expr.Body.ToString(), htmlAttributes);
		}

		/// <summary>
		/// Gets a textbox for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The textbox</returns>
		public IHtmlString TextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.TextBox(expr.Body.ToString(), GetValue(expr));
		}

		/// <summary>
		/// Gets a textbox for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The textbox</returns>
		public IHtmlString TextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expr, object htmlAttributes) {
			return Page.Html.TextBox(expr.Body.ToString(), GetValue(expr), htmlAttributes);
		}

		/// <summary>
		/// Gets a textarea for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The textbox</returns>
		public IHtmlString TextAreaFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.TextArea(expr.Body.ToString(), GetValue(expr).ToString());
		}

		/// <summary>
		/// Gets a textarea for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The textbox</returns>
		public IHtmlString TextAreaFor<TProperty>(Expression<Func<TModel, TProperty>> expr, object htmlAttributes) {
			return Page.Html.TextArea(expr.Body.ToString(), GetValue(expr).ToString(), htmlAttributes);
		}

		/// <summary>
		/// Gets a checkbox for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The checkbox</returns>
		public IHtmlString CheckBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.CheckBox(expr.Body.ToString(), (bool)GetValue(expr), new { @value = "true" });
		}

		/// <summary>
		/// Gets a password for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The password</returns>
		public IHtmlString PasswordFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.Password(expr.Body.ToString());
		}

		/// <summary>
		/// Gets a password for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The password</returns>
		public IHtmlString PasswordFor<TProperty>(Expression<Func<TModel, TProperty>> expr, object htmlAttributes) {
			return Page.Html.Password(expr.Body.ToString(), null, htmlAttributes);
		}

		/// <summary>
		/// Gets a hidden field for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The hidden field</returns>
		public IHtmlString HiddenFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.Hidden(expr.Body.ToString(), GetValue(expr));
		}

		/// <summary>
		/// Gets a hidden field for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The hidden field</returns>
		public IHtmlString HiddenFor<TProperty>(Expression<Func<TModel, TProperty>> expr, object htmlAttributes) {
			return Page.Html.Hidden(expr.Body.ToString(), GetValue(expr), htmlAttributes);
		}

		/// <summary>
		/// Gets a sekect list for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The select list</returns>
		public IHtmlString DropDownListFor<TProperty>(Expression<Func<TModel, TProperty>> expr, IList<SelectListItem> selectList) {
			return Page.Html.DropDownList(expr.Body.ToString(), "", selectList, GetValue(expr), null);
		}

		/// <summary>
		/// Gets a sekect list for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <returns>The select list</returns>
		public IHtmlString DropDownListFor<TProperty>(Expression<Func<TModel, TProperty>> expr, IList<SelectListItem> selectList, object htmlAttributes) {
			return Page.Html.DropDownList(expr.Body.ToString(), "", selectList, GetValue(expr), htmlAttributes);
		}

		/// <summary>
		/// Gets a validation message for the specified model property.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The model expression</param>
		/// <returns>The validation message</returns>
		public IHtmlString ValidationMessageFor<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			return Page.Html.ValidationMessage(expr.Body.ToString());
		}

		#region Private methods
		/// <summary>
		/// Gets the model value for the given expression, either from the current
		/// ModelState or from the current model.
		/// </summary>
		/// <typeparam name="TProperty">The property type</typeparam>
		/// <param name="expr">The expression</param>
		/// <returns>The expression value</returns>
		private object GetValue<TProperty>(Expression<Func<TModel, TProperty>> expr) {
			var name = expr.Body.ToString();
			var state = Page.ModelState[name];

			if (state != null)
				return Page.ModelState[name].Value;

			var ret = expr.Compile()(Page.Model);
			if (ret != null)
				return ret;
			return "";
		}
		#endregion
	}
}