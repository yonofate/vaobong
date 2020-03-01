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
	/// A category is used to classify content in the database.
	/// 
	/// Changes made to records of this type are logged.
	/// </summary>
	[PrimaryKey(Column = "category_id")]
	[Join(TableName = "permalink", ForeignKey = "category_permalink_id", PrimaryKey = "permalink_id")]
	[Serializable]
	public class Category : PiranhaRecord<Category>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "category_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the optional parent id.
		/// </summary>
		[Column(Name = "category_parent_id")]
		[Display(ResourceType = typeof(Piranha.Resources.Category), Name = "Parent")]
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the permalink id.
		/// </summary>
		[Column(Name = "category_permalink_id")]
		public Guid PermalinkId { get; set; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name = "category_name")]
		[Display(ResourceType = typeof(Piranha.Resources.Category), Name = "Name")]
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Category), ErrorMessageResourceName = "NameRequired")]
		[StringLength(64, ErrorMessageResourceType = typeof(Piranha.Resources.Category), ErrorMessageResourceName = "NameLength")]
		public string Name { get; set; }

		/// <summary>
		/// Gets the permalink.
		/// </summary>
		[Column(Name = "permalink_name", ReadOnly = true, Table = "permalink")]
		[Display(ResourceType = typeof(Piranha.Resources.Category), Name = "Permalink")]
		public string Permalink { get; private set; }

		/// <summary>
		/// Gets/sets the description.
		/// </summary>
		[Column(Name = "category_description")]
		[Display(ResourceType = typeof(Piranha.Resources.Category), Name = "Description")]
		[StringLength(255, ErrorMessageResourceType = typeof(Piranha.Resources.Category), ErrorMessageResourceName = "DescriptionLength")]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "category_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name = "category_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "category_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "category_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the child categories.
		/// </summary>
		public List<Category> Categories { get; set; }

		/// <summary>
		/// Gets/sets the level of the category.
		/// </summary>
		public int Level { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new category.
		/// </summary>
		public Category()
			: base() {
			ExtensionType = Extend.ExtensionType.Category;
			Categories = new List<Category>();
			LogChanges = true;
		}

		#region Static accessors
		/// <summary>
		/// Gets the categories sorted recursivly.
		/// </summary>
		/// <returns>The categories</returns>
		public static List<Category> GetStructure() {
			List<Category> cats = Category.Get(new Params() { OrderBy = "category_parent_id, category_name" });
			return Sort(cats, Guid.Empty);
		}

		/// <summary>
		/// Get the available categories for the given post
		/// </summary>
		/// <param name="id">The post id</param>
		/// <returns>The categories</returns>
		public static List<Category> GetByPostId(Guid id, bool draft = true) {
			return Category.Get("category_id IN (" +
				"SELECT relation_related_id FROM relation WHERE relation_type = @0 AND relation_data_id = @1 AND relation_draft = @2)",
				Relation.RelationType.POSTCATEGORY, id, draft);
		}

		/// <summary>
		/// Get the available categories for the given media content.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <returns>The categories</returns>
		public static List<Category> GetByContentId(Guid id, bool draft = true) {
			return Category.Get("category_id IN (" +
				"SELECT relation_related_id FROM relation WHERE relation_type = @0 AND relation_data_id = @1 AND relation_draft = @2)",
				Relation.RelationType.CONTENTCATEGORY, id, draft);
		}

		/// <summary>
		/// Gets the category for the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The category</returns>
		public static Category GetByPermalink(string permalink) {
			return Category.GetSingle("permalink_name = @0", permalink);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Sorts the categories
		/// </summary>
		/// <param name="categories">The categories to sort</param>
		/// <param name="parentid">Parent id</param>
		/// <returns>A list of categories</returns>
		private static List<Category> Sort(List<Category> categories, Guid parentid, int level = 1) {
			List<Category> ret = new List<Category>();

			foreach (Category cat in categories) {
				if (cat.ParentId == parentid) {
					cat.Categories = Sort(categories, cat.Id, level + 1);
					cat.Level = level;
					ret.Add(cat);
				}
			}
			return ret;
		}
		#endregion
	}


	/// <summary>
	/// Category extensions.
	/// </summary>
	public static class CategoryHelper
	{
		/// <summary>
		/// Flattens the structure into a list.
		/// </summary>
		/// <param name="groups">The structure to flatten</param>
		/// <returns>A list of groups</returns>
		public static List<Category> Flatten(this List<Category> categories) {
			List<Category> ret = new List<Category>();

			foreach (Category cat in categories) {
				ret.Add(cat);
				if (cat.Categories.Count > 0)
					ret.AddRange(Flatten(cat.Categories));
			}
			return ret;
		}
	}
}
