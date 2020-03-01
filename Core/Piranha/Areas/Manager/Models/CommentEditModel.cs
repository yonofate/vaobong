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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using Piranha.Entities;

namespace Piranha.Areas.Manager.Models
{
	/// <summary>
	/// View model for the comment edit view.
	/// </summary>
	public class CommentEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the comment id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the id of the entity the comment is attached to.
		/// </summary>
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the current comment status.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Global), Name = "Status")]
		public Comment.CommentStatus Status { get; set; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Global), Name = "Title")]
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the body.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Global), Name = "Content")]
		public string Body { get; set; }

		/// <summary>
		/// Gets/sets the author name.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Comment), Name = "Author")]
		public string AuthorName { get; set; }

		/// <summary>
		/// Gets/sets the author email.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "Email")]
		public string AuthorEmail { get; set; }

		/// <summary>
		/// Gets/sets the optional id of the sysuser who created the comment.
		/// </summary>
		public Guid? CreatedById { get; set; }

		/// <summary>
		/// Gets/sets the available statuses.
		/// </summary>
		public SelectList Statuses { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CommentEditModel() {
			var enums = from Enum e in Enum.GetValues(typeof(Comment.CommentStatus))
						select new { Id = e, Name = Comment.GetStatusName((Comment.CommentStatus)e) };
			Statuses = new SelectList(enums, "Id", "Name");
		}

		/// <summary>
		/// Gets the edit model for the comment with the given id.
		/// </summary>
		/// <param name="id">The comment id.</param>
		/// <returns></returns>
		public static CommentEditModel GetById(Guid id) {
			using (var db = new DataContext()) {
				return db.Comments.Where(c => c.Id == id).Include(c => c.CreatedBy).Select(c =>
					new CommentEditModel() {
						Id = c.Id,
						ParentId = c.ParentId,
						Status = (Comment.CommentStatus)c.InternalStatus,
						Title = c.Title,
						Body = c.Body,
						AuthorName = c.CreatedBy != null ? c.CreatedBy.Firstname + " " + c.CreatedBy.Surname : c.AuthorName,
						AuthorEmail = c.CreatedBy != null ? c.CreatedBy.Email : c.AuthorEmail,
						CreatedById = c.CreatedById
					}).SingleOrDefault();
			}
		}

		/// <summary>
		/// Saves the current edit model.
		/// </summary>
		/// <returns>Whether the entity was updated or not</returns>
		public bool Save() {
			using (var db = new DataContext()) {
				var comment = db.Comments.Where(c => c.Id == Id).SingleOrDefault();
				if (comment == null) {
					comment = new Comment();
					comment.Attach(db, EntityState.Added);

					comment.ParentId = ParentId;
					comment.Title = Title;
					comment.Body = Body;
				}
				comment.Status = Status;

				return db.SaveChanges() > 0;
			}
		}

		/// <summary>
		/// Deletes the current comment.
		/// </summary>
		/// <returns>Whether the entity was removed or not</returns>
		public bool Delete() {
			using (var db = new DataContext()) {
				var comment = db.Comments.Where(c => c.Id == Id).SingleOrDefault();
				if (comment == null) {
					db.Comments.Remove(comment);
					return db.SaveChanges() > 0;
				}
				return false;
			}
		}
	}
}