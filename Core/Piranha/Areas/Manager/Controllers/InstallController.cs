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
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;

using Piranha.Data;
using Piranha.Data.Updates;
using Piranha.Models;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Post model for installation.
	/// </summary>
	public class InstallModel
	{
		/// <summary>
		/// Gets/sets the login of the new admin account.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "LoginRequired")]
		public string UserLogin { get; set; }

		/// <summary>
		/// Gets/sets the email address of the new admin account.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "EmailRequired")]
		public string UserEmail { get; set; }

		/// <summary>
		/// Gets/sets the password of the new admin account.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(Piranha.Resources.Settings), ErrorMessageResourceName = "PasswordRequired")]
		public string Password { get; set; }

		/// <summary>
		/// Gets/sets the password confirmation.
		/// </summary>
		[System.ComponentModel.DataAnnotations.Compare("Password",
			ErrorMessageResourceType = typeof(Piranha.Resources.Settings),
			ErrorMessageResourceName = "PasswordConfirmError")]
		public string PasswordConfirm { get; set; }

		/// <summary>
		/// Gets/sets the current installation type.
		/// </summary>
		public string InstallType { get; set; }
	}

	/// <summary>
	/// Login controller for the manager interface.
	/// </summary>
	public class InstallController : Controller
	{
		/// <summary>
		/// Checks for the piranha connection string and
		/// if the database is up to date.
		/// </summary>
		public ActionResult Index() {
			// Check for no database-config
			if (ConfigurationManager.ConnectionStrings["piranha"] == null)
				return RedirectToAction("welcome");

			// Check for existing installation.
			try {
				if (Data.Database.InstalledVersion < Data.Database.CurrentVersion)
					return RedirectToAction("update", "install");
				return RedirectToAction("index", "account");
			} catch {
				if (Config.ShowDBErrors)
					throw;
			}
			return View("Index");
		}

		/// <summary>
		/// Shows the update page.
		/// </summary>
		public ActionResult Update() {
			if (Data.Database.InstalledVersion < Data.Database.CurrentVersion)
				return View("Update");
			return RedirectToAction("index", "account");
		}

		/// <summary>
		/// Shows the welcome screen.
		/// </summary>
		public ActionResult Welcome() {
			return View();
		}

		/// <summary>
		/// Logins in the specified user and starts the update.
		/// </summary>
		[HttpPost()]
		public ActionResult RunUpdate(LoginModel m) {
			// Authenticate the user
			if (ModelState.IsValid) {
				SysUser user = SysUser.Authenticate(m.Login, m.Password);
				if (user != null) {
					FormsAuthentication.SetAuthCookie(user.Id.ToString(), m.RememberMe);
					HttpContext.Session[PiranhaApp.USER] = user;
					return RedirectToAction("ExecuteUpdate");
				} else {
					ViewBag.Message = @Piranha.Resources.Account.MessageLoginFailed;
					ViewBag.MessageCss = "error";
					return Update();
				}
			} else {
				ViewBag.Message = @Piranha.Resources.Account.MessageLoginEmptyFields;
				ViewBag.MessageCss = "";
				return Update();
			}
		}

		/// <summary>
		/// Executes the available database scripts on the database.
		/// </summary>
		[HttpGet()]
		public ActionResult ExecuteUpdate() {
			if (Application.Current.UserProvider.IsAuthenticated && User.HasAccess("ADMIN")) {
				// Execute all incremental updates in a transaction.
				using (IDbTransaction tx = Database.OpenTransaction()) {
					for (int n = Data.Database.InstalledVersion + 1; n <= Data.Database.CurrentVersion; n++) {
						// Read embedded create script
						Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream(Database.ScriptRoot + ".Updates." +
							n.ToString() + ".sql");
						String sql = new StreamReader(str).ReadToEnd();
						str.Close();

						// Split statements and execute
						string[] stmts = sql.Split(new char[] { ';' });
						foreach (string stmt in stmts) {
							if (!String.IsNullOrEmpty(stmt.Trim()))
								SysUser.Execute(stmt.Trim(), tx);
						}

						// Check for update class
						var utype = Type.GetType("Piranha.Data.Updates.Update" + n.ToString());
						if (utype != null) {
							IUpdate update = (IUpdate)Activator.CreateInstance(utype);
							update.Execute(tx);
						}
					}
					// Now lets update the database version.
					SysUser.Execute("UPDATE sysparam SET sysparam_value = @0 WHERE sysparam_name = 'SITE_VERSION'",
						tx, Data.Database.CurrentVersion);
					SysParam.InvalidateParam("SITE_VERSION");
					tx.Commit();
				}
				return RedirectToAction("index", "account");
			} else return RedirectToAction("update");
		}

		/// <summary>
		/// Creates a new site installation.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost()]
		public ActionResult Create(InstallModel m) {
			if (m.InstallType == "SCHEMA" || ModelState.IsValid) {
				// Read embedded create script
				Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream(Database.ScriptRoot + ".Create.sql");
				String sql = new StreamReader(str).ReadToEnd();
				str.Close();

				// Read embedded data script
				str = Assembly.GetExecutingAssembly().GetManifestResourceStream(Database.ScriptRoot + ".Data.sql");
				String data = new StreamReader(str).ReadToEnd();
				str.Close();

				// Split statements and execute
				string[] stmts = sql.Split(new char[] { ';' });
				using (IDbTransaction tx = Database.OpenTransaction()) {
					// Create database from script
					foreach (string stmt in stmts) {
						if (!String.IsNullOrEmpty(stmt.Trim()))
							SysUser.Execute(stmt, tx);
					}
					tx.Commit();
				}

				if (m.InstallType.ToUpper() == "FULL") {
					// Split statements and execute
					stmts = data.Split(new char[] { ';' });
					using (IDbTransaction tx = Database.OpenTransaction()) {
						// Create user
						SysUser usr = new SysUser() {
							Login = m.UserLogin,
							Email = m.UserEmail,
							GroupId = new Guid("7c536b66-d292-4369-8f37-948b32229b83"),
							CreatedBy = new Guid("ca19d4e7-92f0-42f6-926a-68413bbdafbc"),
							UpdatedBy = new Guid("ca19d4e7-92f0-42f6-926a-68413bbdafbc"),
							Created = DateTime.Now,
							Updated = DateTime.Now
						};
						usr.Save(tx);

						// Create user password
						SysUserPassword pwd = new SysUserPassword() {
							Id = usr.Id,
							Password = m.Password,
							IsNew = false
						};
						pwd.Save(tx);

						// Create default data
						foreach (string stmt in stmts) {
							if (!String.IsNullOrEmpty(stmt.Trim()))
								SysUser.Execute(stmt, tx);
						}
						tx.Commit();
					}
				}
				//
				// Make sure we reload the hostnames after install
				//
				WebPages.WebPiranha.RegisterDefaultHostNames();

				return RedirectToAction("index", "account");
			}
			return Index();
		}
	}
}
