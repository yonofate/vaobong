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
using System.Web.Security;

namespace Piranha.Security
{
	/// <summary>
	/// Interface that defines the different actions needed for handling user
	/// login, creation and verification.
	/// </summary>
	internal interface IUserProvider
	{
		/// <summary>
		/// Gets if the current user is authenticated.
		/// </summary>
		bool IsAuthenticated { get; }

		/// <summary>
		/// Gets the user id of the current user.
		/// </summary>
		Guid UserId { get; }

		/// <summary>
		/// Get the current user.
		/// </summary>
		//MembershipUser User { get ; }

		/// <summary>
		/// Authenticates the user.
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="password">The password</param>
		/// <returns>If the username and passwords matches</returns>
		bool Authenticate(string username, string password);

		/// <summary>
		/// Creates a new user entity with the given credentials.
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="password">The password</param>
		/// <param name="email">The email address</param>
		/// <returns>The provider user key for the new user</returns>
		// object Create(string username, string password, string email) ;

		/// <summary>
		/// Deletes the user with the given Piranha CMS user id.
		/// </summary>
		/// <param name="id">The Piranha CMS user id</param>
		/// <returns>If the user was deleted successfully</returns>
		// bool Delete(Guid id) ;

		/// <summary>
		/// Deletes the user with the given membership provider key.
		/// </summary>
		/// <param name="id">The membership user id</param>
		/// <returns>If the user was deleted successfully</returns>
		// bool Delete(object id) ;

		/// <summary>
		/// Gets the user with the given Piranha CMS user id.
		/// </summary>
		/// <param name="id">The Piranha CMS user id</param>
		/// <returns>The user</returns>
		// MembershipUser GetUser(Guid id) ;
	}
}
