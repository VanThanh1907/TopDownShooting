using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if STB_USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
#endif
#endif

namespace SaveToolbox.Runtime.Utils
{
	/// <summary>
	/// A scriptable object that is also a singleton. Should only ever be 1 in a project. Will auto create itself
	/// on get of an instance. Uses resources folder to function. Unless using addressables.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StbSingletonScriptableObject<T> : ScriptableObject, IAssetPathProvider where T : ScriptableObject
	{
#if STB_USE_ADDRESSABLES
		private const string ADDRESSABLE_SETTINGS_PATH = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
#endif


		public abstract string AssetPath { get; }
#if STB_HAS_ADDRESSABLES
		public abstract string AddressableGroup { get; }
#endif

		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
#if STB_USE_ADDRESSABLES
					Debug.LogError($"[SaveToolbox] Singleton instance for object of type: {typeof(T).Name} is NULL. You are using Addressables please ensure to call GetInstanceAsync before trying to get the instance.");
#else

					var path = typeof(T).Name;
					var singletonObject = Resources.Load<T>(path);
					if (singletonObject == null)
					{
						Debug.Log($"[SaveToolbox] Could not find singleton object of type: {typeof(T)}");
					}
					else
					{
						instance = singletonObject;
					}

#if UNITY_EDITOR
					if (instance == null && !Application.isPlaying) // If it still is null after trying to retrieve it and we're not in play mode, create a new instance.
					{
						instance = CreateSingletonInstance();
					}
#endif
#endif
				}
				return instance;
			}
		}

#if UNITY_EDITOR
		private static T CreateSingletonInstance()
		{
			Debug.Log($"[SaveToolbox] Could not find singleton Scriptable Object of type: {typeof(T)}, attempting to create one.");

			var scriptableObject = CreateInstance<T>();
			var assetPathProvider = scriptableObject as IAssetPathProvider;
			if (assetPathProvider == null)
			{
				throw new Exception("Created instance is not a IAssetPathProvider");
			}

			var assetSavePath = $"{assetPathProvider.AssetPath}/{typeof(T).Name}.Asset";
			if (!Directory.Exists(assetPathProvider.AssetPath))
			{
				Directory.CreateDirectory(assetPathProvider.AssetPath);
			}

			// Check if anything is in the expected path first.
			var loadedAsset = AssetDatabase.LoadAssetAtPath<T>(assetSavePath);

			if (loadedAsset == null)
			{
				var hasDeletedAssetAtTargetPath = AssetDatabase.DeleteAsset(assetSavePath);
				if (hasDeletedAssetAtTargetPath)
				{
					Debug.Log($"[SaveToolbox] Found existing asset at path: {assetSavePath}, attempting to delete it. This could be because of a broken asset of type {typeof(T)}");
				}
				AssetDatabase.CreateAsset(scriptableObject, assetSavePath);

				if (scriptableObject != null && scriptableObject is ISingletonScriptableObjectInstantiationHandler scriptableObjectInstantiationHandler)
				{
					scriptableObjectInstantiationHandler.HandleInstantiation();
				}
				Debug.Log($"[SaveToolbox] Could not find singleton scriptable object of type {typeof(T)}. Successfully created one in a resources folder at path {assetSavePath}.");
			}
			else
			{
				scriptableObject = loadedAsset;
			}

#if STB_USE_ADDRESSABLES
			TryCreateAddressableGroup(assetPathProvider.AddressableGroup, scriptableObject);
#endif

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return scriptableObject;
		}

#if STB_USE_ADDRESSABLES
		private static bool TryCreateAddressableGroup(string groupKey, Object asset)
		{
			// Create an addressables entry.
			var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

			if (addressableSettings == null)
			{
				if (TryCreateAddressableSettings())
				{
					addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
				}
				else
				{
					return false;
				}
			}

			var assetGroup = addressableSettings.FindGroup(groupKey);

			if (assetGroup == null)
			{
				assetGroup = addressableSettings.CreateGroup(groupKey, false, false, false, new List<AddressableAssetGroupSchema>());
				assetGroup.AddSchema<BundledAssetGroupSchema>();
				assetGroup.AddSchema<ContentUpdateGroupSchema>();
			}

			if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _)) return false;
			if (assetGroup.GetAssetEntry(guid) != null) return true; // Already has the entry? return.

			var entry = addressableSettings.CreateOrMoveEntry(guid, assetGroup);
			entry.address = typeof(T).Name;

			addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
			AssetDatabase.SaveAssets();
			return true;
		}

		private static bool TryCreateAddressableSettings()
		{
			if (AddressableAssetSettingsDefaultObject.Settings != null) return true;

			Debug.Log("[SaveToolbox] Default addressable asset settings is null, attempting to initialize addressable asset settings.");

			if (!Directory.Exists(ADDRESSABLE_SETTINGS_PATH))
			{
				Directory.CreateDirectory(ADDRESSABLE_SETTINGS_PATH);
			}

			var newSettings = AddressableAssetSettings.Create(ADDRESSABLE_SETTINGS_PATH, "AddressableAssetSettings", true, true);

			if (newSettings == null) return false;

			AddressableAssetSettingsDefaultObject.Settings = newSettings;
			EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return true;
		}
#endif
#endif

#if STB_USE_ADDRESSABLES
		public static async Task<T> GetInstanceAsync()
		{
			if (instance != null) return instance; // Already have an instance? return that.
			if (!typeof(T).ImplementsType(typeof(IAssetPathProvider))) return default; // Doesn't implement IAssetPathProvider, use that.

			var shouldCreateInstance = true;

			var asyncOperationHandle = Addressables.LoadAssetAsync<T>(typeof(T).Name);
			await asyncOperationHandle.Task;
			shouldCreateInstance = asyncOperationHandle.Result == null;
			if (!shouldCreateInstance)
			{
				instance = asyncOperationHandle.Result;
			}

#if UNITY_EDITOR
			if (shouldCreateInstance && !Application.isPlaying) // If it still is null after trying to retrieve it and we're not in play mode, create a new instance.
			{
				instance = CreateSingletonInstance();
			}
			if (instance != null)
			{
				if (instance is IAssetPathProvider assetPathProvider)
				{
					TryCreateAddressableGroup(assetPathProvider.AddressableGroup, instance);
				}
			}
#endif

			return instance;
		}

		public static void ReleaseInstance()
		{
			if (instance == null) return;

			Addressables.Release(instance);
			instance = null;
		}
#endif
	}

	public interface IAssetPathProvider
	{
		string AssetPath { get; }

#if STB_HAS_ADDRESSABLES
		string AddressableGroup { get; }
#endif
	}

	public interface ISingletonScriptableObjectInstantiationHandler
	{
		void HandleInstantiation();
	}
}