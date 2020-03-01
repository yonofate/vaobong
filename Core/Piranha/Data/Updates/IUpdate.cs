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
using System.Data;

namespace Piranha.Data.Updates
{
	/// <summary>
	/// Interface for defining an update.
	/// </summary>
	public interface IUpdate
	{
		/// <summary>
		/// Executes the current update.
		/// </summary>
		void Execute(IDbTransaction tx);
	}
}