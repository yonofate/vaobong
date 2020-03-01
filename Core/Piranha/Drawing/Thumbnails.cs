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

namespace Piranha.Drawing
{
	/// <summary>
	/// The thumbnails class handles all thumbnails to different types of content
	/// embedded in the core.
	/// </summary>
	public static class Thumbnails
	{
		#region Members
		private static Dictionary<Guid, string> guidToThumb = null;
		private static Dictionary<string, string> typeToThumb = null;
		private static Dictionary<string, Guid> typeToGuid = null;
		private static object mutex = new object();
		#endregion

		/// <summary>
		/// Gets whether the current thumbnail collection contains a
		/// thumbnail for the given mime-type.
		/// </summary>
		/// <param name="type">The mime-type</param>
		/// <returns>Whether the key exists</returns>
		public static bool ContainsKey(string type) {
			Ensure();
			return typeToThumb.ContainsKey(type);
		}

		/// <summary>
		/// Gets whether the current thumbnail collection contains a
		/// thumbnail for the given thumbnail id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>Whether the key exists</returns>
		public static bool ContainsKey(Guid id) {
			Ensure();
			return guidToThumb.ContainsKey(id);
		}

		/// <summary>
		/// Gets the resource path for the thumbnail with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>The resource path</returns>
		public static string GetById(Guid id) {
			Ensure();
			return guidToThumb[id];
		}

		/// <summary>
		/// Gets the resource path fo the thumbnail with the given mime-type.
		/// </summary>
		/// <param name="type">The mime-type</param>
		/// <returns>The resource path</returns>
		public static string GetByType(string type) {
			Ensure();
			if (!String.IsNullOrEmpty(type))
				return typeToThumb[type];
			return typeToThumb["default"];
		}

		/// <summary>
		/// Gets the internal thumbnail id for the given mime-type.
		/// </summary>
		/// <param name="type">The mime-type</param>
		/// <returns>The thumbnail id</returns>
		public static Guid GetIdByType(string type) {
			Ensure();
			if (!String.IsNullOrEmpty(type)) {
				if (ContainsKey(type))
					return typeToGuid[type];
			}
			return Guid.Empty;
		}

		#region Private methods
		/// <summary>
		/// Ensures that the thumbnail collection is created.
		/// </summary>
		private static void Ensure() {
			if (guidToThumb == null) {
				lock (mutex) {
					if (guidToThumb == null) {
						guidToThumb = new Dictionary<Guid, string>();
						typeToThumb = new Dictionary<string, string>();
						typeToGuid = new Dictionary<string, Guid>();

						// Pdf 
						Add(new Guid("6DC9DF6B-3378-4576-90B5-2E801C6484EA"), "application/pdf", "Piranha.Areas.Manager.Content.Img.ico-pdf-64.png");
						// Excel
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/vnd.ms-excel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/msexcel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/x-msexcel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/x-ms-excel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/x-excel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/x-dos_ms_excel", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/xls", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/x-xls", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						Add(new Guid("A37627AD-5973-4FDF-BA15-857B2640D16C"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Piranha.Areas.Manager.Content.Img.ico-excel-64.png");
						// Mp3
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mpeg", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-mpeg", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mp3", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-mp3", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mpeg3", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-mpeg3", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mpg", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-mpg", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-mpegaudio", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						// Wma
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/x-ms-wma", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						// Flac
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/flac", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						// Ogg
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/ogg", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						// M4a
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mp4a-latm", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						Add(new Guid("6356E47D-8926-456E-9A2D-B62CA7251FA9"), "audio/mp4", "Piranha.Areas.Manager.Content.Img.ico-sound-64.png");
						// Avi
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/avi", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/msvideo", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/x-msvideo", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "image/avi", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/xmpg2", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "application/x-troff-msvideo", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "audio/aiff", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "audio/avi", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						// Mpeg
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/mpeg", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/mp4", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						// Mov
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/quicktime", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "video/x-quicktime", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "image/mov", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "audio/x-midi", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						Add(new Guid("DBAD9093-1672-41EE-A5F7-F0433DF109A5"), "audio/x-wav", "Piranha.Areas.Manager.Content.Img.ico-movie-64.png");
						// Ppt
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/vnd.ms-powerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/mspowerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/ms-powerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/mspowerpnt", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/vnd-mspowerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/powerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/x-powerpoint", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						Add(new Guid("264D7937-6B78-43D5-A6C9-44D142406DB1"), "application/x-m", "Piranha.Areas.Manager.Content.Img.ico-ppt-64.png");
						// Zip
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/zip", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/x-zip", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/x-zip-compressed", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/octet-stream", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/x-compress", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "application/x-compressed", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						Add(new Guid("58A1653A-FF16-4D60-865C-156B76A9E038"), "multipart/x-zip", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						// Rar
						Add(new Guid("F63BBF1C-93F5-4B90-9337-232EBBB65908"), "application/x-rar-compressed", "Piranha.Areas.Manager.Content.Img.ico-zip-64.png");
						// Folder
						Add(new Guid("D604308B-BDFC-42FA-AAAA-7D0E9FE8DE5A"), "folder", "Piranha.Areas.Manager.Content.Img.ico-folder-96.png");
						Add(new Guid("D604308B-BDFC-42FA-AAAA-7D0E9FE8DE5A"), "folder-small", "Piranha.Areas.Manager.Content.Img.ico-folder-32.png");
						// Default
						Add(Guid.Empty, "default", "Piranha.Areas.Manager.Content.Img.ico-doc-64.png");
					}
				}
			}
		}

		/// <summary>
		/// Adds a thumbnail resource with the given id and mime-type.
		/// </summary>
		/// <param name="id">The thumbnail id</param>
		/// <param name="type">The mime-type</param>
		/// <param name="thumbnail">The resource path</param>
		private static void Add(Guid id, string type, string thumbnail) {
			if (!guidToThumb.ContainsKey(id))
				guidToThumb.Add(id, thumbnail);
			if (!typeToThumb.ContainsKey(type))
				typeToThumb.Add(type, thumbnail);
			if (!typeToGuid.ContainsKey(type))
				typeToGuid.Add(type, id);
		}
		#endregion
	}
}