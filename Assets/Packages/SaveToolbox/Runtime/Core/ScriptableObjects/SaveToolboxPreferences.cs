using System;
using SaveToolbox.Runtime.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

namespace SaveToolbox.Runtime.Core.ScriptableObjects
{
	/// <summary>
	/// A singleton scriptable object that stores all the settings that define how game data should be saved.
	/// </summary>
	[CreateAssetMenu(fileName = "SaveToolboxPreferences", menuName = "SaveToolbox/SaveToolboxPreferences", order = 1)]
	public class SaveToolboxPreferences : StbSingletonScriptableObject<SaveToolboxPreferences>
	{
		private const string ASSET_PATH = "Assets/Resources";
		private const string ASYNCHRONOUS_SAVING_DEFINE = "STB_ASYNCHRONOUS_SAVING";
		private const string USE_ADDRESSABLES_DEFINE = "STB_USE_ADDRESSABLES";

#if STB_HAS_ADDRESSABLES
		public override string AddressableGroup => "SaveToolbox";
#endif

		/// <summary>
		/// The path at which the object should be saved by default.
		/// </summary>
		/// <exception cref="Exception">If it cannot find an asset path.</exception>
		public override string AssetPath
		{
			get
			{
				var scriptDirectoryParentName = ASSET_PATH;
				if (string.IsNullOrEmpty(scriptDirectoryParentName))
				{
					throw new Exception("Could not retrieve asset path.");
				}

				return scriptDirectoryParentName;
			}
		}

		[field: SerializeField]
		public SaveSettings DefaultSaveSettings { get; private set; }

		// Save settings getter properties.
		public string SaveFileName => DefaultSaveSettings.SaveFileName;
		public string RelativeFolderPath => DefaultSaveSettings.RelativeFolderPath;
		public StbFileFormat SaveFileFormat => DefaultSaveSettings.SaveFileFormat;
		public bool JsonPrettyPrint => DefaultSaveSettings.JsonPrettyPrint;
		public StbSerializationSettings SerializationSettings => DefaultSaveSettings.SerializationSettings;
		public StbEncryptionSettings StbEncryptionSettings => DefaultSaveSettings.StbEncryptionSettings;
		public StbCompressionType CompressionType => DefaultSaveSettings.CompressionType;
		public bool RebuildLoadableObjects => DefaultSaveSettings.RebuildLoadableObjects;
		public bool PhysicsSyncTransformsOnLoad => DefaultSaveSettings.PhysicsSyncTransformsOnLoad;
		public bool SaveScene => DefaultSaveSettings.SaveScene;
		public StbSceneSavingType StbSceneSavingType => DefaultSaveSettings.StbSceneSavingType;
		public bool FreezeTimeScaleOnSaveLoad => DefaultSaveSettings.FreezeTimeScaleOnAsynchronousSaveLoad;

		/// <summary>
		/// Should the data be saved asynchronously?
		/// </summary>
		[field: SerializeField]
		private bool asynchronousSaving;
		public bool AsynchronousSaving {
			get => asynchronousSaving;
			set
			{
				asynchronousSaving = value;
				UpdateScriptingDefines();
			}
		}

		/// <summary>
		/// If the data is saved asynchrnously, what is the lowest acceptable frame rate.
		/// </summary>
		[field: SerializeField]
		public int LowestAcceptableLoadingFrameRate { get; set; } = 30;

		/// <summary>
		/// When processes are completed or failed through the save system their are logs, would you like these to be enabled?
		/// </summary>
		[field: SerializeField]
		public bool LoggingEnabled { get; set; } = true;

#if STB_HAS_ADDRESSABLES
		[field: SerializeField]
		private bool useAddressables;
		public bool UseAddressables
		{
			get => useAddressables;
			set
			{
				useAddressables = value;
				UpdateScriptingDefines();
			}
		}

		private bool previousUseAddressables;
#endif

		private bool previousAsynchronousSaving;

		private void Awake()
		{
			previousAsynchronousSaving = asynchronousSaving;

#if STB_HAS_ADDRESSABLES
			previousUseAddressables = useAddressables;
#endif
		}

		private void UpdateScriptingDefines()
		{
#if UNITY_EDITOR
#if UNITY_6000_0_OR_NEWER
			var scriptingDefines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
#else
			var scriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

#if STB_HAS_ADDRESSABLES
			if (UseAddressables)
			{
				if (!scriptingDefines.Contains($";{USE_ADDRESSABLES_DEFINE}") && !scriptingDefines.Contains($"{USE_ADDRESSABLES_DEFINE}"))
				{
					scriptingDefines += $";{USE_ADDRESSABLES_DEFINE}";
				}
			}
			else
			{
				if (scriptingDefines.Contains($";{USE_ADDRESSABLES_DEFINE}"))
				{
					scriptingDefines = scriptingDefines.Replace($";{USE_ADDRESSABLES_DEFINE}", "");
				}

				if (scriptingDefines.Contains($"{USE_ADDRESSABLES_DEFINE}"))
				{
					scriptingDefines = scriptingDefines.Replace($"{USE_ADDRESSABLES_DEFINE}", "");
				}
			}
			previousUseAddressables = useAddressables;
#endif

			if (asynchronousSaving)
			{
				if (!scriptingDefines.Contains($";{ASYNCHRONOUS_SAVING_DEFINE}") && !scriptingDefines.Contains($"{ASYNCHRONOUS_SAVING_DEFINE}"))
				{
					scriptingDefines += $";{ASYNCHRONOUS_SAVING_DEFINE}";
				}
			}
			else
			{
				if (scriptingDefines.Contains($";{ASYNCHRONOUS_SAVING_DEFINE}"))
				{
					scriptingDefines = scriptingDefines.Replace($";{ASYNCHRONOUS_SAVING_DEFINE}", "");
				}

				if (scriptingDefines.Contains($"{ASYNCHRONOUS_SAVING_DEFINE}"))
				{
					scriptingDefines = scriptingDefines.Replace($"{ASYNCHRONOUS_SAVING_DEFINE}", "");
				}
			}

			previousAsynchronousSaving = asynchronousSaving;
#if UNITY_6000_0_OR_NEWER
			PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), scriptingDefines);
#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefines);
#endif
#endif
		}

		private void OnValidate()
		{
#if STB_HAS_ADDRESSABLES
			if (previousAsynchronousSaving != asynchronousSaving || previousUseAddressables != useAddressables)
#else
			if (previousAsynchronousSaving != asynchronousSaving)
#endif
			{
				UpdateScriptingDefines();
			}
		}
	}
}