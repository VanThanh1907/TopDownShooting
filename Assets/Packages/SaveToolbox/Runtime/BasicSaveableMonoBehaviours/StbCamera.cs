using System;
using SaveToolbox.Runtime.Attributes;
using SaveToolbox.Runtime.Core.MonoBehaviours;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaveToolbox.Runtime.BasicSaveableMonoBehaviours
{
	[AddComponentMenu("SaveToolbox/SavingBehaviours/StbCamera")]
	public class StbCamera : SaveableMonoBehaviour
	{
		[SerializeField]
		private Camera targetCamera;

		public override object Serialize()
		{
			if (targetCamera == null)
			{
				if (!TryGetComponent(out targetCamera)) throw new Exception($"Could not serialize object of type targetCamera as there isn't one referenced or attached to the game object.");
			}

			return new CameraSaveData(targetCamera);
		}

		public override void Deserialize(object data)
		{
			if (targetCamera == null)
			{
				if (!TryGetComponent(out targetCamera)) throw new Exception($"Could not deserialize object of type targetCamera as there isn't one referenced or attached to the game object.");
			}

			var cameraSaveData = (CameraSaveData)data;
#if UNITY_2022_2_OR_NEWER
			targetCamera.anamorphism = cameraSaveData.Anamorphism;
			targetCamera.aperture = cameraSaveData.Aperture;
			targetCamera.curvature = cameraSaveData.Curvature;
			targetCamera.iso = cameraSaveData.Iso;
			targetCamera.barrelClipping = cameraSaveData.BarrelClipping;
			targetCamera.bladeCount = cameraSaveData.BladeCount;
			targetCamera.focusDistance = cameraSaveData.FocusDistance;
			targetCamera.shutterSpeed = cameraSaveData.ShutterSpeed;
#endif

			targetCamera.aspect = cameraSaveData.Aspect;
			targetCamera.depth = cameraSaveData.Depth;
			targetCamera.orthographic = cameraSaveData.Orthographic;
			targetCamera.backgroundColor = cameraSaveData.BackgroundColor;
			targetCamera.cameraType = cameraSaveData.CameraType;
			targetCamera.clearFlags = cameraSaveData.CameraClearFlags;
			targetCamera.cullingMask = cameraSaveData.CullingMask;
			targetCamera.eventMask = cameraSaveData.EventMask;
			targetCamera.focalLength = cameraSaveData.FocalLength;
			targetCamera.gateFit = cameraSaveData.GateFitMode;
			targetCamera.lensShift = cameraSaveData.LensShift;
			targetCamera.orthographicSize = cameraSaveData.OrthographicSize;
			targetCamera.sensorSize = cameraSaveData.SensorSize;
			targetCamera.stereoConvergence = cameraSaveData.StereoConvergence;
			targetCamera.stereoSeparation = cameraSaveData.StereoSeparation;
			targetCamera.targetDisplay = cameraSaveData.TargetDisplay;
			targetCamera.allowDynamicResolution = cameraSaveData.AllowDynamicResolution;
			targetCamera.farClipPlane = cameraSaveData.FarClipPlane;
			targetCamera.fieldOfView = cameraSaveData.FieldOfView;
			targetCamera.layerCullDistances = cameraSaveData.LayerCullDistances;
			targetCamera.layerCullSpherical = cameraSaveData.LayerCullSpherical;
			targetCamera.nearClipPlane = cameraSaveData.NearClipPlane;
			targetCamera.opaqueSortMode = cameraSaveData.OpaqueSortMode;
			targetCamera.stereoTargetEye = cameraSaveData.StereoTargetEyeMask;
			targetCamera.transparencySortAxis = cameraSaveData.TransparencySortAxis;
			targetCamera.useOcclusionCulling = cameraSaveData.UseOcclusionCulling;
			targetCamera.usePhysicalProperties = cameraSaveData.UsePhysicalProperties;
			targetCamera.allowHDR = cameraSaveData.AllowHdr;
			targetCamera.forceIntoRenderTexture = cameraSaveData.ForceIntoRenderTexture;
			targetCamera.allowMSAA = cameraSaveData.AllowMsaa;
			targetCamera.clearStencilAfterLightingPass = cameraSaveData.ClearStencilAfterLightingPass;
		}
	}

	[Serializable]
	public struct CameraSaveData
	{
		[SerializeField, StbSerialize]
		private float anamorphism;
		public float Anamorphism => anamorphism;

		[SerializeField, StbSerialize]
		private float aperture;
		public float Aperture => aperture;

		[SerializeField, StbSerialize]
		private float aspect;
		public float Aspect => aspect;

		[SerializeField, StbSerialize]
		private Vector2 curvature;
		public Vector2 Curvature => curvature;

		[SerializeField, StbSerialize]
		private float depth;
		public float Depth => depth;

		[SerializeField, StbSerialize]
		private int iso;
		public int Iso => iso;

		[SerializeField, StbSerialize]
		private bool orthographic;
		public bool Orthographic => orthographic;

		[SerializeField, StbSerialize]
		private Color backgroundColor;
		public Color BackgroundColor => backgroundColor;

		[SerializeField, StbSerialize]
		private float barrelClipping;
		public float BarrelClipping => barrelClipping;

		[SerializeField, StbSerialize]
		private int bladeCount;
		public int BladeCount => bladeCount;

		[SerializeField, StbSerialize]
		private CameraType cameraType;
		public CameraType CameraType => cameraType;

		[SerializeField, StbSerialize]
		private CameraClearFlags cameraClearFlags;
		public CameraClearFlags CameraClearFlags => cameraClearFlags;

		[SerializeField, StbSerialize]
		private int cullingMask;
		public int CullingMask => cullingMask;

		[SerializeField, StbSerialize]
		private int eventMask;
		public int EventMask => eventMask;

		[SerializeField, StbSerialize]
		private float focalLength;
		public float FocalLength => focalLength;

		[SerializeField, StbSerialize]
		private float focusDistance;
		public float FocusDistance => focusDistance;

		[SerializeField, StbSerialize]
		private Camera.GateFitMode gateFitMode;
		public Camera.GateFitMode GateFitMode => gateFitMode;

		[SerializeField, StbSerialize]
		private Vector2 lensShift;
		public Vector2 LensShift => lensShift;

		[SerializeField, StbSerialize]
		private float orthographicSize;
		public float OrthographicSize => orthographicSize;

		[SerializeField, StbSerialize]
		private Vector2 sensorSize;
		public Vector2 SensorSize => sensorSize;

		[SerializeField, StbSerialize]
		private float shutterSpeed;
		public float ShutterSpeed => shutterSpeed;

		[SerializeField, StbSerialize]
		private float stereoConvergence;
		public float StereoConvergence => stereoConvergence;

		[SerializeField, StbSerialize]
		private float stereoSeparation;
		public float StereoSeparation => stereoSeparation;

		[SerializeField, StbSerialize]
		private int targetDisplay;
		public int TargetDisplay => targetDisplay;

		[SerializeField, StbSerialize]
		private bool allowDynamicResolution;
		public bool AllowDynamicResolution => allowDynamicResolution;

		[SerializeField, StbSerialize]
		private float farClipPlane;
		public float FarClipPlane => farClipPlane;

		[SerializeField, StbSerialize]
		private float fieldOfView;
		public float FieldOfView => fieldOfView;

		[SerializeField, StbSerialize]
		private float[] layerCullDistances;
		public float[] LayerCullDistances => layerCullDistances;

		[SerializeField, StbSerialize]
		private bool layerCullSpherical;
		public bool LayerCullSpherical => layerCullSpherical;

		[SerializeField, StbSerialize]
		private float nearClipPlane;
		public float NearClipPlane => nearClipPlane;

		[SerializeField, StbSerialize]
		private OpaqueSortMode opaqueSortMode;
		public OpaqueSortMode OpaqueSortMode => opaqueSortMode;

		[SerializeField, StbSerialize]
		private StereoTargetEyeMask stereoTargetEyeMask;
		public StereoTargetEyeMask StereoTargetEyeMask => stereoTargetEyeMask;

		[SerializeField, StbSerialize]
		private Vector3 transparencySortAxis;
		public Vector3 TransparencySortAxis => transparencySortAxis;

		[SerializeField, StbSerialize]
		private bool useOcclusionCulling;
		public bool UseOcclusionCulling => useOcclusionCulling;

		[SerializeField, StbSerialize]
		private bool usePhysicalProperties;
		public bool UsePhysicalProperties => usePhysicalProperties;

		[SerializeField, StbSerialize]
		private bool allowHdr;
		public bool AllowHdr => allowHdr;

		[SerializeField, StbSerialize]
		private bool forceIntoRenderTexture;
		public bool ForceIntoRenderTexture => forceIntoRenderTexture;

		[SerializeField, StbSerialize]
		private bool allowMsaa;
		public bool AllowMsaa => allowMsaa;

		[SerializeField, StbSerialize]
		private bool clearStencilAfterLightingPass;
		public bool ClearStencilAfterLightingPass => clearStencilAfterLightingPass;

		public CameraSaveData(float anamorphism, float aperture, float aspect, Vector2 curvature, float depth, int iso,
			bool orthographic, Color backgroundColor, float barrelClipping, int bladeCount, CameraType cameraType,
			CameraClearFlags cameraClearFlags, int cullingMask, int eventMask, float focalLength, float focusDistance,
			Camera.GateFitMode gateFitMode, Vector2 lensShift, float orthographicSize, Vector2 sensorSize,
			float shutterSpeed, float stereoConvergence, float stereoSeparation, int targetDisplay,
			bool allowDynamicResolution, float farClipPlane, float fieldOfView, float[] layerCullDistances,
			bool layerCullSpherical, float nearClipPlane, OpaqueSortMode opaqueSortMode,
			StereoTargetEyeMask stereoTargetEyeMask, Vector3 transparencySortAxis, bool useOcclusionCulling,
			bool usePhysicalProperties, bool allowHdr, bool forceIntoRenderTexture, bool allowMsaa,
			bool clearStencilAfterLightingPass)
		{
			this.anamorphism = anamorphism;
			this.aperture = aperture;
			this.curvature = curvature;
			this.iso = iso;
			this.barrelClipping = barrelClipping;
			this.bladeCount = bladeCount;
			this.focusDistance = focusDistance;
			this.shutterSpeed = shutterSpeed;
			this.aspect = aspect;
			this.depth = depth;
			this.orthographic = orthographic;
			this.backgroundColor = backgroundColor;
			this.cameraType = cameraType;
			this.cameraClearFlags = cameraClearFlags;
			this.cullingMask = cullingMask;
			this.eventMask = eventMask;
			this.focalLength = focalLength;
			this.gateFitMode = gateFitMode;
			this.lensShift = lensShift;
			this.orthographicSize = orthographicSize;
			this.sensorSize = sensorSize;
			this.stereoConvergence = stereoConvergence;
			this.stereoSeparation = stereoSeparation;
			this.targetDisplay = targetDisplay;
			this.allowDynamicResolution = allowDynamicResolution;
			this.farClipPlane = farClipPlane;
			this.fieldOfView = fieldOfView;
			this.layerCullDistances = layerCullDistances;
			this.layerCullSpherical = layerCullSpherical;
			this.nearClipPlane = nearClipPlane;
			this.opaqueSortMode = opaqueSortMode;
			this.stereoTargetEyeMask = stereoTargetEyeMask;
			this.transparencySortAxis = transparencySortAxis;
			this.useOcclusionCulling = useOcclusionCulling;
			this.usePhysicalProperties = usePhysicalProperties;
			this.allowHdr = allowHdr;
			this.forceIntoRenderTexture = forceIntoRenderTexture;
			this.allowMsaa = allowMsaa;
			this.clearStencilAfterLightingPass = clearStencilAfterLightingPass;
		}

		public CameraSaveData(Camera camera)
		{
#if UNITY_2022_2_OR_NEWER
			anamorphism = camera.anamorphism;
			aperture = camera.aperture;
			curvature = camera.curvature;
			iso = camera.iso;
			barrelClipping = camera.barrelClipping;
			bladeCount = camera.bladeCount;
			focusDistance = camera.focusDistance;
			shutterSpeed = camera.shutterSpeed;
#else
			anamorphism = default;
			aperture = default;
			curvature = default;
			iso = default;
			barrelClipping = default;
			bladeCount = default;
			focusDistance = default;
			shutterSpeed = default;
#endif

			aspect = camera.aspect;
			depth = camera.depth;
			orthographic = camera.orthographic;
			backgroundColor = camera.backgroundColor;
			cameraType = camera.cameraType;
			cameraClearFlags = camera.clearFlags;
			cullingMask = camera.cullingMask;
			eventMask = camera.eventMask;
			focalLength = camera.focalLength;
			gateFitMode = camera.gateFit;
			lensShift = camera.lensShift;
			orthographicSize = camera.orthographicSize;
			sensorSize = camera.sensorSize;
			stereoConvergence = camera.stereoConvergence;
			stereoSeparation = camera.stereoSeparation;
			targetDisplay = camera.targetDisplay;
			allowDynamicResolution = camera.allowDynamicResolution;
			farClipPlane = camera.farClipPlane;
			fieldOfView = camera.fieldOfView;
			layerCullDistances = camera.layerCullDistances;
			layerCullSpherical = camera.layerCullSpherical;
			nearClipPlane = camera.nearClipPlane;
			opaqueSortMode = camera.opaqueSortMode;
			stereoTargetEyeMask = camera.stereoTargetEye;
			transparencySortAxis = camera.transparencySortAxis;
			useOcclusionCulling = camera.useOcclusionCulling;
			usePhysicalProperties = camera.usePhysicalProperties;
			allowHdr = camera.allowHDR;
			forceIntoRenderTexture = camera.forceIntoRenderTexture;
			allowMsaa = camera.allowMSAA;
			clearStencilAfterLightingPass = camera.clearStencilAfterLightingPass;
		}
	}
}
