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
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Security;
using Piranha.Models;

/// <summary>
/// Piranha class extensions
/// </summary>
public static class PiranhaApp
{
	#region Members
	public const string USER = "Piranha_User";
	#endregion

	#region Language extensions
	/// <summary>
	/// Implodes the string array into a string with all item separated by the given separator.
	/// </summary>
	/// <param name="arr">The array to implode</param>
	/// <param name="sep">The optional separator</param>
	/// <returns>The string</returns>
	public static string Implode(this string[] arr, string sep = "") {
		string ret = "";
		for (int n = 0; n < arr.Length; n++)
			ret += (n > 0 ? sep : "") + arr[n];
		return ret;
	}

	/// <summary>
	/// Gets a subset of the given array as a new array.
	/// </summary>
	/// <typeparam name="T">The array type</typeparam>
	/// <param name="arr">The array</param>
	/// <param name="startpos">The startpos</param>
	/// <param name="length">The length</param>
	/// <returns>The new array</returns>
	public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0) {
		List<T> tmp = new List<T>();

		length = length > 0 ? length : arr.Length - startpos;

		arr.Each<T>((i, e) => {
			if (i >= startpos && i < (startpos + length))
				tmp.Add(e);
		});
		return tmp.ToArray();
	}

	/// <summary>
	/// Loops the current enumerable and executes the given action on each of
	/// the items.
	/// </summary>
	/// <typeparam name="T">The item type</typeparam>
	/// <param name="ienum">The enumerable</param>
	/// <param name="proc">The action to execute</param>
	public static void Each<T>(this IEnumerable<T> ienum, Action<int, T> proc) {
		int index = 0;
		foreach (T itm in ienum)
			proc(index++, itm);
	}

	/// <summary>
	/// Gets the unvaliated value with the given key.
	/// </summary>
	/// <param name="provider">The value provider</param>
	/// <param name="key">The key</param>
	/// <returns>The value</returns>
	public static ValueProviderResult GetUnvalidatedValue(this IValueProvider provider, string key) {
		return ((IUnvalidatedValueProvider)provider).GetValue(key, true);
	}

	/// <summary>
	/// Gets the first custom attribute of type T for the given type.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The current type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>The attribute, if it was found</returns>
	public static T GetCustomAttribute<T>(this Type type, bool inherit) {
		object[] arr = type.GetCustomAttributes(typeof(T), inherit);

		return arr.Length > 0 ? (T)arr[0] : default(T);
	}

	/// <summary>
	/// Gets the first custom attribute of type T for the given member.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The current type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>The attribute, if it was found</returns>
	public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit) {
		object[] arr = member.GetCustomAttributes(typeof(T), inherit);

		return arr.Length > 0 ? (T)arr[0] : default(T);
	}

	/// <summary>
	/// Gets the attributes of type T for the given type.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>An array of attributes</returns>
	public static T[] GetCustomAttributes<T>(this Type type, bool inherit) {
		return Array.ConvertAll<object, T>(type.GetCustomAttributes(typeof(T), inherit), (o) => (T)o);
	}

	/// <summary>
	/// Ends the current request without throwing a ThreadAbortException
	/// </summary>
	/// <param name="response">The response</param>
	public static void EndClean(this HttpResponse response) {
		try {
			response.End();
		} catch (ThreadAbortException) {
			// We simply swallow this exception as we don't want unhandled
			// exceptions flying around causing the app pool to die.
		}
	}

	/// <summary>
	/// Ends the current request without throwing a ThreadAbortException
	/// </summary>
	/// <param name="response">The response</param>
	public static void EndClean(this HttpResponseBase response) {
		try {
			response.End();
		} catch (ThreadAbortException) {
			// We simply swallow this exception as we don't want unhandled
			// exceptions flying around causing the app pool to die.
		}
	}
	#endregion

	#region MVC Extensions
	/** 
	 * Add the extension @Html.PartialFor
	 * 
	 * Thanks to jmcd (https://github.com/jmcd)
	 *
	 * Original Gist can be found at:
	 * 
	 * https://gist.github.com/2137475
	 */
	public static MvcHtmlString PartialFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression) {
		return html.PartialFor(typeof(TValue).Name, expression);
	}

	public static MvcHtmlString PartialFor<TModel, TValue>(this HtmlHelper<TModel> html, string partialViewName, Expression<Func<TModel, TValue>> expression) {
		var containingModel = html.ViewData.Model;
		var model = expression.Compile()(containingModel);

		var oldTemplateInfo = html.ViewData.TemplateInfo;
		var newViewData = new ViewDataDictionary(html.ViewData) {
			TemplateInfo = new TemplateInfo {
				FormattedModelValue = oldTemplateInfo.FormattedModelValue,
			}
		};

		var newPrefix = ExpressionHelper.GetExpressionText(expression);
		if (oldTemplateInfo.HtmlFieldPrefix.Length > 0) {
			newPrefix = oldTemplateInfo.HtmlFieldPrefix + "." + newPrefix;
		}
		newViewData.TemplateInfo.HtmlFieldPrefix = newPrefix;

		return html.Partial(partialViewName, model, newViewData);
	}
	#endregion

	#region CMS extension
	/// <summary>
	/// Gets the currently logged in user.
	/// </summary>
	/// <param name="p">The security principal</param>
	/// <returns>The current user</returns>
	public static SysUser GetProfile(this IPrincipal p) {
		if (Piranha.Application.Current.UserProvider.IsAuthenticated) {
			// Reload user if session has been dropped
			if (HttpContext.Current.Session[USER] == null)
				HttpContext.Current.Session[USER] =
					SysUser.GetSingle(Piranha.Application.Current.UserProvider.UserId);
			return (SysUser)HttpContext.Current.Session[USER];
		}
		return new SysUser();
	}

	/// <summary>
	/// Checks if the current user has access to the function.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="function">The function to check</param>
	/// <returns>If the user has access</returns>
	public static bool HasAccess(this IPrincipal p, string function) {
		if (Piranha.Application.Current.UserProvider.IsAuthenticated) {
			Dictionary<string, SysAccess> access = SysAccess.GetAccessList();

			if (access.ContainsKey(function)) {
				SysGroup group = SysGroup.GetStructure().GetGroupById(p.GetProfile().GroupId);
				return group != null && (group.Id == access[function].GroupId || group.HasChild(access[function].GroupId));
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the user is a member of the given group or is a member
	/// of a group that has higher priviliges than the given group.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="groupid">The group</param>
	/// <returns>If the user is a member</returns>
	public static bool IsMember(this IPrincipal p, Guid groupid) {
		if (Piranha.Application.Current.UserProvider.IsAuthenticated) {
			if (groupid != Guid.Empty) {
				SysGroup g = SysGroup.GetStructure().GetGroupById(p.GetProfile().GroupId);
				return g.Id == groupid || g.HasChild(groupid);
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Checks if the user is a member of the given group or is a member
	/// of a group that has higher priviliges than the given group.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="groupname">The group</param>
	/// <returns>If the user is a member</returns>
	public static bool IsMember(this IPrincipal p, string groupname) {
		SysGroup g = SysGroup.GetSingle("sysgroup_name = @0", groupname);
		if (g != null)
			return IsMember(p, g.Id);
		return false;
	}

	/// <summary>
	/// Checks if the current user has access to the method. If the user is not
	/// authorized he/she is redirect to the correct url.
	/// </summary>
	/// <param name="m">The method</param>
	public static void CheckAccess(this MethodInfo m) {
		CheckAccess(HttpContext.Current.User, m.GetCustomAttribute<Piranha.AccessAttribute>(true));

	}

	/// <summary>
	/// Checks if the current user has access to the method. If the user is not
	/// authorized he/she is redirect to the correct url.
	/// </summary>
	/// <param name="m">The method</param>
	public static void CheckAccess(this Type t) {
		CheckAccess(HttpContext.Current.User, t.GetCustomAttribute<Piranha.AccessAttribute>(true));

	}

	/// <summary>
	/// Checks the given users access to the permissions specified in the given
	/// access attribute.
	/// </summary>
	/// <param name="user">The user</param>
	/// <param name="access">The attribute</param>
	private static void CheckAccess(IPrincipal user, Piranha.AccessAttribute access) {
		if (access != null) {
			if (!user.HasAccess(access.Function)) {
				if (!String.IsNullOrEmpty(access.RedirectUrl)) {
					HttpContext.Current.Response.Redirect(access.RedirectUrl, false);
				} else {
					SysParam param = SysParam.GetByName("LOGIN_PAGE");
					if (param != null)
						HttpContext.Current.Response.Redirect(param.Value, false);
					else HttpContext.Current.Response.Redirect("~/", false);
				}
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
		}
	}
	#endregion

	#region Entity extensions
	/// <summary>
	/// Gets the sitemap with the given id from the structure
	/// </summary>
	/// <param name="sm">The sitemap</param>
	/// <param name="id">The id</param>
	/// <returns>The partial sitemap</returns>
	public static Sitemap GetRootNode(this List<Sitemap> sm, Guid id) {
		if (sm != null) {
			foreach (Sitemap page in sm) {
				if (page.Id == id)
					return page;
				Sitemap subpage = GetRootNode(page.Pages, id);
				if (subpage != null)
					return subpage;
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the number of images in the content list.
	/// </summary>
	/// <param name="self">The content list</param>
	/// <returns>The image count</returns>
	public static int CountImages(this List<Content> self) {
		int images = 0;
		self.ForEach((c) => {
			if (c.IsImage)
				images++;
		});
		return images;
	}

	/// <summary>
	/// Gets the number of documents in the content list.
	/// </summary>
	/// <param name="self">The content list</param>
	/// <returns>The document count</returns>
	public static int CountDocuments(this List<Content> self) {
		int documents = 0;
		self.ForEach((c) => {
			if (!c.IsImage)
				documents++;
		});
		return documents;
	}

	/// <summary>
	/// Gets the available images from the content list.
	/// </summary>
	/// <param name="self">The content list</param>
	/// <returns>The images</returns>
	public static List<Content> Images(this List<Content> self) {
		return self.Where(c => c.IsImage).ToList();
	}

	/// <summary>
	/// Gets the available documents from the content list.
	/// </summary>
	/// <param name="self">The content list</param>
	/// <returns>The documents</returns>
	public static List<Content> Documents(this List<Content> self) {
		return self.Where(c => !c.IsImage).ToList();
	}
	#endregion
}
