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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.ContentModels
{
	/// <summary>
	/// List model for the content view in the manager area.
	/// </summary>
	public class PopupModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available content.
		/// </summary>
		public List<Piranha.Models.Content> Content { get; set; }

		/// <summary>
		/// Gets/sets the new content for uploading.
		/// </summary>
		public Piranha.Models.Content NewContent { get; set; }

		/// <summary>
		/// Gets/sets the available folders.
		/// </summary>
		public IList<Placement> Folders { get; set; }

		/// <summary>
		/// Gets/sets the currently selected folder.
		/// </summary>
		public Guid SelectedFolder { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public PopupModel() {
			Content = new List<Piranha.Models.Content>();
			NewContent = new Piranha.Models.Content();
			Folders = SortFolders(Models.Content.GetFolderStructure(false));
			Folders.Insert(0, new Placement() { Text = "", Value = Guid.Empty });
		}

		/// <summary>
		/// Gets all available content.
		/// </summary>
		/// <returns>A list of content records</returns>
		public static PopupModel Get(string id = "", bool published = false, string filter = "") {
			PopupModel lm = new PopupModel();

			if (!String.IsNullOrEmpty(id)) {
				lm.SelectedFolder = new Guid(id);
				lm.Content = Piranha.Models.Content.GetStructure(new Guid(id), true, published);
			} else lm.Content = Piranha.Models.Content.GetStructure(published);

			return lm;
		}

		#region Private methods
		/// <summary>
		/// Flattens the folder structure.
		/// </summary>
		/// <param name="media"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		private List<Placement> SortFolders(List<Content> media, int level = 1) {
			var ret = new List<Placement>();

			foreach (var m in media) {
				var prefix = "";
				for (int n = 1; n < level; n++)
					prefix += "&nbsp;&nbsp;&nbsp;";
				ret.Add(new Placement() {
					Text = prefix + m.Name,
					Value = m.Id,
					Level = level
				});
				ret.AddRange(SortFolders(m.ChildContent, level + 1));
			}
			return ret;
		}
		#endregion
	}
}
