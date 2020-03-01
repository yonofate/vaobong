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
using System.Security.Cryptography;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Base class for all records that uses a guid as their primary key column.
	/// </summary>
	/// <typeparam name="T">The type of the record</typeparam>
	[Serializable]
	public abstract class GuidRecord<T> : ActiveRecord<T>
	{
		#region Members
		/// <summary>
		/// Gets/sets whether to log changes made to this entity.
		/// </summary>
		public bool LogChanges = false;

		/// <summary>
		/// Gets/sets the records extension type.
		/// </summary>
		protected Extend.ExtensionType ExtensionType = Extend.ExtensionType.NotSet;
		#endregion

		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		public abstract Guid Id { get; set; }
		#endregion

		#region Handlers
		/// <summary>
		/// NEVER load passwords from the database ever.
		/// </summary>
		/// <param name="pwd">The password</param>
		/// <returns>An empty string</returns>
		protected string OnPasswordLoad(string pwd) {
			return "";
		}

		/// <summary>
		/// Encrypts the password before saving it to the database.
		/// </summary>
		/// <param name="pwd">The password</param>
		/// <returns>The encrypted password</returns>
		protected string OnPasswordSave(string pwd) {
			return Encrypt(pwd);
		}
		#endregion

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			var isnew = IsNew;

			if (IsNew && Id == Guid.Empty)
				Id = Guid.NewGuid();
			var success = base.Save(tx);

			// If the action was successful, insert a log entry.
			if (HttpContext.Current != null && Application.Current.UserProvider.IsAuthenticated && LogChanges && success) {
				// Check that we have a valid user id
				bool validUser = false;
				try {
					var userId = Application.Current.UserProvider.UserId;
					var user = SysUser.GetSingle(userId, tx);
					if (user != null)
						validUser = true;
				} catch { }

				// Only log if we have a valid logged in user.
				if (validUser) {
					bool draft = true;
					if (this is DraftRecord<T>)
						draft = ((DraftRecord<T>)this).IsDraft;

					var log = new SysLog() {
						ParentId = Id,
						ParentType = GetRecordName(),
						Action = !draft ? "PUBLISH" : (isnew ? "INSERT" : "UPDATE")
					};
					log.Save(tx);
				}
			}
			return success;
		}

		/// <summary>
		/// Deletes the current records.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Delete(System.Data.IDbTransaction tx = null) {
			var success = base.Delete(tx);

			// Delete extensions if we have any
			if (success && ExtensionType != Extend.ExtensionType.NotSet) {
				bool draft = false;
				PropertyInfo prop = this.GetType().GetProperty("IsDraft");
				if (prop != null)
					draft = (bool)prop.GetValue(this, null);
				Extension.Execute("DELETE FROM extension WHERE extension_parent_id=@0 AND extension_draft=@1", tx, Id, draft);
			}

			// If the action was successful, insert a log entry.
			if (HttpContext.Current != null && Application.Current.UserProvider.IsAuthenticated && LogChanges && success) {
				bool draft = true;
				if (this is DraftRecord<T>)
					draft = ((DraftRecord<T>)this).IsDraft;

				var log = new SysLog() {
					ParentId = Id,
					ParentType = GetRecordName(),
					Action = !draft ? "DEPUBLISH" : "DELETE"
				};
				log.Save(tx);
			}
			return success;
		}

		/// <summary>
		/// Gets the extensions attached to the current record.
		/// </summary>
		/// <param name="manager">Whether or not this is loaded by the manager</param>
		/// <returns>The extensions</returns>
		public virtual List<Extension> GetExtensions(bool manager = false) {
			if (ExtensionType != Extend.ExtensionType.NotSet) {
				PropertyInfo prop = this.GetType().GetProperty("IsDraft");
				bool draft = prop != null ? (bool)prop.GetValue(this, null) : false;
				List<Extension> ret = null;

				if (Id != Guid.Empty) {
					ret = Extend.ExtensionManager.Current.GetByTypeAndEntity(ExtensionType, Id, draft);
				} else {
					ret = Extend.ExtensionManager.Current.GetByType(ExtensionType, draft);
				}

				foreach (var ext in ret)
					if (Extend.ExtensionManager.Current.HasType(ext.Type)) {
						if (!manager)
							ext.Body.Init(this);
						else ext.Body.InitManager(this);
					}
				return ret;
			}
			return new List<Extension>();
		}

		/// <summary>
		/// Encrypts the given string with MD5.
		/// </summary>
		/// <param name="str">The encrypted string</param>
		/// <returns></returns>
		public static string Encrypt(string str) {
			UTF8Encoding encoder = new UTF8Encoding();
			SHA256CryptoServiceProvider crypto = new SHA256CryptoServiceProvider();

			byte[] bytes = crypto.ComputeHash(encoder.GetBytes(str));
			return Convert.ToBase64String(bytes);
		}

		#region Private methods
		/// <summary>
		/// This is bad design. Return the table name of the current record IF IT
		/// DOESN'T turn out to be media.
		/// </summary>
		/// <returns>The name</returns>
		private string GetRecordName() {
			if (this is Content && ((Content)(object)this).IsFolder)
				return "MEDIAFOLDER";
			else return ((string)typeof(T).GetProperty("TableName",
				BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetValue(this, null)).ToUpper();
		}
		#endregion
	}
}
