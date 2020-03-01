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
	/// The sys group is a member classification for users. Groups are hierarchical
	/// and inherit permissions from child groups.
	/// 
	/// Changes made to records of this type are logged.
	/// </summary>
	[PrimaryKey(Column = "sysgroup_id")]
	[Serializable]
	public class SysGroup : PiranhaRecord<SysGroup>, ICacheRecord<SysGroup>
	{
		#region Fields
		[Column(Name = "sysgroup_id")]
		public override Guid Id { get; set; }

		[Column(Name = "sysgroup_parent_id")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "ParentGroup")]
		public Guid ParentId { get; set; }

		[Column(Name = "sysgroup_name")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "GroupName")]
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "GroupNameRequired")]
		[StringLength(64, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "GroupNameLength")]
		public string Name { get; set; }

		[Column(Name = "sysgroup_description")]
		[Display(ResourceType = typeof(Piranha.Resources.Settings), Name = "GroupDescription")]
		[StringLength(255, ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "GroupDescriptionLength")]
		public string Description { get; set; }

		[Column(Name = "sysgroup_created")]
		public override DateTime Created { get; set; }

		[Column(Name = "sysgroup_updated")]
		public override DateTime Updated { get; set; }

		[Column(Name = "sysgroup_created_by")]
		public override Guid CreatedBy { get; set; }

		[Column(Name = "sysgroup_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the child groups available.
		/// </summary>
		public List<SysGroup> Groups { get; set; }

		/// <summary>
		/// Gets/sets the level of the current record.
		/// </summary>
		public int Level { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SysGroup() {
			Groups = new List<SysGroup>();
			LogChanges = true;
		}

		/// <summary>
		/// Gets the groups sorted recursivly. The result of this method is chaced
		/// for performance.
		/// </summary>
		/// <returns>The groups</returns>
		public static List<SysGroup> GetStructure() {
			if (Application.Current.CacheProvider[typeof(SysGroup).Name] == null)
				Application.Current.CacheProvider[typeof(SysGroup).Name] =
					Sort(SysGroup.Get(new Params() { OrderBy = "sysgroup_parent_id" }), Guid.Empty);
			return (List<SysGroup>)Application.Current.CacheProvider[typeof(SysGroup).Name];
		}

		/// <summary>
		/// Gets the parent groups for the given group id.
		/// </summary>
		/// <param name="id">The group id</param>
		/// <returns>The parent groups</returns>
		public static List<SysGroup> GetParents(Guid id) {
			var groups = GetStructure().Flatten();

			var tmp = new List<SysGroup>();
			if (id != Guid.Empty) {
				groups.ForEach(g => {
					if (g.HasChild(id))
						tmp.Add(g);
				});
			} else tmp = groups;
			return tmp;
		}

		/// <summary>
		/// Checks if the current group has a child group with the given id.
		/// </summary>
		/// <param name="childid">The child id</param>
		/// <returns>Wether the id was a child group</returns>
		public bool HasChild(Guid childid) {
			if (Groups != null) {
				foreach (SysGroup group in Groups) {
					if (group.Id == childid || group.HasChild(childid))
						return true;
				}
			}
			return false;
		}

		#region Private methods
		/// <summary>
		/// Sorts the groups
		/// </summary>
		/// <param name="groups">The groups to sort</param>
		/// <param name="parentid">Parent id</param>
		/// <returns>A list of groups</returns>
		private static List<SysGroup> Sort(List<SysGroup> groups, Guid parentid, int level = 1) {
			List<SysGroup> ret = new List<SysGroup>();

			foreach (SysGroup group in groups) {
				if (group.ParentId == parentid) {
					group.Groups = Sort(groups, group.Id, level + 1);
					group.Level = level;
					ret.Add(group);
				}
			}
			return ret;
		}
		#endregion

		/// <summary>
		/// Invalidates the given record from the cache.
		/// </summary>
		/// <param name="record">The record to invalidate.</param>
		public void InvalidateRecord(SysGroup record) {
			// Invalidate entire cache as groups are recursivly linked
			Application.Current.CacheProvider.Remove(typeof(SysGroup).Name);
		}
	}

	/// <summary>
	/// Extensions
	/// </summary>
	public static class SysGroupHelper
	{
		/// <summary>
		/// Flattens the structure into a list.
		/// </summary>
		/// <param name="groups">The structure to flatten</param>
		/// <returns>A list of groups</returns>
		public static List<SysGroup> Flatten(this List<SysGroup> groups) {
			List<SysGroup> ret = new List<SysGroup>();

			foreach (SysGroup group in groups) {
				ret.Add(group);
				if (group.Groups.Count > 0)
					ret.AddRange(Flatten(group.Groups));
			}
			return ret;
		}

		/// <summary>
		/// Gets the group with the given id from the list.
		/// </summary>
		/// <param name="groups">The group list</param>
		/// <param name="id">The id</param>
		/// <returns>The group</returns>
		public static SysGroup GetGroupById(this List<SysGroup> groups, Guid id) {
			foreach (SysGroup group in groups) {
				SysGroup g = group.Id == id ? group : group.Groups.GetGroupById(id);
				if (g != null)
					return g;
			}
			return null;
		}
	}
}
