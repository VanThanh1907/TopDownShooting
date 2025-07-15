#if STB_HAS_TMP
using System;
using SaveToolbox.Runtime.Core;
using TMPro;
using UnityEngine.UI;
#endif
using UnityEngine;

namespace SaveToolbox.Example.Scripts.GameSaveMenu
{
	public class GameSaveLoadButtonTmp : MonoBehaviour
	{
#if STB_HAS_TMP
		[SerializeField]
		private Button loadButton;

		[SerializeField]
		private TextMeshProUGUI loadName;

		[SerializeField]
		private TextMeshProUGUI dateText;

		[SerializeField]
		private TextMeshProUGUI slotIndex;

		[SerializeField]
		private Button deleteButton;

		private BasicGameMetaData currentMetaData;

		public event Action OnLoad;
		public event Action OnDelete;

		private void OnEnable()
		{
			loadButton.onClick.AddListener(LoadSave);
			deleteButton.onClick.AddListener(DeleteSave);
		}

		public void Initialize(BasicGameMetaData metaData)
		{
			currentMetaData = metaData;
			if (loadName != null)
			{
				loadName.text = currentMetaData.SaveName;
			}

			if (dateText != null)
			{
				dateText.text = currentMetaData.SaveTime;
			}

			if (slotIndex != null)
			{
				slotIndex.text = currentMetaData.SlotIndex.ToString();
			}
		}

		private void LoadSave()
		{
#if STB_ASYNCHRONOUS_SAVING
#pragma warning disable CS4014
			SaveToolboxSystem.Instance.TryLoadGameAsync(currentMetaData.SlotIndex);
#pragma warning restore CS4014
#else
			SaveToolboxSystem.Instance.TryLoadGame(currentMetaData.SlotIndex);
#endif
			OnLoad?.Invoke();
		}

		private void DeleteSave()
		{
			SaveToolboxSystem.Instance.TryDeleteSlot(currentMetaData.SlotIndex);
			OnDelete?.Invoke();
		}

		private void OnDisable()
		{
			loadButton.onClick.RemoveListener(LoadSave);
			deleteButton.onClick.RemoveListener(DeleteSave);
		}
#endif
	}
}