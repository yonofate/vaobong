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
//using System.Data;
using System.Linq;
//using System.Web;

using Piranha.Entities;

namespace Piranha.Areas.Manager.Models
{
	/// <summary>
	/// View model for the comment list view.
	/// </summary>
	public class CommentListModel
	{
		#region Inner classes
		/// <summary>
		/// Internal comment view model.
		/// </summary>
		public class CommentModel
		{
			/// <summary>
			/// Gets/sets the id.
			/// </summary>
			public Guid Id { get; set; }

			/// <summary>
			/// Gets/sets the comment title.
			/// </summary>
			public string Title { get; set; }

			/// <summary>
			/// Gets/sets the current comment status.
			/// </summary>
			public Comment.CommentStatus Status { get; set; }

			/// <summary>
			/// Gets the status name.
			/// </summary>
			public string StatusName {
				get { return Comment.GetStatusName(Status); }
			}

			/// <summary>
			/// Gets/sets the author name.
			/// </summary>
			public string AuthorName { get; set; }

			/// <summary>
			/// Gets/sets the autor email.
			/// </summary>
			public string AuthorEmail { get; set; }

			/// <summary>
			/// Gets/sets the created date.
			/// </summary>
			public DateTime Created { get; set; }
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the available comments.
		/// </summary>
		public List<CommentModel> Comments { get; set; }
		#endregion

		/// <summary>
		/// Gets the view model for the comment list view.
		/// </summary>
		/// <returns>The model</returns>
		public static CommentListModel Get() {
			var m = new CommentListModel();

			using (var db = new DataContext()) {
				m.Comments = db.Comments.Include("CreatedBy").Select(c => new CommentModel() {
					Id = c.Id,
					Title = !String.IsNullOrEmpty(c.Title) ? c.Title : c.Body.Substring(0, 32) + "...",
					Created = c.Created,
					AuthorName = c.CreatedBy != null ? c.CreatedBy.Firstname + " " + c.CreatedBy.Surname : c.AuthorName,
					AuthorEmail = c.CreatedBy != null ? c.CreatedBy.Email : c.AuthorEmail,
					Status = (Comment.CommentStatus)c.InternalStatus,
				}).OrderByDescending(c => c.Created).ToList();
			}
			return m;
		}

		/// <summary>
		/// Gets the comments for the given parent id.
		/// </summary>
		/// <param name="parentid">The parent id.</param>
		/// <returns>The comments</returns>
		public static List<Comment> GetCommentList(Guid parentid) {
			using (var db = new DataContext()) {
				return db.Comments.
					Include("CreatedBy").
					Where(c => c.ParentId == parentid && c.ParentIsDraft == false).
					OrderByDescending(c => c.Created).ToList();
			}
		}
	}
}