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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for the system parameters.
	/// 
	/// Changes made to records of this type are logged.
	/// </summary>
	[PrimaryKey(Column = "sysparam_id")]
	[Serializable]
	public class SysParam : PiranhaRecord<SysParam>, ICacheRecord<SysParam>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "sysparam_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the param name.
		/// </summary>
		[Column(Name = "sysparam_name")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "ParamName")]
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "ParamNameRequired")]
		[StringLength(64, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "ParamNameLength")]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the param value.
		/// </summary>
		[Column(Name = "sysparam_value")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "ParamValue")]
		[StringLength(128, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "ParamValueLength")]
		public string Value { get; set; }

		/// <summary>
		/// Gets/sets the param description.
		/// </summary>
		[Column(Name = "sysparam_description")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "ParamDescription")]
		[StringLength(255, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "ParamDescriptionLength")]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets whether the param is locked or not. This field can not be set through
		/// the admin interface.
		/// </summary>
		[Column(Name = "sysparam_locked")]
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "sysparam_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the update date.
		/// </summary>
		[Column(Name = "sysparam_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the created by id
		/// </summary>
		[Column(Name = "sysparam_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		[Column(Name = "sysparam_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Members
		private static Cache.MemCacheProvider Cache = new Cache.MemCacheProvider();
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the param with the given name.
		/// </summary>
		/// <param name="name">The param name</param>
		/// <returns>The param</returns>
		public static SysParam GetByName(string name) {
			try {
				if (!Cache.Contains(name.ToUpper()))
					Cache[name.ToUpper()] = SysParam.GetSingle("sysparam_name = @0", name);
				return (SysParam)Cache[name.ToUpper()];
			} catch {
				if (Config.ShowDBErrors)
					throw;
			}
			return null;
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SysParam()
			: base() {
			LogChanges = true;
		}

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the action was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (Name != null)
				Name = Name.ToUpper();
			return base.Save(tx);
		}

		/// <summary>
		/// Invalidates the current record from the cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(SysParam record) {
			Cache.Remove(record.Name.ToUpper());

			//if (Cache.ContainsKey(record.Name.ToUpper()))
			//	Cache.Remove(record.Name.ToUpper()) ;
		}

		/// <summary>
		/// Invalidates the sysparam with the given name.
		/// </summary>
		/// <param name="name">The param name.</param>
		public static void InvalidateParam(string name) {
			Cache.Remove(name.ToUpper());

			//if (Cache.ContainsKey(name.ToUpper()))
			//	Cache.Remove(name.ToUpper()) ;
		}
	}
}
