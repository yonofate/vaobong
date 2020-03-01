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
using System.Web;

namespace Piranha.Web.Handlers
{
	/// <summary>
	/// A request handler is a chunk of code that is inserted into BeginRequest,
	/// such as the default handler for permalinks.
	/// </summary>
	public interface IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		void HandleRequest(HttpContext context, params string[] args);
	}
}
