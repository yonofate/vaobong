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
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Areas.Manager.Models;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager controller for comments
	/// </summary>
	public class CommentController : ManagerController
	{
		#region Internal classes
		public class CommentResult
		{
			public int Success { get; set; }
			public int New { get; set; }
		}
		#endregion

		/// <summary>
		/// Gets the list view.
		/// </summary>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Index() {
			return View(CommentListModel.Get());
		}

		/// <summary>
		/// Gets the edit view for the comment with the given id.
		/// </summary>
		/// <param name="id">The comment id</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Edit(string id) {
			return View(CommentEditModel.GetById(new Guid(id)));
		}

		/// <summary>
		/// Saves the edit model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost()]
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Edit(CommentEditModel m) {
			if (ModelState.IsValid) {
				m.Save();
				SuccessMessage(Piranha.Resources.Comment.MessageSaved);
				ModelState.Clear();
			}
			return View(m);
		}

		/// <summary>
		/// Deletes the comment with the given id.
		/// </summary>
		/// <param name="id">The comment id.</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Delete(string id) {
			var m = CommentEditModel.GetById(new Guid(id));
			if (m.Delete()) {
				SuccessMessage(Piranha.Resources.Comment.MessageDeleted);
			} else ErrorMessage(Piranha.Resources.Comment.MessageNotDeleted);
			return Index();
		}

		/// <summary>
		/// Gets the settings view for the comments.
		/// </summary>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Settings() {
			return View(CommentSettingsModel.Get());
		}

		/// <summary>
		/// Saves the given settings model.
		/// </summary>
		/// <param name="m">The model.</param>
		[HttpPost()]
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult Settings(CommentSettingsModel m) {
			if (ModelState.IsValid) {
				m.Save();
				SuccessMessage(Piranha.Resources.Comment.MessageSettingsSaved);
			} else ErrorMessage(Piranha.Resources.Comment.MessageSettingsNotSaved);
			return View(m);
		}

		/// <summary>
		/// Gets the comment list for the given parent id.
		/// </summary>
		/// <param name="parentid">The parent id</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult List(string id) {
			return View(CommentListModel.GetCommentList(new Guid(id)));
		}

		/// <summary>
		/// Approves the given comment and returns the number of new comments
		/// available for the given comment.
		/// </summary>
		/// <param name="id">The comment id</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult AjaxApprove(string id) {
			using (var db = new DataContext()) {
				var comment = db.Comments.Where(c => c.Id == new Guid(id)).SingleOrDefault();
				if (comment != null) {
					comment.Status = Entities.Comment.CommentStatus.Approved;
					db.SaveChanges();

					return Json(new CommentResult() {
						Success = 1,
						New = db.Comments.Where(c => c.ParentId == comment.ParentId && c.InternalStatus == 0).Count()
					}, JsonRequestBehavior.AllowGet);
				}
				return Json(new CommentResult() {
					Success = 0
				}, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Rejects the given comment and returns the number of new comments
		/// available for the given comment.
		/// </summary>
		/// <param name="id">The comment id</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult AjaxReject(string id) {
			using (var db = new DataContext()) {
				var comment = db.Comments.Where(c => c.Id == new Guid(id)).SingleOrDefault();
				if (comment != null) {
					comment.Status = Entities.Comment.CommentStatus.NotApproved;
					db.SaveChanges();

					return Json(new CommentResult() {
						Success = 1,
						New = db.Comments.Where(c => c.ParentId == comment.ParentId && c.InternalStatus == 0).Count()
					}, JsonRequestBehavior.AllowGet);
				}
				return Json(new CommentResult() {
					Success = 0
				}, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Deletes the given comment and returns the number of new comments
		/// available for the given comment.
		/// </summary>
		/// <param name="id">The comment id</param>
		[Access(Function = "ADMIN_COMMENT")]
		public ActionResult AjaxDelete(string id) {
			using (var db = new DataContext()) {
				var comment = db.Comments.Where(c => c.Id == new Guid(id)).SingleOrDefault();
				if (comment != null) {
					db.Comments.Remove(comment);
					db.SaveChanges();

					return Json(new CommentResult() {
						Success = 1,
						New = db.Comments.Where(c => c.ParentId == comment.ParentId && c.InternalStatus == 0).Count()
					}, JsonRequestBehavior.AllowGet);
				}
				return Json(new CommentResult() {
					Success = 0
				}, JsonRequestBehavior.AllowGet);
			}
		}
	}
}
