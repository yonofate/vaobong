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

namespace Piranha.Web.Handlers
{
	public class DraftThumbnailHandler : ThumbnailHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public override void HandleRequest(HttpContext context, params string[] args) {
			//if (context.User != null && context.User.HasAccess("ADMIN"))
			HandleRequest(context, true, args);
			//else HandleRequest(context, false, args) ;
		}
	}
}