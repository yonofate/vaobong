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
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;
using Piranha.Extend;

namespace Piranha.Models.Manager.SettingModels
{
	public class UserEditModel
	{
		#region Binder
		public class Binder : DefaultModelBinder
		{
			/// <summary>
			/// Extend the default binder so that html strings can be fetched from the post.
			/// </summary>
			/// <param name="controllerContext">Controller context</param>
			/// <param name="bindingContext">Binding context</param>
			/// <returns>The page edit model</returns>
			public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				UserEditModel model = (UserEditModel)base.BindModel(controllerContext, bindingContext);

				// Allow HtmlString extensions
				model.Extensions.Each((i, m) => {
					if (m.Body is HtmlString) {
						bindingContext.ModelState.Remove("Extensions[" + i + "].Body");
						m.Body = ExtensionManager.Current.CreateInstance(m.Type,
							bindingContext.ValueProvider.GetUnvalidatedValue("Extensions[" + i + "].Body").AttemptedValue);
					}
				});
				return model;
			}
		}
		#endregion

		#region Members
		private List<SysGroup> groups = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the current user.
		/// </summary>
		public virtual SysUser User { get; set; }

		/// <summary>
		/// Gets/sets the user password.
		/// </summary>
		public SysUserPassword Password { get; set; }

		/// <summary>
		/// Gets/sets the available groups.
		/// </summary>
		public SelectList Groups { get; set; }

		/// <summary>
		/// Gets/sets the extensions.
		/// </summary>
		public IList<Extension> Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a model.
		/// </summary>
		public UserEditModel() {
			List<SysGroup> gr = SysGroup.GetFields("sysgroup_id, sysgroup_name",
				new Params() { OrderBy = "sysgroup_id" });
			groups = new List<SysGroup>();
			groups.Insert(0, new SysGroup());
			gr.Each<SysGroup>((i, g) => {
				if (HttpContext.Current.User.IsMember(g.Id)) groups.Add(g);
			});

			User = new SysUser();
			Password = new SysUserPassword();
			Groups = new SelectList(groups, "Id", "Name");

			// Get extensions
			Extensions = User.GetExtensions(true);
		}

		/// <summary>
		/// Gets the user model for the given id.
		/// </summary>
		/// <param name="id">The user id</param>
		/// <returns>The model</returns>
		public static UserEditModel GetById(Guid id) {
			UserEditModel m = new UserEditModel();
			m.User = SysUser.GetSingle(id);
			m.Password = SysUserPassword.GetSingle(id);
			m.Groups = new SelectList(m.groups, "Id", "Name", m.User.GroupId);

			// Load extensions
			m.Extensions = m.User.GetExtensions(true);

			return m;
		}

		/// <summary>
		/// Saves the user and all related information.
		/// </summary>
		/// <returns>Whether the action succeeded or not.</returns>
		public virtual bool SaveAll() {
			Guid uid = new Guid("4037dc45-90d2-4adc-84aa-593be867c29d");

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					User.UpdatedBy = uid;
					User.Save(tx);
					if (Password.IsSet) {
						Password.Id = User.Id;
						Password.IsNew = false;
						Password.Save(tx);
					}
					foreach (var ext in Extensions) {
						// Call OnSave
						ext.Body.OnManagerSave(User);

						ext.ParentId = User.Id;
						ext.Save(tx);
					}
					tx.Commit();
				} catch { tx.Rollback(); throw; }
			}
			return true;
		}

		/// <summary>
		/// Deletes the user and all related information.
		/// </summary>
		/// <returns>Whether the action succeeded or not.</returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					// Call OnDelete for all extensions
					foreach (var ext in Extensions)
						ext.Body.OnManagerDelete(User);

					// Delete entities
					User.Delete(tx);
					tx.Commit();
				} catch { tx.Rollback(); return false; }
			}
			return true;
		}
	}
}
