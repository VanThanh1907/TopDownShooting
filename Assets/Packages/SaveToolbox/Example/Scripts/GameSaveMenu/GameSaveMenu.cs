using System;
using System.Linq;
using SaveToolbox.Runtime.Core;
using SaveToolbox.Runtime.Core.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace SaveToolbox.Example.Scripts.GameSaveMenu
{
	/// <summary>
	/// A script to handle the saving and loading from a game.
	/// It allows the user to save game data based on name.
	/// If a save exists with a specific name already overwrite.
	/// Data can be loaded by clicking a button related to each save.
	/// </summary>
	public class GameSaveMenu : MonoBehaviour
	{
		[SerializeField]
		private InputField inputField;

		[SerializeField]
		private double saveVersion;

		[SerializeField]
		private Button saveButton;

		[SerializeField]
		private bool hasMaximumNumberOfSlots;

		[SerializeField]
		private int maxSlots = 3;

		[SerializeField]
		private Transform loadButtonsParent;

		[SerializeField]
		private GameSaveLoadButton loadButtonPrefab;

		[SerializeField]
		private GameObject outOfSlotsWarning;

		private void OnEnable()
		{
			saveButton.onClick.AddListener(SaveData);
			UpdateState();
		}

		private
#if STB_ASYNCHRONOUS_SAVING
			async
#endif
			void SaveData()
		{
			var saveSettings = SaveToolboxPreferences.Instance.DefaultSaveSettings.Copy();

			// If we can't get the slot index, return and fail.
			if (!TryGetNextSlot(out var slotIndex))
			{
				saveButton.interactable = false;
				return;
			}

			var metaData = new BasicGameMetaData(slotIndex, inputField != null ? inputField.text : "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), saveVersion);

#if STB_ASYNCHRONOUS_SAVING
#pragma warning disable CS4014
			await SaveToolboxSystem.Instance.TrySaveGameAsync(metaData, saveSettings, slotIndex);
#pragma warning restore CS4014
#else
			SaveToolboxSystem.Instance.TrySaveGame(metaData, saveSettings, slotIndex);
#endif
			UpdateState();
		}

		private void UpdateState()
		{
			try
			{
				var saveSettings = SaveToolboxPreferences.Instance.DefaultSaveSettings.Copy();
				var fileSettings = new FileSaveSettings(saveSettings);
				var basicGameMetaDatas = SaveToolboxSystem.Instance.LoadAllSaveGameMetaDatas<BasicGameMetaData>(fileSettings, false);

				for (var i = loadButtonsParent.childCount - 1; i >= 0; i--)
				{
					var child = loadButtonsParent.GetChild(i);
					if (child.TryGetComponent<GameSaveLoadButton>(out var gameSaveLoadButton))
					{
						gameSaveLoadButton.OnLoad -= HandleButtonLoaded;
						gameSaveLoadButton.OnDelete -= HandleButtonDeleted;
						Destroy(child.gameObject);
					}
				}

				foreach (var basicGameMetaData in basicGameMetaDatas)
				{
					var loadButton = Instantiate(loadButtonPrefab, loadButtonsParent);
					loadButton.OnLoad += HandleButtonLoaded;
					loadButton.OnDelete += HandleButtonDeleted;
					loadButton.Initialize(basicGameMetaData);
				}

				if (hasMaximumNumberOfSlots)
				{
					saveButton.interactable = basicGameMetaDatas.Length < maxSlots;
					if (outOfSlotsWarning != null)
					{
						outOfSlotsWarning.gameObject.SetActive(basicGameMetaDatas.Length >= maxSlots);
					}
				}
				else
				{
					saveButton.interactable = true;
				}
			}
			catch
			{
				// ignore.
			}
		}

		private void HandleButtonLoaded() => UpdateState();
		private void HandleButtonDeleted() => UpdateState();

		private bool TryGetNextSlot(out int slotIndex)
		{
			slotIndex = 0;

			var saveSettings = SaveToolboxPreferences.Instance.DefaultSaveSettings.Copy();
			var fileSettings = new FileSaveSettings(saveSettings);
			var basicGameMetaDatas = SaveToolboxSystem.Instance.LoadAllSaveGameMetaDatas<BasicGameMetaData>(fileSettings, false).ToList();

			if (hasMaximumNumberOfSlots && basicGameMetaDatas.Count >= maxSlots) return false;

			basicGameMetaDatas.Sort((data1, data2) => data1.SlotIndex.CompareTo(data2.SlotIndex));

			// Find the next free slot.
			for (var i = 0; i < basicGameMetaDatas.Count; ++i)
			{
				if (basicGameMetaDatas[i].SlotIndex == i)
				{
					slotIndex = i + 1;
				}
				else
				{
					break;
				}
			}

			return true;
		}

		private void OnDisable()
		{
			saveButton.onClick.RemoveListener(SaveData);
		}
	}

	[Serializable]
	public class BasicGameMetaData : SaveMetaData
	{
		[SerializeField]
		private int slotIndex;
		public int SlotIndex => slotIndex;

		[SerializeField]
		private string saveName;
		public string SaveName => saveName;

		[SerializeField]
		private string saveTime;
		public string SaveTime => saveTime;

		public BasicGameMetaData() {}

		public BasicGameMetaData(int slotIndex, string saveName, string saveTime, double saveVersion = 0.1, object metaData = null) : base(saveVersion, metaData)
		{
			this.slotIndex = slotIndex;
			this.saveName = saveName;
			this.saveTime = saveTime;
		}
	}
}
