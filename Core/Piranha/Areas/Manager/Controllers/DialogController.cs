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
using System.Web.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Controller for handling the different "popup" dialogs.
	/// </summary>
	public class DialogController : Controller
	{
		/// <summary>
		/// Gets the link dialog for the given site id.
		/// </summary>
		/// <param name="id">The site id</param>
		[Access(Function = "ADMIN")]
		public ActionResult Link(string id) {
			return View(Models.LinkDialogModel.GetBySiteId(new Guid(id)));
		}
	}
}
