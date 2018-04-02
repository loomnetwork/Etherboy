using UnityEngine;
using System.Collections.Generic;
namespace Puppet2D
{
	public class Puppet2D_IKHandle : MonoBehaviour
	{


		public bool Flip, SquashAndStretch, Scale;

		[HideInInspector]
		public Vector3 AimDirection;

		[HideInInspector]
		public Transform poleVector;
		[HideInInspector]
		public Vector3 UpDirection;

		[HideInInspector]
		public Vector3[] scaleStart = new Vector3[2];
		[HideInInspector]
		public Transform topJointTransform, middleJointTransform, bottomJointTransform;

		[HideInInspector]
		public Vector3 OffsetScale = new Vector3(1, 1, 1);


		private Transform IK_CTRL;

		private Vector3 root2IK;
		private Vector3 root2IK2MiddleJoint;

		private bool LargerMiddleJoint;

		[HideInInspector]
		public int numberIkBonesIndex;


		public int numberOfBones = 4;

		public int iterations = 10;

		public float damping = 1;

		public Transform IKControl;

		public Transform endTransform;

		public Transform startTransform;

		public List<Vector3> bindPose;
		public List<Transform> bindBones;

		public bool limitBones = true;

		public Quaternion Offset = Quaternion.identity;
		/*
		void LateUpdate () 
		{
			if (!IsEnabled)
			{
				return;
			}
			CalculateIK();
		}
		*/
		public void CalculateIK()
		{


			if (numberIkBonesIndex == 1)
				CalculateMultiIK();
			else
			{


				int flipRotation;
				if (Flip)
					flipRotation = 1;
				else
					flipRotation = -1;
				IK_CTRL = transform;


				//position poleVector

				root2IK = (topJointTransform.position + IK_CTRL.position) / 2;



				Vector3 IK2Root = IK_CTRL.position - topJointTransform.position;

				Quaternion quat;

				quat = Quaternion.AngleAxis(flipRotation * 90, Vector3.forward);


				root2IK2MiddleJoint = quat * IK2Root;

				poleVector.position = root2IK - root2IK2MiddleJoint;


				// Get Angle 
				float angle = GetAngle();


				// Aim Joints

				Quaternion topJointAngleOffset = Quaternion.AngleAxis(angle * flipRotation, Vector3.forward);

				if (!IsNaN(topJointAngleOffset))
					topJointTransform.rotation = Quaternion.LookRotation(IK_CTRL.position - topJointTransform.position, AimDirection) * Quaternion.AngleAxis(90, UpDirection) * topJointAngleOffset;
				else
				{
					if (LargerMiddleJoint)
						topJointTransform.rotation = Quaternion.LookRotation(IK_CTRL.position - topJointTransform.position, AimDirection) * Quaternion.AngleAxis(-90, UpDirection);
					else
						topJointTransform.rotation = Quaternion.LookRotation(IK_CTRL.position - topJointTransform.position, AimDirection) * Quaternion.AngleAxis(90, UpDirection);
				}
				middleJointTransform.rotation = Quaternion.LookRotation(IK_CTRL.position - middleJointTransform.position, AimDirection) * Quaternion.AngleAxis(90, UpDirection);

				bottomJointTransform.rotation = IK_CTRL.rotation * Offset;
				if (Scale)
					bottomJointTransform.localScale = new Vector3(IK_CTRL.localScale.x * OffsetScale.x, IK_CTRL.localScale.y * OffsetScale.y, IK_CTRL.localScale.z * OffsetScale.z);


			}

		}
		private bool IsNaN(Quaternion q)
		{

			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);

		}
		private float GetAngle()
		{
			// Squash And Stretch
			if (SquashAndStretch)
			{
				topJointTransform.localScale = scaleStart[0];
				//middleJointTransform.localScale = scaleStart[1];
			}

			float topLength = Vector3.Distance(topJointTransform.position, middleJointTransform.position);
			float middleLength = Vector3.Distance(middleJointTransform.position, bottomJointTransform.position);
			float length = topLength + middleLength;

			float ikLength = Vector3.Distance(topJointTransform.position, IK_CTRL.position);

			if (middleLength > topLength)
				LargerMiddleJoint = true;
			else
				LargerMiddleJoint = false;

			// Squash And Stretch
			if (SquashAndStretch)
			{
				if (ikLength > length)
				{
					topJointTransform.localScale = new Vector3(scaleStart[0].x, (ikLength / length) * scaleStart[0].y, scaleStart[0].z);
					//bottomJointTransform.localScale = new Vector3(scaleStart[1].x, (length / ikLength)*scaleStart[1].y,scaleStart[1].z);
				}
			}

			ikLength = Mathf.Min(ikLength, length - 0.0001f);

			float adjacent = (topLength * topLength - middleLength * middleLength + ikLength * ikLength) / (2 * ikLength);

			float angle = Mathf.Acos(adjacent / topLength) * Mathf.Rad2Deg;

			return angle;
		}




		public List<Transform> angleLimitTransform = new List<Transform>();
		public List<Vector2> angleLimits = new List<Vector2>();

		void OnValidate()
		{
			// min & max has to be between 0 ... 360
			for (int i = 0; i < angleLimits.Count; i++)
			{
				angleLimits[i] = new Vector2(Mathf.Clamp(angleLimits[i].x, -360, 360), Mathf.Clamp(angleLimits[i].y, -360, 360));
			}


		}


		public void CalculateMultiIK()
		{

			if (transform == null || endTransform == null)
				return;

			int i = 0;

			while (i < iterations)
			{
				CalculateMultiIK_run();
				i++;
			}

			endTransform.rotation = transform.rotation;
		}

		void CalculateMultiIK_run()
		{
			Transform node = endTransform.parent;

			while (true)
			{
				RotateTowardsTarget(node);

				if (node == startTransform)
					break;

				node = node.parent;
			}
		}

		void RotateTowardsTarget(Transform startTransform)
		{
			if (startTransform == null) return;

			Vector2 toTarget = transform.position - startTransform.position;
			Vector2 toEnd = endTransform.position - startTransform.position;

			// Calculate how much we should rotate to get to the target
			float angle = SignedAngle(toEnd, toTarget);

			// Flip sign if character is turned around

			//angle *= Mathf.Sign(startTransform.root.localScale.x);
			if (startTransform.eulerAngles.y % 360 > 90 && startTransform.eulerAngles.y % 360 < 275)
				angle *= -1;

			// "Slows" down the IK solving
			angle *= damping;

			// Wanted angle for rotation
			angle = -(angle - startTransform.localEulerAngles.z);

			// Take care of angle limits 
			//		if (nodeCache != null && nodeCache.ContainsKey(startTransform))
			//		{
			//			Debug.Log (startTransform);
			//			// Clamp angle in local space
			//			var node = nodeCache[startTransform];
			//			angle = ClampAngle (angle, node.min, node.max);
			//		}

			if (angleLimits != null && angleLimitTransform.Contains(startTransform))
			{
				//Debug.Log (startTransform);
				// Clamp angle in local space
				Vector2 limit = angleLimits[angleLimitTransform.IndexOf(startTransform)];
				angle = ClampAngle(angle, limit.x, limit.y);
			}
			startTransform.localRotation = Quaternion.Euler(0, 0, angle);

		}

		public static float SignedAngle(Vector3 a, Vector3 b)
		{
			float angle = Vector3.Angle(a, b);
			float sign = Mathf.Sign(Vector3.Dot(Vector3.back, Vector3.Cross(a, b)));

			return angle * sign;
		}

		float ClampAngle(float angle, float min, float max)
		{
			//angle = angle % 360;

			return Mathf.Clamp(angle, min, max);
		}



	}
}