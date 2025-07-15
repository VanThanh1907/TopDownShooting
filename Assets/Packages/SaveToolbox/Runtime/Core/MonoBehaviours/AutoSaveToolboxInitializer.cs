using UnityEngine;

namespace SaveToolbox.Runtime.Core.MonoBehaviours
{
	[AddComponentMenu("SaveToolbox/Core/AutoSaveToolboxInitializer")]
	public class AutoSaveToolboxInitializer : MonoBehaviour
	{
		private void Awake()
		{
			#if STB_ASYNCHRONOUS_SAVING
			_ =  SaveToolboxSystem.Instance.InitializeAsync();
			#else
			SaveToolboxSystem.Instance.Initialize();
			#endif
		}
	}
}
