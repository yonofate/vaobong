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

namespace Piranha.Extend
{
	/// <summary>
	/// Base interface for defining page and post types.
	/// </summary>
	public interface IType
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the optional description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the html preview.
		/// </summary>
		string Preview { get; }

		/// <summary>
		/// Gets the controller/viewtemplate depending on the current
		/// application is using WebPages or Mvc.
		/// </summary>
		string Controller { get; }

		/// <summary>
		/// Gets if pages of the current type should be able to 
		/// override the controller.
		/// </summary>
		bool ShowController { get; }

		/// <summary>
		/// Gets the view. This is only relevant for Mvc applications.
		/// </summary>
		string View { get; }

		/// <summary>
		/// Gets if pages of the current type should be able to 
		/// override the controller.
		/// </summary>
		bool ShowView { get; }

		/// <summary>
		/// Gets the defíned properties.
		/// </summary>
		IList<string> Properties { get; }
	}
}
