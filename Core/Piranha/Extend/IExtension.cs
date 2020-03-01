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

namespace Piranha.Extend
{
	/// <summary>
	/// Interface for all objects that should be treated as extensions to
	/// the core framework.
	/// </summary>
	public interface IExtension
	{
		/// <summary>
		/// This method is called at application start when the ExtensionManager has 
		/// loaded all of the imports. It should be used to perform one time initialization
		/// like setting database tables, adding meta data and so on.
		/// <param name="db">An authenticated data context</param>
		void Ensure(DataContext db);

		/// <summary>
		/// This method is called when then extension is loaded by the client API. The
		/// model provided is the current entity that the extension is attached to. For example
		/// an extension attached to a Post will get a PostModel object as a parameter.
		/// </summary>
		/// <param name="model">The model of the entity</param>
		void Init(object model);

		/// <summary>
		/// This method is called when the extension is loaded by the manager interface. This
		/// usually happens when an entity of the extension type is open for edit in the manager
		/// interface.
		/// </summary>
		/// <param name="model">The edit model of the entity</param>
		void InitManager(object model);

		/// <summary>
		/// Called when the entity the extension is attached to is saved in the manager
		/// interface.
		/// </summary>
		/// <param name="model">The edit model of the entity</param>
		void OnManagerSave(object model);

		/// <summary>
		/// Called when the entity the extension is attached to is deleted in the manager
		/// interface.
		/// </summary>
		/// <param name="model">The edit model of the entity</param>
		void OnManagerDelete(object model);

		/// <summary>
		/// This method is called when the client model tries to populate the data of it's
		/// extensions. This is very useful when the extension data is in fact only meta 
		/// data for retrieving other information.
		/// </summary>
		/// <param name="model">The model of the entity</param>
		/// <returns>The extension data</returns>
		object GetContent(object model);
	}
}
