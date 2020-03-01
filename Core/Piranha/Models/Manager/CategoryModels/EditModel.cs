﻿/*
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
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Models.Manager.CategoryModels
{
	/// <summary>
	/// Edit model for the category object.
	/// </summary>
	public class EditModel
	{
		#region Binder
		public class Binder : DefaultModelBinder
		{
			/// <summary>
			/// Extend the default binder so that html strings can be fetched from the post.
			/// </summary>
			/// <param name="controllerContext">Controller context</param>
			/// <param name="bindingContext">Binding context</param>
			/// <returns>The page edit model</returns>
			public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				EditModel model = (EditModel)base.BindModel(controllerContext, bindingContext);

				// Allow HtmlString extensions
				model.Extensions.Each((i, m) => {
					if (m.Body is HtmlString) {
						bindingContext.ModelState.Remove("Extensions[" + i + "].Body");
						m.Body = ExtensionManager.Current.CreateInstance(m.Type,
							bindingContext.ValueProvider.GetUnvalidatedValue("Extensions[" + i + "].Body").AttemptedValue);
					}
				});
				return model;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the current category.
		/// </summary>
		public Category Category { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get; set; }

		/// <summary>
		/// Gets/sets the categories.
		/// </summary>
		public SelectList Categories { get; set; }

		/// <summary>
		/// Gets/sets the available extensions.
		/// </summary>
		public List<Extension> Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public EditModel() {
			Category = new Category() {
				Id = Guid.NewGuid()
			};
			GetRelated();
		}

		/// <summary>
		/// Gets the edit model for the category with the given id.
		/// </summary>
		/// <param name="id">The category id</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Guid id) {
			EditModel m = new EditModel() {
				Category = Category.GetSingle(id)
			};
			m.GetRelated();

			return m;
		}

		/// <summary>
		/// Saves the edit model.
		/// </summary>
		public bool SaveAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					if (Permalink.IsNew)
						Permalink.Name = Permalink.Generate(Category.Name);
					Permalink.Save(tx);
					Category.Save(tx);
					foreach (var ext in Extensions) {
						// Call OnSave
						ext.Body.OnManagerSave(Category);

						ext.ParentId = Category.Id;
						ext.Save(tx);
					}
					tx.Commit();
				} catch { tx.Rollback(); throw; }
			}
			Refresh();

			return true;
		}

		/// <summary>
		/// Deletes the model and all related data.
		/// </summary>
		public bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					// Call OnDelete for all extensions
					Extensions.ForEach(e => e.Body.OnManagerDelete(Category));

					// Delete all relations to the current category
					List<Relation> pc = Relation.GetByTypeAndRelatedId(Relation.RelationType.POSTCATEGORY, Category.Id);
					pc.ForEach((r) => r.Delete(tx));

					// Delete category
					Category.Delete(tx);

					// Delete permalink
					Permalink.Delete(tx);
					tx.Commit();
				} catch { tx.Rollback(); return false; }
			}
			return true;
		}

		/// <summary>
		/// Refreshes the model from the database.
		/// </summary>
		public void Refresh() {
			if (Category != null) {
				if (!Category.IsNew) {
					Category = Category.GetSingle(Category.Id);
				}
				GetRelated();
			}
		}

		/// <summary>
		/// Gets the related information
		/// </summary>
		private void GetRelated() {
			// Get Permalink
			Permalink = Permalink.GetSingle(Category.PermalinkId);
			if (Permalink == null) {
				Permalink = new Permalink() {
					Id = Guid.NewGuid(),
					Type = Models.Permalink.PermalinkType.CATEGORY,
					NamespaceId = new Guid("AE46C4C4-20F7-4582-888D-DFC148FE9067")
				};
				Category.PermalinkId = Permalink.Id;
			}

			// Get categories
			List<Category> cats = Piranha.Models.Category.Get("category_id != @0", Category.Id,
				new Params() { OrderBy = "category_name ASC" });
			cats.Insert(0, new Category());
			Categories = new SelectList(cats, "Id", "Name");

			// Get extensions
			Extensions = Category.GetExtensions(true);
		}
	}
}
