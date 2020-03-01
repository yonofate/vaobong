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
using System.Data.Entity;
using System.Linq;

using Piranha.Entities;

namespace Piranha.Areas.Manager.Models
{
	/// <summary>
	/// Internal comment settings model.
	/// </summary>
	public class CommentSettingsModel
	{
		#region Members
		public static string PARAM_PAGES = "COMMENT_ENABLE_PAGES";
		public static string PARAM_POSTS = "COMMENT_ENABLE_POSTS";
		public static string PARAM_MEDIA = "COMMENT_ENABLE_MEDIA";
		public static string PARAM_UPLOADS = "COMMENT_ENABLE_UPLOADS";
		public static string PARAM_ANONYMOUS = "COMMENT_ENABLE_ANONYMOUS";
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets whether comments should be enabled for pages.
		/// </summary>
		public bool EnablePages { get; set; }

		/// <summary>
		/// Gets/sets whether comments should be enabled for posts.
		/// </summary>
		public bool EnablePosts { get; set; }

		/// <summary>
		/// Gets/sets whether comments should be enabled for media.
		/// </summary>
		public bool EnableMedia { get; set; }

		/// <summary>
		/// Gets/sets whether comments should be enabled for uploads.
		/// </summary>
		public bool EnableUploads { get; set; }

		/// <summary>
		/// Gets/sets whether comments should be enabled by anonymous users.
		/// </summary>
		public bool EnableAnonymous { get; set; }

		public int PageCommentCount { get; set; }
		public int PostCommentCount { get; set; }
		public int MediaCommentCount { get; set; }
		public int UploadCommentCount { get; set; }
		#endregion

		/// <summary>
		/// Gets the current comment settings
		/// </summary>
		/// <returns></returns>
		public static CommentSettingsModel Get() {
			var names = new string[] { PARAM_PAGES, PARAM_POSTS, PARAM_MEDIA, PARAM_UPLOADS, PARAM_ANONYMOUS };
			var model = new CommentSettingsModel();

			using (var db = new DataContext()) {
				var paramList = db.Params.Where(p => names.Contains(p.Name)).ToList();

				// Pages
				var param = paramList.Where(p => p.Name == PARAM_PAGES).SingleOrDefault();
				if (param != null)
					model.EnablePages = param.Value == "1";
				else model.EnablePages = false;
				model.PageCommentCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM comment JOIN page ON comment_parent_id = page_id").First();

				// Posts
				param = paramList.Where(p => p.Name == PARAM_POSTS).SingleOrDefault();
				if (param != null)
					model.EnablePosts = param.Value == "1";
				else model.EnablePosts = true;
				model.PostCommentCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM comment JOIN post ON comment_parent_id = post_id").First();

				// Media
				param = paramList.Where(p => p.Name == PARAM_MEDIA).SingleOrDefault();
				if (param != null)
					model.EnableMedia = param.Value == "1";
				else model.EnableMedia = false;
				model.PostCommentCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM comment JOIN content ON comment_parent_id = content_id").First();

				// Uploads
				param = paramList.Where(p => p.Name == PARAM_UPLOADS).SingleOrDefault();
				if (param != null)
					model.EnableUploads = param.Value == "1";
				else model.EnableUploads = false;
				model.PostCommentCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM comment JOIN upload ON comment_parent_id = upload_id").First();

				// Anonymous
				param = paramList.Where(p => p.Name == PARAM_ANONYMOUS).SingleOrDefault();
				if (param != null)
					model.EnableAnonymous = param.Value == "1";
				else model.EnableAnonymous = false;
			}
			return model;
		}

		/// <summary>
		/// Saves the current comment settings
		/// </summary>
		public void Save() {
			using (var db = new DataContext()) {
				// Pages
				var param = db.Params.Where(p => p.Name == PARAM_PAGES).SingleOrDefault();
				if (param == null) {
					param = new Param() {
						Name = PARAM_PAGES,
						IsLocked = true
					};
					param.Attach(db, EntityState.Added);
				}
				param.Value = EnablePages ? "1" : "0";

				// Posts
				param = db.Params.Where(p => p.Name == PARAM_POSTS).SingleOrDefault();
				if (param == null) {
					param = new Param() {
						Name = PARAM_POSTS,
						IsLocked = true
					};
					param.Attach(db, EntityState.Added);
				}
				param.Value = EnablePosts ? "1" : "0";

				// Media
				param = db.Params.Where(p => p.Name == PARAM_MEDIA).SingleOrDefault();
				if (param == null) {
					param = new Param() {
						Name = PARAM_MEDIA,
						IsLocked = true
					};
					param.Attach(db, EntityState.Added);
				}
				param.Value = EnableMedia ? "1" : "0";

				// Uploads
				param = db.Params.Where(p => p.Name == PARAM_UPLOADS).SingleOrDefault();
				if (param == null) {
					param = new Param() {
						Name = PARAM_UPLOADS,
						IsLocked = true
					};
					param.Attach(db, EntityState.Added);
				}
				param.Value = EnableUploads ? "1" : "0";

				// Anonymous
				param = db.Params.Where(p => p.Name == PARAM_ANONYMOUS).SingleOrDefault();
				if (param == null) {
					param = new Param() {
						Name = PARAM_ANONYMOUS,
						IsLocked = true
					};
					param.Attach(db, EntityState.Added);
				}
				param.Value = EnableAnonymous ? "1" : "0";

				// Save changes
				db.SaveChanges();
			}
		}
	}
}