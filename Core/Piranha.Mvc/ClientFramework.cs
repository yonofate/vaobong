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
using System.ComponentModel.Composition;

using Piranha.Web;

namespace Piranha.Mvc
{
	/// <summary>
	/// Client framework class for ASP.NET Web Pages.
	/// </summary>
	[Export(typeof(IClientFramework))]
	public class ClientFramework : IClientFramework
	{
		/// <summary>
		/// Gets the current framework type.
		/// </summary>
		public FrameworkType Type {
			get { return FrameworkType.Mvc; }
		}
	}
}
