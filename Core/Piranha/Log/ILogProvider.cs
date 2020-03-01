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
using System.Text;
using System.Threading.Tasks;

namespace Piranha.Log
{
	public interface ILogProvider
	{
		/// <summary>
		/// Logs an error to the log file.
		/// </summary>
		/// <param name="origin">The origin of the error</param>
		/// <param name="message">The message</param>
		/// <param name="details">Optional error details</param>
		void Error(string origin, string message, Exception details = null);

	}
}
