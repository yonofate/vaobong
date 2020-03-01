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
using System.Web.Mvc;

using Piranha.Models.Manager.CategoryModels;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager area controller for the category entity.
	/// </summary>
	public class CategoryController : ManagerController
	{
		/// <summary>
		/// Gets the list view for the categories.
		/// </summary>
		[Access(Function = "ADMIN_CATEGORY")]
		public ActionResult Index() {
			var m = ListModel.Get();

			// Executes the category list loaded hook, if registered
			if (WebPages.Hooks.Manager.CategoryListModelLoaded != null)
				WebPages.Hooks.Manager.CategoryListModelLoaded(this, WebPages.Manager.GetActiveMenuItem(), m);

			return View("Index", m);
		}

		/// <summary>
		/// Edits or inserts a new category.
		/// </summary>
		/// <param name="id">The category id</param>
		[Access(Function = "ADMIN_CATEGORY")]
		public ActionResult Edit(string id = "") {
			EditModel m = id != "" ? EditModel.GetById(new Guid(id)) : new EditModel();

			if (m.Category.IsNew) {
				ViewBag.Title = Piranha.Resources.Category.EditTitleExisting;
			} else {
				ViewBag.Title = Piranha.Resources.Category.EditTitleNew;
			}

			// Executes the category edit loaded hook, if registered
			if (WebPages.Hooks.Manager.CategoryEditModelLoaded != null)
				WebPages.Hooks.Manager.CategoryEditModelLoaded(this, WebPages.Manager.GetActiveMenuItem(), m);

			return View("Edit", m);
		}

		/// <summary>
		/// Saves the given model.
		/// </summary>
		/// <param name="m">The model</param>
		/// <returns></returns>
		[HttpPost(), ValidateInput(false)]
		[Access(Function = "ADMIN_CATEGORY")]
		public ActionResult Edit(EditModel m) {
			if (ModelState.IsValid) {
				// Executes the category edit before save hook, if registered
				if (WebPages.Hooks.Manager.CategoryEditModelBeforeSave != null)
					WebPages.Hooks.Manager.CategoryEditModelBeforeSave(this, WebPages.Manager.GetActiveMenuItem(), m);

				if (m.SaveAll()) {
					// Executes the category edit before save hook, if registered
					if (WebPages.Hooks.Manager.CategoryEditModelAfterSave != null)
						WebPages.Hooks.Manager.CategoryEditModelAfterSave(this, WebPages.Manager.GetActiveMenuItem(), m);

					ViewBag.Title = Piranha.Resources.Category.EditTitleExisting;
					SuccessMessage(Piranha.Resources.Category.MessageSaved);
					ModelState.Clear();
				} else {
					ViewBag.Title = Piranha.Resources.Category.EditTitleNew;
					ErrorMessage(Piranha.Resources.Category.MessageNotSaved);
				}
			}
			return View("Edit", m);
		}

		/// <summary>
		/// Deletes the category with the given id.
		/// </summary>
		/// <param name="id">The category id</param>
		[Access(Function = "ADMIN_CATEGORY")]
		public ActionResult Delete(string id) {
			EditModel m = EditModel.GetById(new Guid(id));

			if (m.DeleteAll())
				SuccessMessage(Piranha.Resources.Category.MessageDeleted);
			else ErrorMessage(Piranha.Resources.Category.MessageNotDeleted);
			return RedirectToAction("index");
		}
	}
}
