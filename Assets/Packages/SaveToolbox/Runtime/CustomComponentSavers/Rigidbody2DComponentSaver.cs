using SaveToolbox.Runtime.BasicSaveableMonoBehaviours;
using UnityEngine;

namespace SaveToolbox.Runtime.CustomComponentSavers
{
	public class Rigidbody2DComponentSaver : AbstractComponentSaver<Rigidbody2D>
	{
		public override object Serialize()
		{
			return new Rigidbody2DSaveData(Target);
		}

		public override void Deserialize(object data)
		{
			var rigidBodyData = (Rigidbody2DSaveData)data;
			Target.bodyType = (RigidbodyType2D)rigidBodyData.RigidBody2DBodyType;
			Target.simulated = rigidBodyData.Simulated;
			Target.useAutoMass = rigidBodyData.UseAutoMass;
			Target.mass = rigidBodyData.Mass;
			Target.interpolation = (RigidbodyInterpolation2D)rigidBodyData.Interpolation;
			Target.collisionDetectionMode = (CollisionDetectionMode2D)rigidBodyData.CollisionDetection;
			Target.angularVelocity = rigidBodyData.AngularVelocity;
			Target.centerOfMass = rigidBodyData.CentreOfMass;
			Target.constraints = (RigidbodyConstraints2D)rigidBodyData.RigidBodyConstraints;

#if UNITY_2022_2_OR_NEWER
			Target.includeLayers = rigidBodyData.IncludeLayers;
			Target.excludeLayers = rigidBodyData.ExcludeLayers;
#endif

#if UNITY_6000_0_OR_NEWER
			Target.linearDamping = rigidBodyData.Drag;
			Target.angularDamping = rigidBodyData.AngularDrag;
			Target.linearVelocity = rigidBodyData.Velocity;
#else
			Target.velocity = rigidBodyData.Velocity;
			Target.drag = rigidBodyData.Drag;
			Target.angularDrag = rigidBodyData.AngularDrag;
			Target.isKinematic = rigidBodyData.IsKinematic;
#endif
		}
	}
}