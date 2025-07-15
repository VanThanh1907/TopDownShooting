using SaveToolbox.Runtime.Core.ScriptableObjects;
using UnityEngine;

namespace SaveToolbox.Runtime.Core.MonoBehaviours
{
	public class AutoAddressableInstanceInitializer : MonoBehaviour
	{
#if STB_USE_ADDRESSABLES
		private async void Awake()
		{
			var saveToolboxPreferences = await SaveToolboxPreferences.GetInstanceAsync();
			var loadableObjectDatabase = await LoadableObjectDatabase.GetInstanceAsync();
			var scriptableObjectDatabase = await ScriptableObjectDatabase.GetInstanceAsync();
		}
#endif
	}
}
