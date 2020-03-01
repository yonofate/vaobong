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

namespace Piranha.Web
{
	/// <summary>
	/// Interface defining the currently active client framework. This is imported
	/// by the Piranha.Web.Application class.
	/// </summary>
	public interface IClientFramework
	{
		/// <summary>
		/// Gets the currently active framework.
		/// </summary>
		FrameworkType Type { get; }
	}
}
