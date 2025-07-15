using SaveToolbox.Runtime.BasicSaveableMonoBehaviours;
using UnityEngine;

namespace SaveToolbox.Runtime.CustomComponentSavers
{
	public class RigidbodyComponentSaver : AbstractComponentSaver<Rigidbody>
	{
		public override object Serialize()
		{
			return new RigidbodySaveData(Target);
		}

		public override void Deserialize(object data)
		{
			var rigidBodyData = (RigidbodySaveData)data;
			Target.mass = rigidBodyData.Mass;
			Target.useGravity = rigidBodyData.UseGravity;
			Target.isKinematic = rigidBodyData.IsKinematic;
			Target.interpolation = (RigidbodyInterpolation)rigidBodyData.Interpolation;
			Target.collisionDetectionMode = (CollisionDetectionMode)rigidBodyData.CollisionDetection;
			Target.angularVelocity = rigidBodyData.AngularVelocity;
			Target.centerOfMass = rigidBodyData.CentreOfMass;
			Target.constraints = (RigidbodyConstraints)rigidBodyData.RigidBodyConstraints;

#if UNITY_2022_2_OR_NEWER
			Target.automaticCenterOfMass = rigidBodyData.AutomaticCenterOfMass;
			Target.automaticInertiaTensor = rigidBodyData.AutomaticTensor;
			Target.includeLayers = rigidBodyData.IncludeLayers;
			Target.excludeLayers = rigidBodyData.ExcludeLayers;
#endif

#if UNITY_6000_0_OR_NEWER
			Target.linearDamping = rigidBodyData.Drag;
			Target.angularDamping = rigidBodyData.AngularDrag;
			Target.linearVelocity = rigidBodyData.Velocity;
#else
			Target.drag = rigidBodyData.Drag;
			Target.angularDrag = rigidBodyData.AngularDrag;
			Target.velocity = rigidBodyData.Velocity;
#endif
		}
	}
}