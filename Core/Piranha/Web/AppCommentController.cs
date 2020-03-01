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
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Entities;

namespace Piranha.Web
{
	/// <summary>
	/// Controller for handling comments from the web application.
	/// </summary>
	public class AppCommentController : Controller
	{
		#region Inner classes
		public class CommentSaveModel
		{
			#region Properties
			public Guid Id { get; set; }
			public Guid ParentId { get; set; }
			public string Title { get; set; }
			public string Body { get; set; }
			public string SessionId { get; set; }
			public string AuthorName { get; set; }
			public string AuthorEmail { get; set; }
			#endregion
		}
		#endregion

		/// <summary>
		/// Saves the given model.
		/// </summary>
		/// <param name="m">The comment</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Save(CommentSaveModel m) {
			if (ModelState.IsValid && m.SessionId == Session.SessionID) {
				using (var db = new DataContext()) {
					// Check if the comment belongs to a standard entity.
					var isPage = db.Pages.Where(p => p.Id == m.ParentId).SingleOrDefault() != null;
					var isPost = !isPage && db.Posts.Where(p => p.Id == m.ParentId).SingleOrDefault() != null;
					var isMedia = !isPage && !isPost && db.Posts.Where(c => c.Id == m.ParentId).SingleOrDefault() != null;
					var isUload = !isPage && !isPost && !isMedia && db.Uploads.Where(u => u.Id == m.ParentId).SingleOrDefault() != null;

					// Now get the comment settings
					var sett = Areas.Manager.Models.CommentSettingsModel.Get();

					// Check comment settings according to type
					if (isPage && (!sett.EnablePages || (!Application.Current.UserProvider.IsAuthenticated && !sett.EnableAnonymous)))
						return Redirect("~/");
					else if (isPost && (!sett.EnablePosts || (!Application.Current.UserProvider.IsAuthenticated && !sett.EnableAnonymous)))
						return Redirect("~/");
					else if (isMedia && (!sett.EnableMedia || (!Application.Current.UserProvider.IsAuthenticated && !sett.EnableAnonymous)))
						return Redirect("~/");
					else if (isUload && (!sett.EnableUploads || (!Application.Current.UserProvider.IsAuthenticated && !sett.EnableAnonymous)))
						return Redirect("~/");

					Comment comment = null;

					// Try to load the comment if this is an update
					if (m.Id != null)
						comment = db.Comments.Where(c => c.Id == m.Id).SingleOrDefault();

					// If no existing comment was found, create a new
					if (comment == null) {
						comment = new Comment();
						comment.Attach(db, EntityState.Added);
					}

					// If the user isn't authenticated, add user info
					if (!Application.Current.UserProvider.IsAuthenticated) {
						comment.AuthorName = m.AuthorName;
						comment.AuthorEmail = m.AuthorEmail;
					}

					// Update standard properties
					comment.Title = m.Title;
					comment.Body = m.Body;

					db.SaveChanges();
				}
			}
			return new RedirectResult("~/");
		}
	}
}
