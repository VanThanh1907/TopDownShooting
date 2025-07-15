using System;
using SaveToolbox.Runtime.Core.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace SaveToolbox.Runtime.Core
{
#if UNITY_EDITOR
	/// <summary>
	/// A script to ensure the scriptable object singletons for the package are create in a resource folder.
	/// Checks if they exist and if not creates them.
	/// </summary>
	[InitializeOnLoad]
	public class ScriptLibraryInitializer
	{
		static ScriptLibraryInitializer()
		{
			// Subscribe to callback to initialize scriptable objects, because we can't do it until domain is fully reloaded.
			EditorApplication.delayCall += HandleEditorDelayCall;
		}

		private static
#if STB_USE_ADDRESSABLES
			async
#endif
			void HandleEditorDelayCall()
		{
			// Get the instances to auto-create the files.
			try
			{
				SaveToolboxPreferences saveToolboxPreferences = null;
#if STB_USE_ADDRESSABLES
				saveToolboxPreferences = await SaveToolboxPreferences.GetInstanceAsync();
#else
				saveToolboxPreferences = SaveToolboxPreferences.Instance;
#endif
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			try
			{
				LoadableObjectDatabase loadableObjectDatabase = null;
#if STB_USE_ADDRESSABLES
				loadableObjectDatabase = await LoadableObjectDatabase.GetInstanceAsync();
#else
				loadableObjectDatabase = LoadableObjectDatabase.Instance;
#endif
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			try
			{
				ScriptableObjectDatabase scriptableObjectDatabase = null;
#if STB_USE_ADDRESSABLES
				scriptableObjectDatabase = await ScriptableObjectDatabase.GetInstanceAsync();
#else
				scriptableObjectDatabase = ScriptableObjectDatabase.Instance;
#endif
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}
#endif
}
