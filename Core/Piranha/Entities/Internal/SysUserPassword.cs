﻿/*
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// The SysUserPassword record is used for changing a users password in the database.
	/// </summary>
	[Table(Name = "sysuser"), PrimaryKey(Column = "sysuser_id")]
	public class SysUserPassword : GuidRecord<SysUserPassword>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "sysuser_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the password.
		/// </summary>
		[Column(Name = "sysuser_password", OnLoad = "OnPasswordLoad", OnSave = "OnPasswordSave")]
		[Display(ResourceType = typeof(Piranha.Resources.Global), Name = "Password")]
		public string Password { get; set; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the confirmation password.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Global), Name = "Confirm")]
		[Compare("Password",
			ErrorMessageResourceType = typeof(Piranha.Resources.Settings),
			ErrorMessageResourceName = "PasswordConfirmError")]
		public string PasswordConfirm { get; set; }

		/// <summary>
		/// Checks if the password is set and can be saved.
		/// </summary>
		public bool IsSet { get { return !String.IsNullOrEmpty(Password); } }
		#endregion

		/// <summary>
		/// Generates a simple 8 character random password.
		/// </summary>
		/// <returns>The password</returns>
		public static string GeneratePassword() {
			Random rnd = new Random();
			string sc = "!#%&/()=@$";

			// Generate base password
			string pw = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);

			// Insert two random special characters somewhere
			pw = pw.Insert(rnd.Next() % pw.Length, sc.Substring(rnd.Next() % sc.Length, 1));
			pw = pw.Insert(rnd.Next() % pw.Length, sc.Substring(rnd.Next() % sc.Length, 1));

			return pw;
		}

		/// <summary>
		/// Saves the record to the database. Checks so empty passwords don't get saved.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the operation succeeded or not</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (IsSet)
				return base.Save(tx);
			else return false;
		}
	}
}
