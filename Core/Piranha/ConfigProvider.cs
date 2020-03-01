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

namespace Piranha
{
	/// <summary>
	/// Simple class for defining a configured provider
	/// </summary>
	internal sealed class ConfigProvider
	{
		/// <summary>
		/// Get/sets the full assembly name.
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		/// Gets/sets the full type name.
		/// </summary>
		public string TypeName { get; set; }
	}
}