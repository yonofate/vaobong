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
using System.Threading.Tasks;

/// <summary>
/// Core extensions for the MVC library.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Gets the view for model.
	/// </summary>
	/// <param name="m">The page model</param>
	/// <returns>The view</returns>
	public static string GetView(this Piranha.Models.PageModel m) {
		return ((Piranha.Models.Page)m.Page).View;
	}

	/// <summary>
	/// Gets the view for model.
	/// </summary>
	/// <param name="m">The post model</param>
	/// <returns>The view</returns>
	public static string GetView(this Piranha.Models.PostModel m) {

        if (m == null || m.Post == null) return string.Empty;

		return ((Piranha.Models.Post)m.Post).View;
	}
}

