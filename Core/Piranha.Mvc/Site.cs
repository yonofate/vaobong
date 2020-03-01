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

using Piranha.Web;

namespace Piranha.Mvc
{
	/// <summary>
	/// This is the static mvc wrapper for Piranha.Web.SiteHelper
	/// </summary>
	public static class Site
	{
		#region Members
		private static SiteHelper Helper = new SiteHelper();
		#endregion

		/// <summary>
		/// Gets the site title.
		/// </summary>
		public static string SiteTitle {
			get { return Helper.SiteTitle; }
		}

		/// <summary>
		/// Gets the site description.
		/// </summary>
		public static string SiteDescription {
			get { return Helper.SiteDescription; }
		}

	}
}
