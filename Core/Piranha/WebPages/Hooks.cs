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
using System.Web.Mvc;

namespace Piranha.WebPages
{
	/// <summary>
	/// The different hook in the core that the application can attach
	/// itself to.
	/// </summary>
	public static class Hooks
	{
		/// <summary>
		/// The different hooks available for the breadcrumb.
		/// </summary>
		public static class Breadcrumb
		{
			/// <summary>
			/// Renders the start of the breadcrumb.
			/// </summary>
			public static Delegates.BreadcrumbStartHook RenderStart;

			/// <summary>
			/// Renders the end of the breadcrumb.
			/// </summary>
			public static Delegates.BreadcrumbEndHook RenderEnd;

			/// <summary>
			/// Renders a breadcrumb item.
			/// </summary>
			public static Delegates.BreadcrumbItemHook RenderItem;

			/// <summary>
			/// Renders the currently active breadcrumb item.
			/// </summary>
			public static Delegates.BreadcrumbItemHook RenderActiveItem;
		}

		/// <summary>
		/// The different hooks available for the head.
		/// </summary>
		public static class Head
		{
			/// <summary>
			/// Renders optional information in the head.
			/// </summary>
			public static Delegates.HeadHook Render;
		}

		/// <summary>
		/// The different hooks availble for the menu.
		/// </summary>
		public static class Menu
		{
			/// <summary>
			/// Renders the start of a menu level.
			/// </summary>
			public static Delegates.MenuLevelHook RenderLevelStart;

			/// <summary>
			/// Renders the end of a menu level.
			/// </summary>
			public static Delegates.MenuLevelHook RenderLevelEnd;

			/// <summary>
			/// Renders the start of a menu item.
			/// </summary>
			public static Delegates.MenuItemHook RenderItemStart;

			/// <summary>
			/// Renders the end of a menu item.
			/// </summary>
			public static Delegates.MenuItemHook RenderItemEnd;

			/// <summary>
			/// Renders the menu item link.
			/// </summary>
			public static Delegates.MenuItemLinkHook RenderItemLink;
		}

		/// <summary>
		/// The different model hooks available for the application.
		/// </summary>
		public static class Model
		{
			/// <summary>
			/// Executed after the page model is loaded.
			/// </summary>
			public static Delegates.ModelLoadedHook<Models.PageModel> PageModelLoaded;

			/// <summary>
			/// Executed after the page model is loaded.
			/// </summary>
			public static Delegates.ModelLoadedHook<Models.PostModel> PostModelLoaded;
		}

		/// <summary>
		/// The different hooks available for the manager area.
		/// </summary>
		public static class Manager
		{
			#region Post Hooks
			/// <summary>
			/// Executed after the post list model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Areas.Manager.Models.PostListModel> PostListModelLoaded;

			/// <summary>
			/// Executed after the post edit model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.PostModels.EditModel> PostEditModelLoaded;

			/// <summary>
			/// Executed before the page edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.PostModels.EditModel> PostEditModelBeforeSave;

			/// <summary>
			/// Executed after the page edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.PostModels.EditModel> PostEditModelAfterSave;
			#endregion

			#region Page Hooks
			/// <summary>
			/// Executed after the page list model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.PageModels.ListModel> PageListModelLoaded;

			/// <summary>
			/// Executed after the page edit model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.PageModels.EditModel> PageEditModelLoaded;

			/// <summary>
			/// Executed before the page edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.PageModels.EditModel> PageEditModelBeforeSave;

			/// <summary>
			/// Executed after the page edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.PageModels.EditModel> PageEditModelAfterSave;
			#endregion

			#region Media Hooks
			/// <summary>
			/// Executed after the media list model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.ContentModels.ListModel> MediaListModelLoaded;

			/// <summary>
			/// Executed after the media edit model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.ContentModels.EditModel> MediaEditModelLoaded;

			/// <summary>
			/// Executed before the media edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.ContentModels.EditModel> MediaEditModelBeforeSave;

			/// <summary>
			/// Executed after the media edit model is saved.
			/// </summary>
			public static Delegates.ManagerPublishableModelHook<Models.Manager.ContentModels.EditModel> MediaEditModelAfterSave;
			#endregion

			#region Category Hooks
			/// <summary>
			/// Executed after the category list model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.CategoryModels.ListModel> CategoryListModelLoaded;

			/// <summary>
			/// Executed after the catgegory edit model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.CategoryModels.EditModel> CategoryEditModelLoaded;

			/// <summary>
			/// Executed before the category edit model is saved.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.CategoryModels.EditModel> CategoryEditModelBeforeSave;

			/// <summary>
			/// Executed after the category edit model is saved.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.CategoryModels.EditModel> CategoryEditModelAfterSave;
			#endregion

			#region User Hooks
			/// <summary>
			/// Executed after the user list model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.SettingModels.UserListModel> UserListModelLoaded;

			/// <summary>
			/// Executed after the user edit model is loaded but before the view is called.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.SettingModels.UserEditModel> UserEditModelLoaded;

			/// <summary>
			/// Executed before the user edit model is saved.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.SettingModels.UserEditModel> UserEditModelBeforeSave;

			/// <summary>
			/// Executed after the user edit model is saved.
			/// </summary>
			public static Delegates.ManagerModelHook<Models.Manager.SettingModels.UserEditModel> UserEditModelAfterSave;
			#endregion

			/// <summary>
			/// The different hooks available for the toolbar.
			/// </summary>
			public static class Toolbar
			{
				/// <summary>
				/// Executed when the manager toolbar for the page list is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Models.Manager.PageModels.ListModel> PageListToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the page edit view is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Models.Manager.PageModels.EditModel> PageEditToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the post list is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Areas.Manager.Models.PostListModel> PostListToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the post edit view is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Models.Manager.PostModels.EditModel> PostEditToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the media list is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Models.Manager.ContentModels.ListModel> MediaListToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the media edit view is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Models.Manager.ContentModels.EditModel> MediaEditToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the site list is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Areas.Manager.Models.SiteTreeListModel> SiteListToolbarRender;

				/// <summary>
				/// Executed when the manager toolbar for the site edit view is rendered.
				/// </summary>
				public static Delegates.ManagerToolbarRender<Areas.Manager.Models.SiteTreeEditModel> SiteEditToolbarRender;

				/// <summary>
				/// Renders the toolbar.
				/// </summary>
				/// <param name="url">The url</param>
				/// <param name="model">The current model</param>
				/// <returns>The rendered html</returns>
				public static HtmlString Render(UrlHelper url, object model) {
					StringBuilder str = new StringBuilder();

					if (model is Models.Manager.PageModels.ListModel && PageListToolbarRender != null)
						PageListToolbarRender(url, str, (Models.Manager.PageModels.ListModel)model);
					else if (model is Models.Manager.PageModels.EditModel && PageEditToolbarRender != null)
						PageEditToolbarRender(url, str, (Models.Manager.PageModels.EditModel)model);
					else if (model is Areas.Manager.Models.PostListModel && PostListToolbarRender != null)
						PostListToolbarRender(url, str, (Areas.Manager.Models.PostListModel)model);
					else if (model is Models.Manager.PostModels.EditModel && PostEditToolbarRender != null)
						PostEditToolbarRender(url, str, (Models.Manager.PostModels.EditModel)model);
					else if (model is Models.Manager.ContentModels.ListModel && MediaListToolbarRender != null)
						MediaListToolbarRender(url, str, (Models.Manager.ContentModels.ListModel)model);
					else if (model is Models.Manager.ContentModels.EditModel && MediaEditToolbarRender != null)
						MediaEditToolbarRender(url, str, (Models.Manager.ContentModels.EditModel)model);
					else if (model is Areas.Manager.Models.SiteTreeListModel && SiteListToolbarRender != null)
						SiteListToolbarRender(url, str, (Areas.Manager.Models.SiteTreeListModel)model);
					else if (model is Areas.Manager.Models.SiteTreeEditModel && SiteEditToolbarRender != null)
						SiteEditToolbarRender(url, str, (Areas.Manager.Models.SiteTreeEditModel)model);

					return new HtmlString(str.ToString());
				}
			}
		}

		public static class Mail
		{
			public static Delegates.SendPassword SendPassword;
		}
	}
}
