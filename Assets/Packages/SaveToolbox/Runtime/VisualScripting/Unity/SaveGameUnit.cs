#if STB_HAS_VISUAL_SCRIPTING
using SaveToolbox.Runtime.Core;
using Unity.VisualScripting;

namespace SaveToolbox.Runtime.VisualScripting.Unity
{
	[UnitCategory("SaveToolbox")]
	[UnitTitle("Try Save Game")]
	public class SaveGameUnit : Unit
	{
		[DoNotSerialize, PortLabel("Input")]
		public ControlInput input;

		[DoNotSerialize, PortLabel("Output")]
		public ControlOutput output;

		[Inspectable]
		private int saveSlotIndex;

		protected override void Definition()
		{
			input = ControlInput("Input", SaveGame);
			output = ControlOutput("Output");
		}

		private ControlOutput SaveGame(Flow flow)
		{
			SaveToolboxSystem.Instance.TrySaveGame(saveSlotIndex);
			return output;
		}
	}
}
#endif