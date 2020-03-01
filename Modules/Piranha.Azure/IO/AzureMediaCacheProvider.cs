/*
 * Copyright (c) 2011-2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Piranha.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piranha.Azure.IO
{
	/// <summary>
	/// Media Cache provider for storing cache data on Windows Azure Blob storage.
	/// </summary>
	public class AzureMediaCacheProvider : IMediaCacheProvider
	{
		#region Members
		private readonly CloudStorageAccount account;
		private readonly CloudBlobClient client;

		private readonly CloudBlobContainer cacheContainer;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AzureMediaCacheProvider() {
			var config = GetConfig();

			account = CloudStorageAccount.Parse(config.Settings.StorageConnectionString.Value);
			client = account.CreateCloudBlobClient();

			cacheContainer = client.GetContainerReference("cache");
			cacheContainer.CreateIfNotExists();
		}

		/// <summary>
		/// Gets the data for the cached image with the given dimensions. In case of
		/// a cache miss null is returned.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		/// <returns>
		/// The binary data, null in case of a cache miss
		/// </returns>
		public byte[] Get(Guid id, int width, int? height, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, width, height, false);

			if (blob.Exists()) {
				using (var stream = new MemoryStream()) {
					blob.DownloadToStream(stream);
					return stream.ToArray();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the draft data for the cached image with the given dimensions. In case of
		/// a cache miss null is returned.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		/// <returns>
		/// The binary data, null in case of a cache miss
		/// </returns>
		public byte[] GetDraft(Guid id, int width, int? height, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, width, height, true);

			if (blob.Exists()) {
				using (var stream = new MemoryStream()) {
					blob.DownloadToStream(stream);
					return stream.ToArray();
				}
			}
			return null;
		}

		/// <summary>
		/// Stores the given cache data for the image with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		public void Put(Guid id, byte[] data, int width, int? height, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, width, height, false);

			using (var stream = new MemoryStream(data)) {
				blob.UploadFromStream(stream);
			}
		}

		/// <summary>
		/// Stores the given cache data for the image draft with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		public void PutDraft(Guid id, byte[] data, int width, int? height, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, width, height, true);

			using (var stream = new MemoryStream(data)) {
				blob.UploadFromStream(stream);
			}
		}

		/// <summary>
		/// Deletes all cached images related to the given id, both draft and published.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		public void Delete(Guid id, MediaType type = MediaType.Media) {
			foreach (var listing in cacheContainer.ListBlobs(prefix: GetVirtualPrefix(type, id), useFlatBlobListing: true)) {
				var blob = listing as ICloudBlob;
				if (blob != null) {
					blob.DeleteIfExists();
				}
			}
		}

		/// <summary>
		/// Gets the total size of all items in the cache.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The size of the cache in bytes.</returns>
		public long GetTotalSize(Guid id, MediaType type = MediaType.Media) {
			long size = 0;
			foreach (var listing in cacheContainer.ListBlobs(prefix: GetVirtualPrefix(type, id), useFlatBlobListing: true)) {
				var blob = listing as ICloudBlob;
				if (blob != null) {
					size += blob.Properties.Length;
				}
			}
			return size;
		}

		#region Private methods
		/// <summary>
		/// Gets the blob block for the given entity.
		/// </summary>
		/// <param name="type">The media type</param>
		/// <param name="id">The id</param>
		/// <param name="draft">If this is a draft or not</param>
		/// <returns>The blob block</returns>
		private CloudBlockBlob GetBlock(MediaType type, Guid id, int width, int? height, bool draft) {
			return cacheContainer.GetBlockBlobReference(GetVirtualPath(type, id, width, height, draft));
		}

		/// <summary>
		/// Gets the blob's virtual path for the given entity.
		/// </summary>
		/// <param name="type">The media type</param>
		/// <param name="id">The id</param>
		/// <param name="draft">If this is a draft or not</param>
		/// <returns>The blob path</returns>
		private string GetVirtualPath(MediaType type, Guid id, int? width, int? height, bool draft) {
			StringBuilder virtualPath = new StringBuilder();

			virtualPath.Append(GetVirtualPrefix(type, id));

			if (draft)
				virtualPath.Append("draft-");

			if (width.HasValue)
				virtualPath.AppendFormat("{0}", width.Value);

			if (height.HasValue)
				virtualPath.AppendFormat("x{0}", height.Value);

			return virtualPath.ToString();
		}

		/// <summary>
		/// Gets the blob's virtual prefix for the given entity.
		/// </summary>
		/// <param name="type">The media type</param>
		/// <param name="id">The id</param>
		/// <returns>The blob prefix</returns>
		private string GetVirtualPrefix(MediaType type, Guid id) {
			if (type == MediaType.Media)
				return string.Format("Content/{0}", id.ToString());
			else
				return string.Format("Uploads/{0}", id.ToString());
		}

		/// <summary>
		/// Gets the configuration section from the Web.config.
		/// </summary>
		/// <returns>The configuration</returns>
		private ConfigFile GetConfig() {
			var section = (ConfigFile)ConfigurationManager.GetSection("piranha.azure");

			if (section == null)
				section = new ConfigFile();
			return section;
		}
		#endregion
	}
}
