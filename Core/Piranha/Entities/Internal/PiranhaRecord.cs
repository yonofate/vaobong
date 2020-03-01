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
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Base class for all active records in the Piranha framework.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	[Serializable]
	public abstract class PiranhaRecord<T> : GuidRecord<T>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public abstract DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		public abstract DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		public abstract Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		public abstract Guid UpdatedBy { get; set; }
		#endregion

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			return Save(tx, true);
		}

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <param name="setdates">Whether to automatically set the dates</param>
		/// <returns>Wether the operation was successful</returns>
		protected bool Save(System.Data.IDbTransaction tx = null, bool setdates = true) {
			if (Database.Identity != Guid.Empty || Application.Current.UserProvider.IsAuthenticated) {
				if (IsNew) {
					if (setdates)
						Created = DateTime.Now;
					CreatedBy = Database.Identity != Guid.Empty ? Database.Identity : Application.Current.UserProvider.UserId;
				}
				if (setdates)
					Updated = DateTime.Now;
				UpdatedBy = Database.Identity != Guid.Empty ? Database.Identity : Application.Current.UserProvider.UserId;

				return base.Save(tx);
			}
			throw new AccessViolationException("User must be logged in to save data.");
		}
	}
}
