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

namespace Piranha.Web.Handlers
{
	public class RequestHandlerRegistration
	{
		/// <summary>
		/// Gets/sets the url prefix.
		/// </summary>
		public string UrlPrefix { get; set; }

		/// <summary>
		/// Gets/sets the handler id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets/sets the handler.
		/// </summary>
		public IRequestHandler Handler { get; set; }
	}
}
