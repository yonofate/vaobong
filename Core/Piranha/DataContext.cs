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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Piranha
{
	/// <summary>
	/// The Piranha DbContext
	/// </summary>
	public class DataContext : DbContext
	{
		#region Members
		/// <summary>
		/// Gets the logged in identity in case this context is used
		/// without a HttpContext.
		/// </summary>
		internal Guid Identity { get; private set; }
		#endregion

		#region DbSets
		// External DbSets
		public DbSet<Entities.User> Users { get; set; }
		public DbSet<Entities.Group> Groups { get; set; }
		public DbSet<Entities.Permission> Permissions { get; set; }
		public DbSet<Entities.Param> Params { get; set; }
		public DbSet<Entities.Log> Logs { get; set; }
		public DbSet<Entities.PageTemplate> PageTemplates { get; set; }
		public DbSet<Entities.PostTemplate> PostTemplates { get; set; }
		public DbSet<Entities.RegionTemplate> RegionTemplates { get; set; }
		public DbSet<Entities.Namespace> Namespaces { get; set; }
		public DbSet<Entities.SiteTree> SiteTrees { get; set; }
		public DbSet<Entities.Permalink> Permalinks { get; set; }
		public DbSet<Entities.Category> Categories { get; set; }
		public IQueryable<Entities.Media> Media { get { return Set<Entities.Media>().Where(m => !m.IsDraft); } }
		public IQueryable<Entities.Property> Properties { get { return Set<Entities.Property>().Where(p => !p.IsDraft); } }
		public IQueryable<Entities.Region> Regions { get { return Set<Entities.Region>().Where(r => !r.IsDraft); } }
		public IQueryable<Entities.Post> Posts { get { return Set<Entities.Post>().Where(p => !p.IsDraft); } }
		public IQueryable<Entities.Page> Pages { get { return Set<Entities.Page>().Where(p => !p.IsDraft); } }
		public DbSet<Entities.Extension> Extensions { get; set; }
		public DbSet<Entities.Upload> Uploads { get; set; }
		public DbSet<Entities.Comment> Comments { get; set; }

		// Drafts
		public IQueryable<Entities.Page> PageDrafts { get { return Set<Entities.Page>().Where(p => p.IsDraft); } }
		public IQueryable<Entities.Post> PostDrafts { get { return Set<Entities.Post>().Where(p => p.IsDraft); } }
		public IQueryable<Entities.Media> MediaDraft { get { return Set<Entities.Media>().Where(m => m.IsDraft); } }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new db context.
		/// </summary>
		public DataContext()
			: base("Piranha") {
			((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
				new ObjectMaterializedEventHandler(OnEntityLoad);
		}

		/// <summary>
		/// Logs in the given user to the current data context.
		/// </summary>
		/// <param name="login">The username</param>
		/// <param name="password">The password</param>
		/// <returns>Whether the login was successful</returns>
		public bool Login(string login, string password) {
			var usr = Models.SysUser.Authenticate(login, password);

			if (usr != null) {
				Identity = usr.Id;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Logs in the user with the given encrypted api key to the current data context.
		/// </summary>
		/// <param name="apiKey">The encrypted api key</param>
		/// <returns>Whether the login was successful</returns>
		public bool Login(string apiKey) {
			var id = Web.APIKeys.GetUserId(apiKey);

			if (id.HasValue) {
				Identity = id.Value;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Logs in the default sys user.
		/// </summary>
		public void LoginSys() {
			Identity = Config.SysUserId;
		}

		/// <summary>
		/// Logs out the current user.
		/// </summary>
		public void Logout() {
			Identity = Guid.Empty;
		}

		/// <summary>
		/// Initializes the current model.
		/// </summary>
		/// <param name="modelBuilder">The model builder</param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Configurations.Add(new Entities.Maps.UserMap());
			modelBuilder.Configurations.Add(new Entities.Maps.GroupMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PermissionMap());
			modelBuilder.Configurations.Add(new Entities.Maps.ParamMap());
			modelBuilder.Configurations.Add(new Entities.Maps.LogMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PageTemplateMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PostTemplateMap());
			modelBuilder.Configurations.Add(new Entities.Maps.RegionTemplateMap());
			modelBuilder.Configurations.Add(new Entities.Maps.NamespaceMap());
			modelBuilder.Configurations.Add(new Entities.Maps.SiteTreeMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PermalinkMap());
			modelBuilder.Configurations.Add(new Entities.Maps.CategoryMap());
			modelBuilder.Configurations.Add(new Entities.Maps.MediaMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PropertyMap());
			modelBuilder.Configurations.Add(new Entities.Maps.RegionMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PostMap());
			modelBuilder.Configurations.Add(new Entities.Maps.PageMap());
			modelBuilder.Configurations.Add(new Entities.Maps.ExtensionMap());
			modelBuilder.Configurations.Add(new Entities.Maps.UploadMap());
			modelBuilder.Configurations.Add(new Entities.Maps.CommentMap());

			if (!string.IsNullOrWhiteSpace(Config.EntitySchema)) {
				modelBuilder.HasDefaultSchema(Config.EntitySchema);
			}

			base.OnModelCreating(modelBuilder);
		}

		/// <summary>
		/// Event called when an entity has been loaded.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">Event arguments</param>
		void OnEntityLoad(object sender, ObjectMaterializedEventArgs e) {
			if (e.Entity is Entities.BaseEntity)
				((Entities.BaseEntity)e.Entity).OnLoad(this);
		}

		/// <summary>
		/// Saves the changes made to the context.
		/// </summary>
		/// <returns>The numbe of changes saved.</returns>
		public override int SaveChanges() {
			foreach (var entity in ChangeTracker.Entries()) {
				//
				// Call the correct software trigger.
				//
				if (entity.Entity is Entities.BaseEntity) {
					if (entity.State == EntityState.Added || entity.State == EntityState.Modified) {
						((Entities.BaseEntity)entity.Entity).OnSave(this, entity.State);
					} else if (entity.State == EntityState.Deleted) {
						((Entities.BaseEntity)entity.Entity).OnDelete(this);
					}
				}
			}
			return base.SaveChanges();
		}

		/// <summary>
		/// Validates an entity
		/// </summary>
		/// <param name="entity">The entity to validate</param>
		/// <param name="items">Optional params</param>
		/// <returns>The validation result</returns>
		protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entity, IDictionary<object, object> items) {
			DbEntityValidationResult ret = null;

			if (entity.Entity is Entities.BaseEntity)
				ret = ((Entities.BaseEntity)entity.Entity).OnValidate(entity);
			var valid = base.ValidateEntity(entity, items);
			if (ret != null) {
				foreach (var error in ret.ValidationErrors)
					valid.ValidationErrors.Add(error);
			}
			return valid;
		}
	}
}
