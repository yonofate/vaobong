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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using Piranha.Models;
using Piranha.Web;

namespace Piranha.WebPages
{
	/// <summary>
	/// Base class for all content pages.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class ContentPage<T> : BasePage
	{
		#region Properties
		/// <summary>
		/// Gets/sets the content model.
		/// </summary>
		public new T Model { get; protected set; }

		/// <summary>
		/// Gets/sets the form helper.
		/// </summary>
		public FormHelper<T> Form { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ContentPage()
			: base() {
			Form = new FormHelper<T>(this);
		}

		/// <summary>
		/// Initializes the page.
		/// </summary>
		protected override void InitializePage() {
			// Create the model if it's not initialized
			if (Model == null)
				Model = Activator.CreateInstance<T>();

			// Get all model properties
			var properties = Model.GetType().GetProperties();

			// Check for model properties marked for caching.
			foreach (var prop in properties) {
				var attr = prop.GetCustomAttribute<ModelPropertyAttribute>(true);
				if (attr != null && (!IsPost || attr.LoadOnPost)) {
					var name = "CACHE_" + this.GetType().Name.ToUpper() + "_" +
						(Model is PageModel ? ((PageModel)(object)Model).Page.Permalink.ToUpper() + "_" : "") +
						(Model is PostModel ? ((PostModel)(object)Model).Post.Permalink.ToUpper() + "_" : "") + prop.Name.ToUpper();
					if ((!(Model is PageModel) || !((PageModel)(object)Model).Page.IsDraft) && Cache[name] != null) {
						prop.SetValue(Model, Cache.Get(name), null);
					} else {
						// Check if we have a load method defined
						if (!String.IsNullOrEmpty(attr.OnLoad)) {
							var method = Model.GetType().GetMethod(attr.OnLoad);
							if (method != null) {
								method.Invoke(Model, null);
							}
						}
					}
				}
			}

			base.InitializePage();

			// Now check if we should update some cache.
			foreach (var prop in properties) {
				var attr = prop.GetCustomAttribute<ModelPropertyAttribute>(true);
				if (attr != null && (!IsPost || attr.LoadOnPost)) {
					var name = "CACHE_" + this.GetType().Name.ToUpper() + "_" +
						(Model is PageModel ? ((PageModel)(object)Model).Page.Permalink.ToUpper() + "_" : "") +
						(Model is PostModel ? ((PostModel)(object)Model).Post.Permalink.ToUpper() + "_" : "") + prop.Name.ToUpper();
					if ((!(Model is PageModel) || !((PageModel)(object)Model).Page.IsDraft) && Cache[name] == null) {
						if (attr.AbsoluteExpiration > 0) {
							Cache.Add(name, prop.GetValue(Model, null), null, DateTime.Now.AddMinutes(attr.AbsoluteExpiration), System.Web.Caching.Cache.NoSlidingExpiration,
								attr.Priority, null);
						} else if (attr.SlidingExpiration > 0) {
							Cache.Add(name, prop.GetValue(Model, null), null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, attr.SlidingExpiration, 0),
								attr.Priority, null);
						}
					}
				}
			}
		}
	}
}
