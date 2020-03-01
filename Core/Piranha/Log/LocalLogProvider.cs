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
using System.IO;
using System.Web;

namespace Piranha.Log
{
	/// <summary>
	/// Class for logging error messages.
	/// </summary>
	internal class LocalLogProvider : ILogProvider
	{
		#region Member
		/// <summary>
		/// The physical path to the log folder
		/// </summary>
		private readonly string path;

		/// <summary>
		/// The physical path to the log file
		/// </summary>
		private readonly string file;

		/// <summary>
		/// Mutex for keeping it thread safe.
		/// </summary>
		private readonly object mutex = new object();
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LocalLogProvider()
			: this(HttpContext.Current.Server.MapPath("~/App_Data/Logs"), HttpContext.Current.Server.MapPath("~/App_Data/Logs/Log.txt")) {

		}

		/// <summary>
		/// Creates a new local log provider using the specified folder and file for output
		/// </summary>
		/// <param name="folderPath">The folder containing the log file</param>
		/// <param name="filePath">The file path to the log file</param>
		public LocalLogProvider(string folderPath, string filePath) {

			if (string.IsNullOrEmpty(folderPath)) throw new ArgumentNullException("folderPath");
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

			this.path = folderPath;
			this.file = filePath;

			// Create the log directory if it doesn't exist
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		/// Logs an error to the log file.
		/// </summary>
		/// <param name="origin">The origin of the error</param>
		/// <param name="message">The message</param>
		/// <param name="details">Optional error details</param>
		public void Error(string origin, string message, Exception details = null) {
			lock (mutex) {
				using (var writer = new StreamWriter(file, true)) {
					writer.WriteLine(String.Format(
						"ERROR [{0}] Origin [{1}] Message [{2}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), origin, message));
					if (details != null)
						writer.WriteLine(details);
				}
			}
		}
	}
}