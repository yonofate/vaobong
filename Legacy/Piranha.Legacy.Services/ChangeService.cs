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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

using Piranha.Data;
using Piranha.Legacy.Services.DataContracts;

namespace Piranha.Legacy.Services
{
	/// <summary>
	/// ReST API for syncing changes.
	/// </summary>
	[ServiceContract()]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ChangeService : BaseService
	{
		[OperationContract()]
		[WebGet(UriTemplate = "get/{internalid}/{date}/{time}", ResponseFormat = WebMessageFormat.Json)]
		public Stream GetChanges(string internalid, string date, string time) {
			return Serialize(GetChangesInternal(internalid.ToUpper(), date + FormatTime(time)));
		}

		public Changes GetChangesInternal(string internalid, string date) {
			Changes changes = new Changes();
			DateTime latest;

			try {
				latest = Convert.ToDateTime(date);
			} catch {
				latest = new DateTime(2000, 1, 1);
			}

			// Get all deleted content
			string query = "syslog_parent_type = @0 AND syslog_action = @1 AND syslog_created > @2";
			changes.Deleted.Pages = Piranha.Models.SysLog.Get(query, "PAGE", "DEPUBLISH", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.Posts = Piranha.Models.SysLog.Get(query, "POST", "DEPUBLISH", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.Content = Piranha.Models.SysLog.Get(query, "CONTENT", "DELETE", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.Categories = Piranha.Models.SysLog.Get(query, "CATEGORY", "DELETE", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.PageTemplates = Piranha.Models.SysLog.Get(query, "PAGETEMPLATE", "DELETE", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.PostTemplates = Piranha.Models.SysLog.Get(query, "POSTTEMPLATE", "DELETE", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();
			changes.Deleted.MediaFolders = Piranha.Models.SysLog.Get(query, "MEDIAFOLDER", "DELETE", latest).
				Select(l => new DeletedItem() { Id = l.ParentId, Deleted = l.Created.ToString() }).ToList();

			// Check if we have deleted pages or pages publised after the given date. If so return the sitemap
			if (changes.Deleted.Pages.Count > 0 || Models.Page.GetScalar("SELECT COUNT(page_id) FROM page JOIN sitetree ON page_sitetree_id = sitetree_id WHERE page_last_published > @0 AND sitetree_internal_id = @1", latest, internalid) > 0)
				changes.Sitemap = new SitemapServices().Get(internalid);

			// Get all pages last published after the given date.
			Models.Page.GetFields("page_id", "page_last_modified > @0 AND page_draft = 0 AND sitetree_internal_id = @1", latest, internalid).ForEach(p =>
				changes.Pages.Add(new PageService().GetInternal(p.Id.ToString())));

			// Get all posts last published after the given date.
			Models.Post.GetFields("post_id", "post_last_modified > @0 AND post_draft = 0", latest).ForEach(p =>
				changes.Posts.Add(new PostService().GetInternal(p.Id.ToString())));

			// Get all categories updated after the given date.
			new CategoryService().Get().Where(c => Convert.ToDateTime(c.Updated) > latest).
				Each((i, c) => changes.Categories.Add(c));

			// Get all content updated after the given date.
			Models.Content.GetFields("content_id", "content_last_published > @0 AND content_draft = 0 AND content_folder = 0", latest).ForEach(c =>
				changes.Content.Add(new ContentService().Get(c.Id.ToString())));

			// Get all page templates updated after the given date.
			Models.PageTemplate.GetFields("pagetemplate_id", "pagetemplate_updated > @0", latest, new Params() { OrderBy = "pagetemplate_name" }).ForEach(pt =>
				changes.PageTemplates.Add(new PageTemplateService().Get(pt.Id.ToString())));

			// Get all post templates updated after the given date.
			Models.PostTemplate.GetFields("posttemplate_id", "posttemplate_updated > @0", latest, new Params() { OrderBy = "posttemplate_name" }).ForEach(pt =>
				changes.PostTemplates.Add(new PostTemplateService().Get(pt.Id.ToString())));

			// Check if we have deleted pages or pages publised after the given date. If so return the sitemap
			if (changes.Deleted.MediaFolders.Count > 0 || Models.Content.GetScalar("SELECT COUNT(content_id) FROM content WHERE content_last_published > @0", latest) > 0)
				changes.MediaFolders = new ContentService().GetFolders();

			// Set the timespage
			changes.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

			return changes;
		}

		private string FormatTime(string str) {
			if (!String.IsNullOrEmpty(str))
				return " " + str.Replace('-', ':');
			return "";
		}
	}
}
