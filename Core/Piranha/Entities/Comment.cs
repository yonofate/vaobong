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
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Piranha.Entities
{
	/// <summary>
	/// Entity for a comment. A comment can be attached to any entity.
	/// </summary>
	[Serializable]
	public class Comment : BaseEntity
	{
		#region Inner classes
		/// <summary>
		/// Inner class representing the author of a comment.
		/// </summary>
		public class CommentAuthor
		{
			/// <summary>
			/// Gets the name of the author.
			/// </summary>
			public string Name { get; internal set; }

			/// <summary>
			/// Gets the email of the author.
			/// </summary>
			public string Email { get; internal set; }
		}

		/// <summary>
		/// The different comment statuses.
		/// </summary>
		public enum CommentStatus
		{
			New = 0,
			Approved = 1,
			NotApproved = 2
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the comment id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The id of the entity this comment is attached to.
		/// </summary>
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets whether the comment is attached to a draft.
		/// </summary>
		public bool ParentIsDraft { get; set; }

		/// <summary>
		/// Gets/sets the current comment status.
		/// </summary>
		public CommentStatus Status {
			get { return (CommentStatus)InternalStatus; }
			set { InternalStatus = (int)value; }
		}

		/// <summary>
		/// Gets/sets the number of times this comment has been reported
		/// as abusive.
		/// </summary>
		public int ReportedCount { get; set; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the body of the content.
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Gets/sets the optional author name if this is an anonymous comment.
		/// </summary>
		public string AuthorName { get; set; }

		/// <summary>
		/// Gets/sets the optional author email if this is an anonymous comment.
		/// </summary>
		public string AuthorEmail { get; set; }

		/// <summary>
		/// Gets/sets the date the entity was first created.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the date the entity was last changed.
		/// </summary>
		public DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the optional id of the user who initially created the entity.
		/// </summary>
		public Guid? CreatedById { get; set; }

		/// <summary>
		/// Gets/sets the optional id of the user who last changed the entity.
		/// </summary>
		public Guid? UpdatedById { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the optional user who initially created the entity.
		/// </summary>
		public User CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the optional user who last changed the entity.
		/// </summary>
		public User UpdatedBy { get; set; }
		#endregion

		#region Internal properties
		/// <summary>
		/// Gets/sets the internal integer status value.
		/// </summary>
		public int InternalStatus { get; set; }
		#endregion

		#region Ignored properties
		/// <summary>
		/// Gets the comment author for display purposes.
		/// </summary>
		public CommentAuthor Author {
			get {
				if (CreatedById.HasValue) {
					if (CreatedBy == null)
						using (var db = new DataContext()) {
							CreatedBy = db.Users.Where(u => u.Id == CreatedById).SingleOrDefault();
						}
					return new CommentAuthor() {
						Name = CreatedBy.Firstname + " " + CreatedBy.Surname,
						Email = CreatedBy.Email
					};
				}
				return new CommentAuthor() {
					Name = AuthorName,
					Email = AuthorEmail
				};
			}
		}

		/// <summary>
		/// Gets the resource name for the current comment status.
		/// </summary>
		public string StatusName {
			get {
				return GetStatusName(Status);
			}
		}
		#endregion

		/// <summary>
		/// Attaches the entity to the given context.
		/// </summary>
		/// <param name="db">The db context</param>
		public void Attach(Piranha.DataContext db) {
			if (this.Id == Guid.Empty || db.Set<Comment>().Count(t => t.Id == this.Id) == 0)
				db.Entry(this).State = EntityState.Added;
			else db.Entry(this).State = EntityState.Modified;
		}

		/// <summary>
		/// Saves the current entity.
		/// </summary>
		/// <param name="db">The db context</param>
		/// <param name="state">The current entity state</param>
		public override void OnSave(DataContext db, EntityState state) {
			// We never connect comments to drafts.
			ParentIsDraft = false;

			if (state == EntityState.Added) {
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				Created = Updated = DateTime.Now;
				if (Application.Current.UserProvider.IsAuthenticated || db.Identity != Guid.Empty)
					CreatedById = UpdatedById = db.Identity != Guid.Empty ? db.Identity : Application.Current.UserProvider.UserId;
			} else if (state == EntityState.Modified) {
				Updated = DateTime.Now;
				if (Application.Current.UserProvider.IsAuthenticated || db.Identity != Guid.Empty)
					UpdatedById = db.Identity != Guid.Empty ? db.Identity : Application.Current.UserProvider.UserId;
			}
			base.OnSave(db, state);
		}

		/// <summary>
		/// Gets the name for the given comment status.
		/// </summary>
		/// <param name="status">The status</param>
		/// <returns>The name</returns>
		public static string GetStatusName(CommentStatus status) {
			if (status == CommentStatus.New)
				return Piranha.Resources.Comment.New;
			else if (status == CommentStatus.Approved)
				return Piranha.Resources.Comment.Approved;
			else return Piranha.Resources.Comment.NotApproved;
		}
	}
}
