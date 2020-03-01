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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Piranha.Log;

namespace Piranha.Azure.Log
{
	/// <summary>
	/// Log provider for storing logs on Windows Azure.
	/// </summary>
	public class TraceLogProvider : ILogProvider
	{
		/// <summary>
		/// Logs an error to the log file.
		/// </summary>
		/// <param name="origin">The origin of the error</param>
		/// <param name="message">The message</param>
		/// <param name="details">Optional error details</param>
		public void Error(string origin, string message, Exception details = null) {
			System.Diagnostics.Trace.WriteLine(string.Format("ERROR [{0}] Origin [{1}] Message [{2}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), origin, message));
			if (details != null) {
				System.Diagnostics.Trace.WriteLine(details);
			}
		}
	}
}
