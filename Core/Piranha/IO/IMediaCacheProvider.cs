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

namespace Piranha.IO
{
	/// <summary>
	/// The media cache provider handles all cached resized images from both the
	/// media library, as well as media uploaded from the application.
	/// </summary>
	public interface IMediaCacheProvider
	{
		/// <summary>
		/// Gets the data for the cached image with the given dimensions. In case of
		/// a cache miss null is returned.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		/// <returns>The binary data, null in case of a cache miss</returns>
		byte[] Get(Guid id, int width, int? height, MediaType type = MediaType.Media);

		/// <summary>
		/// Gets the draft data for the cached image with the given dimensions. In case of
		/// a cache miss null is returned.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		/// <returns>The binary data, null in case of a cache miss</returns>
		byte[] GetDraft(Guid id, int width, int? height, MediaType type = MediaType.Media);

		/// <summary>
		/// Stores the given cache data for the image with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		void Put(Guid id, byte[] data, int width, int? height, MediaType type = MediaType.Media);

		/// <summary>
		/// Stores the given cache data for the image draft with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The optional height of the image</param>
		/// <param name="type">The media type</param>
		void PutDraft(Guid id, byte[] data, int width, int? height, MediaType type = MediaType.Media);

		/// <summary>
		/// Deletes all cached images related to the given id, both draft and published.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		void Delete(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Gets the total size of all items in the cache.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The size of the cache in bytes.</returns>
		long GetTotalSize(Guid id, MediaType type = MediaType.Media);
	}
}
