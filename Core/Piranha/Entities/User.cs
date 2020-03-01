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
using System.Web;

namespace Piranha.Entities
{
	/// <summary>
	/// The core user entity.
	/// </summary>
	[Serializable]
	public class User : BaseEntity
	{
		#region Properties
		/// <summary>
		/// Gets/sets the unique user id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the id of the group to which the user belong.
		/// </summary>
		public Guid? GroupId { get; set; }

		/// <summary>
		/// Gets/sets the users API-key for ReST services.
		/// </summary>
		public Guid? APIKey { get; set; }

		/// <summary>
		/// Gets/sets the unique login name.
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Gets/sets the SHA2 encrypted password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets/sets the firstname of the user.
		/// </summary>
		public string Firstname { get; set; }

		/// <summary>
		/// Gets/sets the surname of the user.
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// Gets/sets the surname of the user.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Gets/sets the users prefered culture.
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		/// Gets/sets the last time the user logged in to the system.
		/// </summary>
		public DateTime? LastLogin { get; set; }

		/// <summary>
		/// Gets/sets the previous login for the user.
		/// </summary>
		public DateTime? PreviousLogin { get; set; }

		/// <summary>
		/// Gets/sets whether the user account is locked or not.
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets/sets the optional end date of user lock.
		/// </summary>
		public DateTime? LockedUntil { get; set; }

		/// <summary>
		/// Gets/sets the date the user was initially created.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the date the user was last updated.
		/// </summary>
		public DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the id of the user who initially created the user.
		/// </summary>
		public Guid? CreatedById { get; set; }

		/// <summary>
		/// Gets/sets the id of the user who last updated the user.
		/// </summary>
		public Guid? UpdatedById { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the group to which the user belong.
		/// </summary>
		public Group Group { get; set; }

		/// <summary>
		/// Gets/sets the currently available extensions.
		/// </summary>
		public IList<Extension> Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public User() {
			Extensions = new List<Extension>();
		}

		/// <summary>
		/// Generates a new password for the current user.
		/// </summary>
		public void GeneratePassword() {
			Password = Models.SysUserPassword.Encrypt(CreatePassword());
		}

		/// <summary>
		/// Generates a new password for the current user and sends it to the user via email.
		/// </summary>
		/// <param name="context">Optional data context</param>
		public void GenerateAndSendPassword(DataContext context = null) {
			if (WebPages.Hooks.Mail.SendPassword != null) {
				var password = CreatePassword();

				Password = Models.SysUserPassword.Encrypt(password);

				using (var db = (context == null ? new DataContext() : null)) {
					var dbContext = context == null ? db : context;

					if (dbContext.Users.Where(u => u.Id == Id).Count() == 0)
						Attach(dbContext, EntityState.Added);
					else Attach(dbContext, EntityState.Modified);

					if (dbContext.SaveChanges() > 0)
						WebPages.Hooks.Mail.SendPassword(this, password);
				}
			} else {
				throw new Exception("You need to attach a hander for Piranha.WebPages.Hooks.Mail.SendPassword in order to send a new password.");
			}
		}

		/// <summary>
		/// Saves the current user.
		/// </summary>
		/// <param name="db">The db context</param>
		/// <param name="state">The entity state</param>
		public override void OnSave(DataContext db, EntityState state) {
			if (db.Identity != Guid.Empty || Application.Current.UserProvider.IsAuthenticated) {
				if (state == EntityState.Added) {
					if (Id == Guid.Empty)
						Id = Guid.NewGuid();
					Created = Updated = DateTime.Now;
					CreatedById = UpdatedById = db.Identity != Guid.Empty ? db.Identity : Application.Current.UserProvider.UserId;
				} else if (state == EntityState.Modified) {
					Updated = DateTime.Now;
					UpdatedById = db.Identity != Guid.Empty ? db.Identity : Application.Current.UserProvider.UserId;
				}
			} else throw new UnauthorizedAccessException("User must be logged in to save entity");
		}

		#region Private methods
		/// <summary>
		/// Generates a new unencrypted password that can be sent to the user.
		/// </summary>
		/// <returns>The password.</returns>
		private string CreatePassword() {
			Random rnd = new Random();
			string sc = "!#%&/()=@$";

			// Generate base password
			string pw = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);

			// Insert two random special characters somewhere
			pw = pw.Insert(rnd.Next() % pw.Length, sc.Substring(rnd.Next() % sc.Length, 1));
			pw = pw.Insert(rnd.Next() % pw.Length, sc.Substring(rnd.Next() % sc.Length, 1));

			return pw;
		}
		#endregion
	}
}
