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
	/// ReST API for the sitemap.
	/// </summary>
	[ServiceContract()]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class SitemapServices
	{
		/// <summary>
		/// Gets the sitemap.
		/// </summary>
		/// <returns>The sitemap</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/{internalid}", ResponseFormat = WebMessageFormat.Json)]
		public List<Sitemap> Get(string internalid) {
			List<Models.Sitemap> sm = Models.Sitemap.GetStructure(internalid.ToUpper(), true);
			return BuildMap(sm);
		}

		/// <summary>
		/// Gets the sitemap as xml.
		/// </summary>
		/// <returns>The sitemap</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/xml/{internalid}", ResponseFormat = WebMessageFormat.Xml)]
		public List<Sitemap> GetXml(string internalid) {
			return Get(internalid);
		}

		/// <summary>
		/// Gets the partial sitemap as xml.
		/// </summary>
		/// <param name="root">The root node id</param>
		/// <returns>The sitemap</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "getpartial/{id}", ResponseFormat = WebMessageFormat.Json)]
		public List<Sitemap> GetPartial(string id) {
			List<Models.Sitemap> sm = Models.Sitemap.GetStructure(true).GetRootNode(new Guid(id)).Pages;
			return BuildMap(sm);
		}

		/// <summary>
		/// Gets the partial sitemap.
		/// </summary>
		/// <param name="root">The root node id</param>
		/// <returns>The sitemap</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "getpartial/xml/{id}", ResponseFormat = WebMessageFormat.Xml)]
		public List<Sitemap> GetPartialXml(string id) {
			return GetPartial(id);
		}

		#region Private methods
		/// <summary>
		/// Builds the sitemap recursivly.
		/// </summary>
		private List<Sitemap> BuildMap(List<Models.Sitemap> sm) {
			List<Sitemap> sitemap = new List<Sitemap>();

			sm.ForEach(map => {
				if (map.GroupId == Guid.Empty || HttpContext.Current.User.IsMember(map.GroupId)) {
					sitemap.Add(new Sitemap() {
						Id = map.Id,
						Title = map.Title,
						Permalink = map.Permalink,
						IsHidden = map.IsHidden,
						HasChildren = map.Pages.Count > 0,
						ChildNodes = BuildMap(map.Pages),
						LastPublished = map.LastPublished.ToString()
					});
				}
			});
			return sitemap;
		}
		#endregion
	}
}
