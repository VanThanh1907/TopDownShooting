using System;
using SaveToolbox.Runtime.Core.MonoBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace SaveToolbox.Runtime.BasicSaveableMonoBehaviours
{
	/// <summary>
	/// Saves data about the UI scrollRect that is referenced.
	/// </summary>
	public class StbScrollRect : SaveableMonoBehaviour
	{
		/// <summary>
		/// The referenced scrollRect.
		/// </summary>
		[SerializeField]
		private ScrollRect scrollRect;

		public override object Serialize()
		{
			if (scrollRect == null)
			{
				if (!TryGetComponent(out scrollRect)) throw new Exception($"Could not serialize object of type scrollRect as there isn't one referenced or attached to the game object.");
			}

			var value = new Vector2(scrollRect.horizontalScrollbar.value, scrollRect.verticalScrollbar.value);
			return value;
		}

		public override void Deserialize(object data)
		{
			if (scrollRect == null)
			{
				if (!TryGetComponent(out scrollRect)) throw new Exception($"Could not deserialize object of type scrollRect as there isn't one referenced or attached to the game object.");
			}
			var scrollRectValue = (Vector2)data;
			scrollRect.horizontalScrollbar.value = scrollRectValue.x;
			scrollRect.verticalScrollbar.value = scrollRectValue.y;
		}
	}
}