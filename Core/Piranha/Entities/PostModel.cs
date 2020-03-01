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
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using Piranha.Extend;

namespace Piranha.Entities
{
	/// <summary>
	/// Entity post model.
	/// </summary>
	public class PostModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the post
		/// </summary>
		public Post Post { get; set; }

		/// <summary>
		/// Gets/sets the attachments.
		/// </summary>
		public IList<Media> Attachments { get; set; }

		/// <summary>
		/// Gets/sets the properties.
		/// </summary>
		public dynamic Properties { get; set; }

		/// <summary>
		/// Gets/sets the extensions.
		/// </summary>
		public dynamic Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected PostModel() {
			Properties = new ExpandoObject();
			Extensions = new ExpandoObject();
			Attachments = new List<Media>();
		}

		/// <summary>
		/// Gets the post models matching the given predicate.
		/// </summary>
		/// <param name="predicate">The predicate</param>
		/// <returns>A list of models</returns>
		public static IList<PostModel> Where(Expression<Func<Post, bool>> predicate) {
			var ret = new List<PostModel>();

			using (var db = new DataContext()) {
				var posts = db.Posts
					.Include(p => p.Template)
					.Include(p => p.Properties)
					.Include(p => p.Extensions)
					.Where(predicate)
					.ToList();

				foreach (var post in posts)
					ret.Add(BuildModel(db, post));
			}
			return ret;
		}

		/// <summary>
		/// Builds a post model object from the given post.
		/// </summary>
		/// <param name="db">The data context</param>
		/// <param name="post">The post</param>
		/// <returns>The model</returns>
		private static PostModel BuildModel(DataContext db, Post post) {
			var m = new PostModel();

			m.Post = post;

			// Get Properties
			foreach (var pt in post.Template.Properties) {
				var val = post.Properties.Where(p => p.Name == pt).SingleOrDefault();

				if (val != null)
					((IDictionary<string, object>)m.Properties).Add(pt, val.Value);
				else ((IDictionary<string, object>)m.Properties).Add(pt, "");
			}
			// Get Extensions
			foreach (var ext in post.Extensions) {
				if (ExtensionManager.Current.HasType(ext.Type))
					((IDictionary<string, object>)m.Extensions)[ExtensionManager.Current.GetInternalIdByType(ext.Type)] = ext.Body;
			}
			// Get Media
			var media = db.Media.Where(med => post.Attachments.Contains(med.Id)).ToList();
			foreach (var attachment in post.Attachments) {
				var val = media.Where(med => med.Id == attachment).SingleOrDefault();
				if (val != null)
					m.Attachments.Add(val);
			}

			// Initialize extensions
			foreach (var key in ((IDictionary<string, object>)m.Extensions).Keys) {
				var val = ((IDictionary<string, object>)m.Extensions)[key];

				if (val != null) {
					var method = val.GetType().GetMethod("Init");
					if (method != null)
						val = method.Invoke(val, new object[] { m });
				}
			}
			return m;
		}
	}
}