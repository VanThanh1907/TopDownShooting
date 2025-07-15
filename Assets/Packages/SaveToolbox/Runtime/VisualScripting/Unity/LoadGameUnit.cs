#if STB_HAS_VISUAL_SCRIPTING
using SaveToolbox.Runtime.Core;
using Unity.VisualScripting;

namespace SaveToolbox.Runtime.VisualScripting.Unity
{
	[UnitCategory("SaveToolbox")]
	[UnitTitle("Try Load Game")]
	public class LoadGameUnity : Unit
	{
		[DoNotSerialize, PortLabel("Input")]
		public ControlInput input;

		[DoNotSerialize, PortLabel("Output")]
		public ControlOutput output;

		[Inspectable]
		private int saveSlotIndex;

		protected override void Definition()
		{
			input = ControlInput("Input", LoadGame);
			output = ControlOutput("Output");
		}

		private ControlOutput LoadGame(Flow flow)
		{
			SaveToolboxSystem.Instance.TryLoadGame(saveSlotIndex);
			return output;
		}
	}
}
#endif