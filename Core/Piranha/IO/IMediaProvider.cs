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
	/// The different types of media that media provider needs to handle
	/// </summary>
	public enum MediaType
	{
		/// <summary>
		/// All media content upload from the manager interface.
		/// </summary>
		Media,
		/// <summary>
		/// Media content uploaded by the client application.
		/// </summary>
		Upload
	}

	/// <summary>
	/// Interface defining the different actions a media provider need to implement.
	/// </summary>
	public interface IMediaProvider
	{
		/// <summary>
		/// Gets the data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The media data, null if the data wasn't found</returns>
		byte[] Get(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Gets the draft data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		/// <returns>The draft media data, null if the data wasn't found</returns>
		byte[] GetDraft(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Stores the given data for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="type">The media type</param>
		void Put(Guid id, byte[] data, MediaType type = MediaType.Media);

		/// <summary>
		/// Stores the given data as a draft for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="data">The media data</param>
		/// <param name="type">The media type</param>
		void PutDraft(Guid id, byte[] data, MediaType type = MediaType.Media);

		/// <summary>
		/// Deletes the data for the media entity with given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		void Delete(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Deletes the draft for the media entity with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		void DeleteDraft(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Publishes the current draft for the media entity with the given id. This means
		/// making the draft data the published data
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		void Publish(Guid id, MediaType type = MediaType.Media);

		/// <summary>
		/// Unpublishes the current draft for the media entity with the given id. This means
		/// making the published data the draft data
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="type">The media type</param>
		void Unpublish(Guid id, MediaType type = MediaType.Media);
	}
}
