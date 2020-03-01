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
using System.Reflection;
using System.Web.Mvc;

using Piranha.Models;
using Piranha.Web;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Controller for the manager area with built in access control.
	/// </summary>
	public abstract class ManagerController : Controller
	{
		/// <summary>
		/// Adds a success message to the current view.
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void SuccessMessage(string msg, bool persist = false) {
			ViewBag.MessageCss = "success";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "success";
				TempData["Message"] = msg;
			}
		}

		/// <summary>
		/// Adds an error message to the current view.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void ErrorMessage(string msg, bool persist = false) {
			ViewBag.MessageCss = "error";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "error";
				TempData["Message"] = msg;
			}
		}

		/// <summary>
		/// Adds an information message to the current view.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void InformationMessage(string msg, bool persist = false) {
			ViewBag.MessageCss = "";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "";
				TempData["Message"] = msg;
			}
		}

		/// <summary>
		/// Do additional security checks for the manager area.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext) {
			if (Application.Current.UserProvider.IsAuthenticated && User.HasAccess("ADMIN")) {
				// Get methodinfo for current action.
				MethodInfo m = null;

				try {
					m = this.GetType().GetMethod(filterContext.ActionDescriptor.ActionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				} catch {
					// If this fails we have multiple actions with the same name. We'll have to try and
					// match it on FormMethod.
					this.GetType().GetMethods().Each((i, method) => {
						if (method.Name.ToLower() == filterContext.ActionDescriptor.ActionName.ToLower()) {
							if (Request.HttpMethod == "POST") {
								if (method.GetCustomAttribute<HttpPostAttribute>(true) != null) {
									m = method;
								}
							} else if (Request.HttpMethod == "GET") {
								if (method.GetCustomAttribute<HttpGetAttribute>(true) != null ||
									method.GetCustomAttribute<HttpPostAttribute>(true) == null) {
									m = method;
								}
							}
						}
					});
				}

				if (m != null) {
					AccessAttribute attr = m.GetCustomAttribute<AccessAttribute>(true);
					if (attr != null) {
						if (!User.HasAccess(attr.Function))
							filterContext.Result = RedirectToAction("index", "account");
					}
				}

				// Get possible return url
				if (!String.IsNullOrEmpty(Request["returl"]))
					ViewBag.ReturnUrl = Request["returl"];
				else ViewBag.ReturnUrl = "";

				if (TempData.ContainsKey("MessageCss")) {
					ViewBag.MessageCss = TempData["MessageCss"];
					TempData.Remove("MessageCss");
				}
				if (TempData.ContainsKey("Message")) {
					ViewBag.Message = TempData["Message"];
					TempData.Remove("Message");
				}

				base.OnActionExecuting(filterContext);
			} else {
				filterContext.Result = RedirectToAction("index", "account");
			}
		}
	}
}
