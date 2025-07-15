
using System;
using System.Collections.Generic;
using SaveToolbox.Runtime.Attributes;
using SaveToolbox.Runtime.Core.MonoBehaviours;
using UnityEngine;

namespace SaveToolbox.Runtime.BasicSaveableMonoBehaviours
{
	[AddComponentMenu("SaveToolbox/SavingBehaviours/StbAnimator")]
	public class StbAnimator : SaveableMonoBehaviour
	{
		[SerializeField]
		private Animator animator;

		public override object Serialize()
		{
			if (animator == null)
			{
				if (!TryGetComponent(out animator)) throw new Exception($"Could not serialize object of type Animator as there isn't one referenced or attached to the game object.");
			}

			var animatorSaveData = new AnimatorSaveData(animator);
			return animatorSaveData;
		}

		public override void Deserialize(object data)
		{
			var animatorSaveData = (AnimatorSaveData)data;

			for (var index = 0; index < animatorSaveData.AnimatorStateSaveData.Length; index++)
			{
				var animatorStateSaveData =  animatorSaveData.AnimatorStateSaveData[index];
				animator.Play(animatorStateSaveData.StateHash, index, animatorStateSaveData.NormalizedTime);
			}

			for (var i = 0; i < animator.parameters.Length; ++i)
			{
				var animatorControllerParameter = animator.parameters[i];
				var parameterData = animatorSaveData.Parameters[i];
				switch (animatorControllerParameter.type)
				{
					case AnimatorControllerParameterType.Float:
						animator.SetFloat(parameterData.ParameterKey, parameterData.FloatValue);
						break;
					case AnimatorControllerParameterType.Int:
						animator.SetInteger(parameterData.ParameterKey, parameterData.IntValue);
						break;
					case AnimatorControllerParameterType.Bool:
						animator.SetBool(parameterData.ParameterKey, parameterData.BoolValue);
						break;
					case AnimatorControllerParameterType.Trigger:
						if (parameterData.WasTriggered)
						{
							animator.SetTrigger(parameterData.ParameterKey);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}

	[Serializable]
	public struct AnimatorSaveData
	{
		[SerializeField, StbSerialize]
		private AnimatorStateSaveData[] animatorStateSaveData;
		public AnimatorStateSaveData[] AnimatorStateSaveData => animatorStateSaveData;

		[SerializeField, StbSerialize]
		private List<AnimatorParameterSaveData> parameters;
		public List<AnimatorParameterSaveData> Parameters => parameters;

		public AnimatorSaveData(Animator animator)
		{
			var layerCount = animator.layerCount;
			parameters = new List<AnimatorParameterSaveData>();
			animatorStateSaveData = new AnimatorStateSaveData[layerCount];
			for (var i = 0; i < layerCount; ++i)
			{
				var currentState = animator.GetCurrentAnimatorStateInfo(i);
				animatorStateSaveData[i] = new AnimatorStateSaveData(currentState.fullPathHash, currentState.normalizedTime);
			}

			foreach (var animatorControllerParameter in animator.parameters)
			{
				switch (animatorControllerParameter.type)
				{
					case AnimatorControllerParameterType.Float:
						parameters.Add(new AnimatorParameterSaveData(animatorControllerParameter.name, floatValue: animator.GetFloat(animatorControllerParameter.name)));
						break;
					case AnimatorControllerParameterType.Int:
						parameters.Add(new AnimatorParameterSaveData(animatorControllerParameter.name, intValue: animator.GetInteger(animatorControllerParameter.name)));
						break;
					case AnimatorControllerParameterType.Bool:
						parameters.Add(new AnimatorParameterSaveData(animatorControllerParameter.name, boolValue: animator.GetBool(animatorControllerParameter.name)));
						break;
					case AnimatorControllerParameterType.Trigger:
						parameters.Add(new AnimatorParameterSaveData(animatorControllerParameter.name, wasTriggered: animator.GetBool(animatorControllerParameter.name)));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}

	[Serializable]
	public struct AnimatorStateSaveData
	{
		[SerializeField, StbSerialize]
		private int stateHash;
		public int StateHash => stateHash;

		[SerializeField, StbSerialize]
		private float normalizedTime;
		public float NormalizedTime => normalizedTime;

		public AnimatorStateSaveData(int stateHash, float normalizedTime)
		{
			this.stateHash = stateHash;
			this.normalizedTime = normalizedTime;
		}
	}

	[Serializable]
	public struct AnimatorParameterSaveData
	{
		[SerializeField, StbSerialize]
		private string parameterKey;
		public string ParameterKey => parameterKey;

		[SerializeField, StbSerialize]
		private int intValue;
		public int IntValue => intValue;

		[SerializeField, StbSerialize]
		private float floatValue;
		public float FloatValue => floatValue;

		[SerializeField, StbSerialize]
		private bool boolValue;
		public bool BoolValue => boolValue;

		[SerializeField, StbSerialize]
		private bool wasTriggered;
		public bool WasTriggered => wasTriggered;

		public AnimatorParameterSaveData(string parameterKey, int intValue = 0, float floatValue = 0f, bool boolValue = false, bool wasTriggered = false)
		{
			this.parameterKey = parameterKey;
			this.intValue = intValue;
			this.floatValue = floatValue;
			this.boolValue = boolValue;
			this.wasTriggered = wasTriggered;
		}
	}
}
