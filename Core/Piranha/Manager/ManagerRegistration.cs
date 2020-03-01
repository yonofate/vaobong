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
using System.Web.Mvc;

namespace Piranha.Manager
{
	/// <summary>
	/// The area registration for the manager module.
	/// </summary>
	internal class ManagerRegistration : AreaRegistration
	{
		#region Members
		/// <summary>
		/// The manager namespace. Additional namespaces can be configured with Config.ManagerNamespaces
		/// </summary>
		private string[] namespaces = { "Piranha.Areas.Manager.Controllers" };
		#endregion

		/// <summary>
		/// Gets the area name.
		/// </summary>
		public override string AreaName {
			get { return "Manager"; }
		}

		/// <summary>
		/// Registers the manager area.
		/// </summary>
		/// <param name="context">The context</param>
		public override void RegisterArea(AreaRegistrationContext context) {
			if (!Config.DisableManager) {
				// Register manager routing
				context.MapRoute(
					"Manager",
					"manager/{controller}/{action}/{id}",
					new { area = "manager", controller = "account", action = "index", id = UrlParameter.Optional },
					namespaces.Union(Config.ManagerNamespaces).ToArray()
				).DataTokens["UseNamespaceFallback"] = false;

				// Register filters & binders
				RegisterGlobalFilters(GlobalFilters.Filters);
			}
		}

		#region Private methods
		/// <summary>
		/// Registers all global filters.
		/// </summary>
		/// <param name="filters">The current filter collection</param>
		private void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}
		#endregion
	}
}