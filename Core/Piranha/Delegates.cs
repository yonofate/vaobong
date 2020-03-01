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
using System.Web.Mvc;

using Piranha.WebPages;

namespace Piranha
{
	/// <summary>
	/// The different delegates used by the framework.
	/// </summary>
	public static class Delegates
	{
		public delegate void BreadcrumbStartHook(Web.UIHelper ui, StringBuilder str);
		public delegate void BreadcrumbEndHook(Web.UIHelper ui, StringBuilder str);
		public delegate void BreadcrumbItemHook(Web.UIHelper ui, StringBuilder str, Models.Sitemap page);

		public delegate void HeadHook(Web.UIHelper ui, StringBuilder str, Models.Page page, Models.Post post);

		public delegate void MenuItemHook(Web.UIHelper ui, StringBuilder str, Models.Sitemap page, bool active, bool activechild);
		public delegate void MenuItemLinkHook(Web.UIHelper ui, StringBuilder str, Models.Sitemap page);
		public delegate void MenuLevelHook(Web.UIHelper ui, StringBuilder str, string cssclass);

		public delegate void ModelLoadedHook<T>(T model);
		public delegate void ManagerModelHook<T>(Controller controller, WebPages.Manager.MenuItem menu, T model);
		public delegate void ManagerPublishableModelHook<T>(Controller controller, WebPages.Manager.MenuItem menu, T model, bool publish);

		public delegate void ManagerToolbarRender<T>(UrlHelper url, StringBuilder str, T model);
		public delegate void SendPassword(Entities.User user, string password);
	}
}
