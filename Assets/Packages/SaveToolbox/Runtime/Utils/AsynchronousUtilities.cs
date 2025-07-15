using System.Threading.Tasks;
using SaveToolbox.Runtime.Core.ScriptableObjects;
using UnityEngine;

namespace SaveToolbox.Runtime.Utils
{
	public static class AsynchronousUtilities
	{
		private static float currentFrameTime;
		private static float startFrameTime;
		private static int LowestAcceptableLoadingFrameRate => SaveToolboxPreferences.Instance.LowestAcceptableLoadingFrameRate;

		public static async Task CheckFrameTime()
		{
			currentFrameTime = Time.realtimeSinceStartup;
			var difference = currentFrameTime - startFrameTime;
			if (difference > 1f / LowestAcceptableLoadingFrameRate)
			{
				await Task.Yield();
				startFrameTime = Time.realtimeSinceStartup;
			}
		}
	}
}