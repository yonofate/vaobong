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

using Piranha.Data;

namespace Piranha.Models.Manager.CategoryModels
{
	/// <summary>
	/// Category list model for the manager area.
	/// </summary>
	public class ListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the category list.
		/// </summary>
		public List<Category> Categories { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public ListModel() {
			Categories = new List<Category>();
		}

		/// <summary>
		/// Gets the list model for the categories.
		/// </summary>
		public static ListModel Get() {
			ListModel m = new ListModel();
			m.Categories = Category.GetStructure().Flatten();

			return m;
		}
	}
}
