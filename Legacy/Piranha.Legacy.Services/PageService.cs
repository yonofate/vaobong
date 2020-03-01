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
	/// ReST API for pages.
	/// </summary>
	[ServiceContract()]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PageService : BaseService
	{
		/// <summary>
		/// Gets the page specified by the given id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/{id}")]
		public Stream Get(string id) {
			return Serialize(GetInternal(id));
		}

		/// <summary>
		/// Gets the page specified by the given id as xml.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/xml/{id}", ResponseFormat = WebMessageFormat.Xml)]
		public Page GetXml(string id) {
			return GetInternal(id);
		}

		/// <summary>
		/// Gets the page specified by the given id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page.</returns>
		internal Page GetInternal(string id) {
			try {
				Models.PageModel pm = Models.PageModel.GetById(new Guid(id));

				if (pm != null && (pm.Page.GroupId == Guid.Empty || HttpContext.Current.User.IsMember(pm.Page.GroupId))) {
					// Page data
					Page page = new Page() {
						Id = pm.Page.Id,
						ParentId = ((Piranha.Models.Page)pm.Page).ParentId,
						Seqno = ((Piranha.Models.Page)pm.Page).Seqno,
						TemplateName = pm.Page.TemplateName,
						Title = pm.Page.Title,
						NavigationTitle = pm.Page.NavigationTitle,
						Permalink = pm.Page.Permalink,
						IsHidden = ((Piranha.Models.Page)pm.Page).IsHidden,
						Created = pm.Page.Created.ToString(),
						Updated = pm.Page.Updated.ToString(),
						Published = pm.Page.Published.ToString(),
						LastPublished = pm.Page.LastPublished.ToString()
					};

					// Regions
					foreach (var key in ((IDictionary<string, object>)pm.Regions).Keys) {
						if (((IDictionary<string, object>)pm.Regions)[key] is HtmlString) {
							page.Regions.Add(new Region() {
								Name = key, Body =
	((HtmlString)((IDictionary<string, object>)pm.Regions)[key]).ToHtmlString()
							});
						} else {
							page.Regions.Add(new Region() {
								Name = key, Body =
	((IDictionary<string, object>)pm.Regions)[key]
							});
						}
					}

					// Properties
					foreach (var key in ((IDictionary<string, object>)pm.Properties).Keys)
						page.Properties.Add(new Property() {
							Name = key, Value = (string)
((string)((IDictionary<string, object>)pm.Properties)[key])
						});

					// Attachments
					foreach (var content in pm.Attachments)
						page.Attachments.Add(new Attachment() { Id = content.Id, IsImage = content.IsImage });

					// Extensions
					page.ExpandedExtensions = pm.Extensions;
					foreach (var key in ((IDictionary<string, object>)pm.Extensions).Keys) {
						if (((IDictionary<string, object>)pm.Extensions)[key] is HtmlString) {
							page.Extensions.Add(new Extension() {
								Name = key, Body =
((HtmlString)((IDictionary<string, object>)pm.Extensions)[key]).ToHtmlString()
							});
						} else {
							page.Extensions.Add(new Extension() {
								Name = key, Body =
((IDictionary<string, object>)pm.Extensions)[key]
							});
						}
					}
					return page;
				}
			} catch { }
			return null;
		}
	}
}
