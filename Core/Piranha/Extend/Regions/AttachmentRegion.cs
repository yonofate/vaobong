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
using System.ComponentModel.Composition;
using System.Web.Script.Serialization;

namespace Piranha.Extend.Regions
{
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "AttachmentRegion")]
	[ExportMetadata("Name", "AttachmentRegionName")]
	[ExportMetadata("ResourceType", typeof(Resources.Extensions))]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class AttachmentRegion : Extension
	{
		#region Inner classes
		/// <summary>
		/// Inner class for an attachment.
		/// </summary>
		public class Attachment
		{
			/// <summary>
			/// The attached media id.
			/// </summary>
			public Guid Id { get; set; }

			/// <summary>
			/// The display name of the media.
			/// </summary>
			[ScriptIgnore]
			public string Name { get; set; }

			/// <summary>
			/// The content type of the media.
			/// </summary>
			[ScriptIgnore]
			public string Type { get; set; }

			[ScriptIgnore]
			public DateTime LastPublished { get; set; }
			[ScriptIgnore]
			public DateTime Updated { get; set; }
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the attachments.
		/// </summary>
		public IList<Attachment> Items { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AttachmentRegion() {
			Items = new List<Attachment>();
		}

		/// <summary>
		/// Initializes the region for the manager interface.
		/// </summary>
		/// <param name="model">The current model</param>
		public override void InitManager(object model) {
			var verified = new List<Attachment>();

			foreach (var item in Items) {
				var media = Models.Content.GetSingle(item.Id, true);

				if (media != null) {
					item.Name = media.DisplayName;
					item.Type = media.Type;
					item.Updated = media.Updated;
					item.LastPublished = media.LastPublished;

					verified.Add(item);
				}
			}
			Items = verified;
		}

		/// <summary>
		/// Gets the content for the client.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public override object GetContent(object model) {
			var ret = new List<Models.Content>();

			foreach (var item in Items) {
				var media = Models.Content.GetSingle(item.Id);

				if (media != null)
					ret.Add(media);
			}
			return ret;
		}
	}
}