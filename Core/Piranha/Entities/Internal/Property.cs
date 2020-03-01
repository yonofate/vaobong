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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page region.
	/// </summary>
	[PrimaryKey(Column = "property_id,property_draft")]
	[Serializable]
	public class Property : PiranhaRecord<Property>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "property_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets whether this is a draft or not.
		/// </summary>
		[Column(Name = "property_draft")]
		public bool IsDraft { get; set; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		[Column(Name = "property_parent_id")]
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name = "property_name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the body content.
		/// </summary>
		[Column(Name = "property_value")]
		public string Value { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "property_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name = "property_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "property_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "property_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		/// <summary>
		/// Gets all properties associated with the given parent id of the given state.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <param name="draft">Whether to get the draft version or not</param>
		/// <returns>The properties</returns>
		public static List<Property> GetByParentId(Guid id, bool draft = false) {
			return Get("property_parent_id = @0 AND property_draft = @1", id, draft);
		}

		/// <summary>
		/// Gets all properties associated with the given parent id regardless of state.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <returns>The properties</returns>
		internal static List<Property> GetAllByParentId(Guid id) {
			return Get("property_parent_id = @0", id);
		}

		/// <summary>
		/// Gets the name & value for all properties associated with the given parent 
		/// id of the given state.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <param name="draft">Whether to get the draft version or not</param>
		/// <returns>The properties</returns>
		public static List<Property> GetContentByParentId(Guid id, bool draft = false) {
			return GetFields("property_name, property_value", "property_parent_id = @0 AND property_draft = @1", id, draft);
		}
	}
}
