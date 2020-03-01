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
using System.Web;

using Piranha.Web;

namespace Piranha.WebPages
{
	public class BaseHelper : System.Web.WebPages.HelperPage
	{
		#region Members
		private static Web.SiteHelper siteHelper = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the helper for the piranha methods.
		/// </summary>
		public static UIHelper UI {
			get {
				return new UIHelper();
			}
		}

		/// <summary>
		/// Gets the helper for the piranha site.
		/// </summary>
		public static Web.SiteHelper Site {
			get {
				if (siteHelper == null)
					siteHelper = new Web.SiteHelper();
				return siteHelper;
			}
		}
		#endregion
	}
}