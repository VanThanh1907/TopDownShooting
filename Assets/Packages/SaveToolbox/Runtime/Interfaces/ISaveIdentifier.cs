using System;

namespace SaveToolbox.Runtime.Interfaces
{
	public interface ISaveIdentifier
	{
		/// <summary>
		/// A string to identify the ISaveDataEntity when it comes to deserializing again.
		/// </summary>
		string SaveIdentifier { get; set; }

#if UNITY_2021_3_OR_NEWER
		/// <summary>
		/// Create a new randomly generated identifier for the ISaveDataEntity. Uses a string representation of a Guid.
		/// </summary>
		public void GenerateNewIdentifier()
		{
			SaveIdentifier = Guid.NewGuid().ToString();
		}
#endif
	}
}