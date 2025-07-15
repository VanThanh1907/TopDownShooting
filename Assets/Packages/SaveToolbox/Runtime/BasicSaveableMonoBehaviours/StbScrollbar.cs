using System;
using SaveToolbox.Runtime.Core.MonoBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace SaveToolbox.Runtime.BasicSaveableMonoBehaviours
{
	/// <summary>
	/// Saves data about the UI scrollbar that is referenced.
	/// </summary>
	[AddComponentMenu("SaveToolbox/SavingBehaviours/StbScrollbar")]
	public class StbScrollbar : SaveableMonoBehaviour
	{
		/// <summary>
		/// The referenced scrollbar.
		/// </summary>
		[SerializeField]
		private Scrollbar scrollbar;

		public override object Serialize()
		{
			if (scrollbar == null)
			{
				if (!TryGetComponent(out scrollbar)) throw new Exception($"Could not serialize object of type scrollbar as there isn't one referenced or attached to the game object.");
			}
			var scrollbarValue = scrollbar.value;
			return scrollbarValue;
		}

		public override void Deserialize(object data)
		{
			if (scrollbar == null)
			{
				if (!TryGetComponent(out scrollbar)) throw new Exception($"Could not deserialize object of type scrollbar as there isn't one referenced or attached to the game object.");
			}
			var scrollbarValue = (float)data;
			scrollbar.value = scrollbarValue;
		}
	}
}