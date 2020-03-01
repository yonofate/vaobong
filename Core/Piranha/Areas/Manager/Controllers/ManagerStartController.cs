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
	/// Controller that simply redirects to the startpage after authentication 
	/// tokens have been set correctly.
	/// </summary>
	public class ManagerStartController : ManagerController
	{
		/// <summary>
		/// Redirects to the current startpage for the user.
		/// </summary>
		[Access(Function = "ADMIN")]
		public ActionResult Index() {
			// Redirect after logon
			var startpage = WebPages.Manager.GetStartpage();
			return RedirectToAction(startpage.Action, startpage.Controller);
		}
	}
}
