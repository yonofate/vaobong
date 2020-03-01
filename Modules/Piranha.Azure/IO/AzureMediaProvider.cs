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
using System.Configuration;
using System.IO;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

using Piranha.IO;

namespace Piranha.Azure.IO
{
	/// <summary>
	/// Media provider for storing data on Windows Azure Blob storage.
	/// </summary>
	public class AzureMediaProvider : IMediaProvider
	{
		#region Members
		private readonly CloudStorageAccount account;
		private readonly CloudBlobClient client;

		private readonly CloudBlobContainer mediaContainer;
		private readonly CloudBlobContainer uploadContainer;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AzureMediaProvider() {
			var config = GetConfig();

			account = CloudStorageAccount.Parse(config.Settings.StorageConnectionString.Value);
			client = account.CreateCloudBlobClient();

			mediaContainer = client.GetContainerReference("media");
			mediaContainer.CreateIfNotExists();

			uploadContainer = client.GetContainerReference("uploads");
			uploadContainer.CreateIfNotExists();
		}

		/// <summary>
		/// Gets the data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The media data, null if the data wasn't found</returns>
		public byte[] Get(Guid id, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, false);

			if (blob.Exists()) {
				using (var stream = new MemoryStream()) {
					blob.DownloadToStream(stream);
					return stream.ToArray();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the draft data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The draft media data, null if the data wasn't found</returns>
		public byte[] GetDraft(Guid id, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, true);

			if (blob.Exists()) {
				using (var stream = new MemoryStream()) {
					blob.DownloadToStream(stream);
					return stream.ToArray();
				}
			}
			return null;
		}

		/// <summary>
		/// Stores the given data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="type">The media type</param>
		public void Put(Guid id, byte[] data, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, false);

			using (var stream = new MemoryStream(data)) {
				blob.UploadFromStream(stream);
			}
		}

		/// <summary>
		/// Stores the given data as a draft for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="type">The media type</param>
		public void PutDraft(Guid id, byte[] data, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, true);

			using (var stream = new MemoryStream(data)) {
				blob.UploadFromStream(stream);
			}
		}

		/// <summary>
		/// Deletes the data for the media entity with given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		public void Delete(Guid id, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, false);
			blob.DeleteIfExists();
		}

		/// <summary>
		/// Deletes the draft for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		public void DeleteDraft(Guid id, MediaType type = MediaType.Media) {
			var blob = GetBlock(type, id, true);
			blob.DeleteIfExists();
		}

		/// <summary>
		/// Publishes the current draft for the media entity with the given id. This means
		/// making the draft data the published data
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		public void Publish(Guid id, MediaType type = MediaType.Media) {
			var bytes = GetDraft(id, type);

			if (bytes != null) {
				Put(id, bytes, type);
				DeleteDraft(id, type);
			}
		}

		/// <summary>
		/// Unpublishes the current draft for the media entity with the given id. This means
		/// making the published data the draft data
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		public void Unpublish(Guid id, MediaType type = MediaType.Media) {
			var bytes = Get(id, type);

			if (bytes != null) {
				PutDraft(id, bytes, type);
				Delete(id, type);
			}
		}

		#region Private methods
		/// <summary>
		/// Gets the blob block for the given entity.
		/// </summary>
		/// <param name="type">The media type</param>
		/// <param name="id">The id</param>
		/// <param name="draft">If this is a draft or not</param>
		/// <returns>The blob block</returns>
		private CloudBlockBlob GetBlock(MediaType type, Guid id, bool draft) {
			if (type == MediaType.Media)
				return mediaContainer.GetBlockBlobReference(id.ToString() + (draft ? "-draft" : ""));
			return uploadContainer.GetBlockBlobReference(id.ToString() + (draft ? "-draft" : ""));
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
