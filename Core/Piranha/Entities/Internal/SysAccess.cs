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
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for the access control table.
	/// 
	/// Changes made to records of this type are logged.
	/// </summary>
	[PrimaryKey(Column = "sysaccess_id")]
	[Join(TableName = "sysgroup", ForeignKey = "sysaccess_group_id", PrimaryKey = "sysgroup_id")]
	[Serializable]
	public class SysAccess : PiranhaRecord<SysAccess>, ICacheRecord<SysAccess>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "sysaccess_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the group id.
		/// </summary>
		[Column(Name = "sysaccess_group_id")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "AccessGroup")]
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "AccessGroupRequired")]
		public Guid GroupId { get; set; }

		/// <summary>
		/// Gets the group name.
		/// </summary>
		[Column(Name = "sysgroup_name", ReadOnly = true, Table = "sysgroup")]
		public string GroupName { get; private set; }

		/// <summary>
		/// Gets/sets the function name.
		/// </summary>
		[Column(Name = "sysaccess_function")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "AccessName")]
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "AccessNameRequired")]
		[StringLength(64, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "AccessNameLength")]
		public string Function { get; set; }

		/// <summary>
		/// Gets/sets the description.
		/// </summary>
		[Column(Name = "sysaccess_description")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "AccessDescription")]
		[StringLength(64, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "AccessDescriptionLength")]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets whether the access rule is locked.
		/// </summary>
		[Column(Name = "sysaccess_locked")]
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "sysaccess_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name = "sysaccess_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the created by id.
		/// </summary>
		[Column(Name = "sysaccess_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		[Column(Name = "sysaccess_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the indexed access list for the applications
		/// </summary>
		/// <returns>The access list</returns>
		public static Dictionary<string, SysAccess> GetAccessList() {
			if (Application.Current.CacheProvider[typeof(SysAccess).Name] == null) {
				var sysAccesses = new Dictionary<string, SysAccess>();
				SysAccess.Get().ForEach((e) =>
					sysAccesses.Add(e.Function, e));
                Application.Current.CacheProvider[typeof(SysAccess).Name] = sysAccesses;
			}
			return (Dictionary<string, SysAccess>)Application.Current.CacheProvider[typeof(SysAccess).Name];
		}
		#endregion

		/// <summary>
		/// Default constructor. 
		/// </summary>
		public SysAccess()
			: base() {
			LogChanges = true;
		}

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the action was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (Function != null)
				Function = Function.ToUpper();
			return base.Save(tx);
		}

		/// <summary>
		/// Invalidates the current access cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(SysAccess record) {
			Application.Current.CacheProvider.Remove(typeof(SysAccess).Name);
		}
	}
}
