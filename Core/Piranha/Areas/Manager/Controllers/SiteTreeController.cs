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

using Piranha.Areas.Manager.Models;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager controller for handling site trees.
	/// </summary>
	public class SiteTreeController : ManagerController
	{
		/// <summary>
		/// Gets the list view for the site tree.
		/// </summary>
		[Access(Function = "ADMIN_SITETREE")]
		public ActionResult Index() {
			return View("Index", SiteTreeListModel.Get());
		}

		/// <summary>
		/// Edits or creates a new site tree.
		/// </summary>
		/// <param name="id">The site tree id</param>
		[Access(Function = "ADMIN_SITETREE")]
		public ActionResult Edit(string id = "") {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = Resources.SiteTree.EditTitleExisting;
				return View(SiteTreeEditModel.GetById(new Guid(id)));
			}
			ViewBag.Title = Resources.SiteTree.EditTitleNew;
			return View(new SiteTreeEditModel());
		}

		/// <summary>
		/// Saves the given edit model.
		/// </summary>
		/// <param name="m">The edit model</param>
		[HttpPost(), ValidateInput(false)]
		[Access(Function = "ADMIN_SITETREE")]
		public ActionResult Edit(SiteTreeEditModel m) {
			if (ModelState.IsValid) {
				if (m.Save()) {
					ViewBag.Title = Resources.SiteTree.EditTitleExisting;
					SuccessMessage(Resources.SiteTree.MessageSaved);
					ModelState.Clear();
				} else {
					if (m.Id == Guid.Empty)
						ViewBag.Title = Resources.SiteTree.EditTitleNew;
					else ViewBag.Title = Resources.SiteTree.EditTitleExisting;

					ErrorMessage(Resources.SiteTree.MessageNotSaved);
				}
			}
			return View(m);
		}

		/// <summary>
		/// Deletes the given model.
		/// </summary>
		/// <param name="id">The site tree id</param>
		[Access(Function = "ADMIN_SITETREE")]
		public ActionResult Delete(string id) {
			var m = SiteTreeEditModel.GetById(new Guid(id));

			if (m.Delete()) {
				SuccessMessage(Resources.SiteTree.MessageDeleted);
			} else SuccessMessage(Resources.SiteTree.MessageNotDeleted);

			return Index();
		}
	}
}
