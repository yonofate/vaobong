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

using Piranha.Models;
using Piranha.Models.Manager.TemplateModels;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager controller for handling page & post templates.
	/// </summary>
	public class TemplateController : ManagerController
	{
		#region Page templates
		/// <summary>
		/// Gets the list of all page templates.
		/// </summary>
		[Access(Function = "ADMIN_PAGE_TEMPLATE")]
		public ActionResult PageList() {
			return View("PageList", PageListModel.Get());
		}

		/// <summary>
		/// Opens the insert or edit view for the template depending on
		/// whether a template id was passed to the action or not.
		/// </summary>
		/// <param name="id">The template id</param>
		[Access(Function = "ADMIN_PAGE_TEMPLATE")]
		public ActionResult Page(string id = "") {
			PageEditModel m = new PageEditModel();

			if (id != "") {
				m = PageEditModel.GetById(new Guid(id));
				ViewBag.Title = Piranha.Resources.Template.EditPageTitleExisting;
			} else {
				ViewBag.Title = Piranha.Resources.Template.EditPageTitleNew;
			}
			return View("PageEdit", m);
		}

		/// <summary>
		/// Saves the current template.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		[Access(Function = "ADMIN_PAGE_TEMPLATE")]
		public ActionResult Page(PageEditModel m) {
			if (ModelState.IsValid) {
				if (m.SaveAll()) {
					ModelState.Clear();
					ViewBag.Title = Piranha.Resources.Template.EditPageTitleExisting;
					SuccessMessage(Piranha.Resources.Template.MessagePageSaved);
				} else ErrorMessage(Piranha.Resources.Template.MessagePageNotSaved);
			} else {
				if (m.Template.IsNew)
					ViewBag.Title = Piranha.Resources.Template.EditPageTitleNew;
				else ViewBag.Title = Piranha.Resources.Template.EditPageTitleExisting;
			}
			return View("PageEdit", m);
		}

		/// <summary>
		/// Deletes the specified page template.
		/// </summary>
		/// <param name="id">The template id</param>
		[Access(Function = "ADMIN_PAGE_TEMPLATE")]
		public ActionResult DeletePage(string id) {
			PageEditModel pm = PageEditModel.GetById(new Guid(id));

			if (pm.DeleteAll())
				SuccessMessage(Piranha.Resources.Template.MessagePageDeleted);
			else ErrorMessage(Piranha.Resources.Template.MessagePageNotDeleted);
			return RedirectToAction("pagelist");
		}
		#endregion

		#region Post templates
		/// <summary>
		/// Gets the list of post templates.
		/// </summary>
		[Access(Function = "ADMIN_POST_TEMPLATE")]
		public ActionResult PostList() {
			return View("PostList", PostListModel.Get());
		}

		/// <summary>
		/// Opens the insert or edit view for the template depending on
		/// whether a template id was passed to the action or not.
		/// </summary>
		/// <param name="id">The template id</param>
		[Access(Function = "ADMIN_POST_TEMPLATE")]
		public ActionResult Post(string id = "") {
			PostEditModel m = new PostEditModel();

			if (id != "") {
				m = PostEditModel.GetById(new Guid(id));
				ViewBag.Title = Piranha.Resources.Template.EditPostTitleExisting;
			} else {
				ViewBag.Title = Piranha.Resources.Template.EditPostTitleNew;
			}
			return View("PostEdit", m);
		}

		/// <summary>
		/// Saves the current template.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		[Access(Function = "ADMIN_POST_TEMPLATE")]
		public ActionResult Post(PostEditModel m) {
			ViewBag.Title = Piranha.Resources.Template.EditPostTitleNew;

			if (ModelState.IsValid) {
				try {
					if (m.SaveAll()) {
						ModelState.Clear();
						ViewBag.Title = Piranha.Resources.Template.EditPostTitleExisting;
						SuccessMessage(Piranha.Resources.Template.MessagePostSaved);
					} else ErrorMessage(Piranha.Resources.Template.MessagePostNotSaved);
				} catch (DuplicatePermalinkException) {
					// Manually set the duplicate error.
					ModelState.AddModelError("Permalink", @Piranha.Resources.Global.PermalinkDuplicate);
					// If this is the default permalink, remove the model state so it will be shown.
					if (Permalink.Generate(m.Template.Name) == m.Permalink.Name)
						ModelState.Remove("Permalink.Name");
				} catch (Exception e) {
					ErrorMessage(e.ToString());
				}
			}
			return View("PostEdit", m);
		}

		/// <summary>
		/// Deletes the specified post template.
		/// </summary>
		/// <param name="id">The template id</param>
		[Access(Function = "ADMIN_POST_TEMPLATE")]
		public ActionResult DeletePost(string id) {
			PostEditModel pm = PostEditModel.GetById(new Guid(id));

			if (pm.DeleteAll())
				SuccessMessage(Piranha.Resources.Template.MessagePostDeleted);
			else ErrorMessage(Piranha.Resources.Template.MessagePostNotDeleted);
			return RedirectToAction("postlist");
		}
		#endregion

		#region Region templates
		/// <summary>
		/// Creates a new region template row from the given data.
		/// </summary>
		/// <param name="m">The model</param>
		/// <returns>The region template</returns>
		[HttpPost()]
		public ActionResult RegionTemplate(RegionInsertModel m) {
			var region = new Piranha.Models.RegionTemplate() {
				TemplateId = m.TemplateId,
				Name = m.Name,
				InternalId = m.InternalId,
				Type = m.Type,
				Seqno = m.Seqno
			};
			return View("Region", region);
		}
		#endregion
	}
}
