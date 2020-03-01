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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Piranha.Web
{
	/// <summary>
	/// Culture helper class.
	/// </summary>
	public class CultureHelper
	{
		/// <summary>
		/// Gets the name of the current UI culture.
		/// </summary>
		/// <returns>The current UI culture</returns>
		public string CurrentUICulture {
			get {
				return CultureInfo.CurrentUICulture.Name;
			}
		}

		/// <summary>
		/// Gets the default ui culture as specified in the current web.config
		/// </summary>
		public string DefaultUICulture {
			get {
				GlobalizationSection gs = (GlobalizationSection)WebConfigurationManager.GetSection("system.web/globalization");
				return gs.UICulture;
			}
		}

		/// <summary>
		/// Gets whether the current ui culture is the default culture.
		/// </summary>
		public bool IsDefaultCulture {
			get {
				return CurrentUICulture == DefaultUICulture;
			}
		}
	}
}
