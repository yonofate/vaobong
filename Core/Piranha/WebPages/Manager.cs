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

namespace Piranha.WebPages
{
	public static class Manager
	{
		#region Inner classes
		/// <summary>
		/// A menu group on the top level in the manager interface.
		/// </summary>
		public class MenuGroup
		{
			#region Properties
			/// <summary>
			/// The internal, non translatable textual id.
			/// </summary>
			public string InternalId { get; set; }

			/// <summary>
			/// The name of the group.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// The menu items this group contains.
			/// </summary>
			public IList<MenuItem> Items { get; set; }

			/// <summary>
			/// The optional css class of the group.
			/// </summary>
			public string CssClass { get; set; }

			/// <summary>
			/// Gets the name of the current controller.
			/// </summary>
			private string ControllerName {
				get { return HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower(); }
			}

			/// <summary>
			/// Gets the name of the current action.
			/// </summary>
			private string ActionName {
				get { return HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower(); }
			}
			#endregion

			/// <summary>
			/// Default constructor.
			/// </summary>
			public MenuGroup() {
				Items = new List<MenuItem>();
			}

			/// <summary>
			/// Checks if the current user has access to the group.
			/// </summary>
			/// <returns>Whether the current user has access.</returns>
			public bool HasAccess() {
				return ItemsForUser().Count > 0;
			}

			/// <summary>
			/// Checks if the current group is active.
			/// </summary>
			/// <returns>Whether the group is active</returns>
			public bool IsActive() {
				var controller = ControllerName;
				var action = ActionName;

				foreach (var item in ItemsForUser())
					if (item.Controller.ToLower() == controller) {
						if (String.IsNullOrEmpty(item.SelectedActions))
							return true;
						foreach (var str in item.SelectedActions.Split(new char[] { ',' }))
							if (str.Trim().ToLower() == action.ToLower())
								return true;
					}
				return false;
			}

			/// <summary>
			/// Gets the items available for the current user.
			/// </summary>
			/// <returns>The menu items</returns>
			public IList<MenuItem> ItemsForUser() {
				var ret = new List<MenuItem>();

				foreach (var item in Items)
					if (!String.IsNullOrEmpty(item.Permission)) {
						if (HttpContext.Current.User.HasAccess(item.Permission))
							ret.Add(item);
					} else ret.Add(item);
				return ret;
			}
		}

		/// <summary>
		/// A menu item in the manager interface.
		/// </summary>
		public class MenuItem
		{
			#region Properties
			/// <summary>
			/// The internal, non translatable textual id.
			/// </summary>
			public string InternalId { get; set; }

			/// <summary>
			/// Gets/sets the item name.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// Gets/sets the item action.
			/// </summary>
			public string Action { get; set; }

			/// <summary>
			/// Gets/sets the item controller.
			/// </summary>
			public string Controller { get; set; }

			/// <summary>
			/// Gets/sets the permission needed to access the item.
			/// </summary>
			public string Permission { get; set; }

			/// <summary>
			/// Gets/sets the selected actions for this item if several items share
			/// the same controller.
			/// </summary>
			public string SelectedActions { get; set; }
			#endregion

			/// <summary>
			/// Default constructor.
			/// </summary>
			public MenuItem() {
				SelectedActions = "";
			}

			/// <summary>
			/// Gets whether this menu item is currently active.
			/// </summary>
			/// <returns>Whether the item is active</returns>
			public bool IsActive() {
				var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
				var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
				if (String.IsNullOrEmpty(action))
					action = "index";

				if (Controller.ToLower() == controller) {
					if (!String.IsNullOrEmpty(SelectedActions)) {
						string[] actions = SelectedActions.Split(new char[] { ',' });

						foreach (var a in actions)
							if (a.Trim().ToLower() == action)
								return true;
					} else return true;
				}
				return false;
			}
		}
		#endregion

		/// <summary>
		/// The default manager menu.
		/// </summary>
		public static List<MenuGroup> Menu = new List<MenuGroup>() {
			new MenuGroup() { InternalId = "Content", Name = @Resources.Global.MenuContent, CssClass = "ico content", Items = new List<MenuItem>() {
				new MenuItem() { InternalId = "Pages", Name = @Resources.Tabs.ContentPages, Action = "index", Controller = "page", 
					Permission = "ADMIN_PAGE", },
				new MenuItem() { InternalId = "Posts", Name = @Resources.Tabs.ContentPosts, Action = "index", Controller = "post", 
					Permission = "ADMIN_POST" },
				new MenuItem() { InternalId = "Media", Name = @Resources.Tabs.ContentMedia, Action = "index", Controller = "content", 
					Permission = "ADMIN_CONTENT" },
				new MenuItem() { InternalId = "Comments", Name = @Resources.Tabs.SettingsComments, Action = "index", Controller = "comment", 
					Permission = "ADMIN_COMMENT", SelectedActions = "index, delete, edit" },
			}},
			new MenuGroup() { InternalId = "Settings", Name = @Resources.Global.MenuSettings, CssClass = "ico settings", Items = new List<MenuItem>() {
				new MenuItem() { InternalId = "PageTypes", Name = @Resources.Tabs.SettingsPageTypes, Action = "pagelist", Controller = "template", 
					Permission = "ADMIN_PAGE_TEMPLATE", SelectedActions = "pagelist, page" },
				new MenuItem() { InternalId = "PostTypes", Name = @Resources.Tabs.SettingsPostTypes, Action = "postlist", Controller = "template", 
					Permission = "ADMIN_POST_TEMPLATE", SelectedActions = "postlist, post" },
				new MenuItem() { InternalId = "Categories", Name = @Resources.Tabs.SettingsCategories, Action = "index", Controller = "category", 
					Permission = "ADMIN_CATEGORY" },
				new MenuItem() { InternalId = "Comments", Name = @Resources.Tabs.SettingsComments, Action = "settings", Controller = "comment", 
					Permission = "ADMIN_COMMENT", SelectedActions = "settings" },
				new MenuItem() { InternalId = "Sites", Name = @Resources.Tabs.SettingsSites, Action = "index", Controller = "sitetree",
					Permission = "ADMIN_SITETREE" },
			}},
			new MenuGroup() { InternalId = "System", Name = @Resources.Global.MenuSystem, CssClass = "ico system", Items = new List<MenuItem>() {
				new MenuItem() { InternalId = "Users", Name = @Resources.Tabs.SystemUsers, Action = "userlist", Controller = "settings", 
					Permission = "ADMIN_USER", SelectedActions = "userlist, usergroup, user, deleteuser" },
				new MenuItem() { InternalId = "Groups", Name = @Resources.Tabs.SystemGroups, Action = "grouplist", Controller = "settings", 
					Permission = "ADMIN_GROUP", SelectedActions = "grouplist, group, deletegroup" },
				new MenuItem() { InternalId = "Permissions", Name = @Resources.Tabs.SystemAccess, Action = "accesslist", Controller = "settings", 
					Permission = "ADMIN_ACCESS", SelectedActions = "accesslist, access, deleteaccess" },
				new MenuItem() { InternalId = "Parameters", Name = @Resources.Tabs.SystemParams, Action = "paramlist", Controller = "settings", 
					Permission = "ADMIN_PARAM", SelectedActions = "paramlist, param, deleteparam" }
			}}
		};

		/// <summary>
		/// Gets the currently active menu item.
		/// </summary>
		/// <returns>The menu item</returns>
		public static MenuItem GetActiveMenuItem() {
			foreach (var group in Menu) {
				if (group.IsActive()) {
					foreach (var item in group.Items)
						if (item.IsActive())
							return item;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the startpage for the currently logged in user.
		/// </summary>
		/// <returns>The menu item</returns>
		public static MenuItem GetStartpage() {
			foreach (var group in Menu) {
				foreach (var item in group.ItemsForUser()) {
					return item;
				}
			}
			return null;
		}
	}
}