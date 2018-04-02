using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(Puppet2D_IKHandle))]
	public class Puppet2D_IKHandleEditor : Editor
	{
		public string[] IkType = { "Basic 3 Bone", "Multi Bone" };

		//	private List<Quaternion> bindPose;
		//	private List<Transform> bindBones;

		private bool _SquashAndStretch = false;

		public void OnEnable()
		{
			Puppet2D_IKHandle myTarget = (Puppet2D_IKHandle)target;
			//myTarget.startTransform = myTarget.topJointTransform;

			myTarget.endTransform = myTarget.bottomJointTransform;



		}



		public override void OnInspectorGUI()
		{
			Puppet2D_IKHandle myTarget = (Puppet2D_IKHandle)target;

			GUI.changed = false;

			//				DrawDefaultInspector();	
			EditorGUI.BeginChangeCheck();
			myTarget.numberIkBonesIndex = EditorGUILayout.Popup("IK Type", myTarget.numberIkBonesIndex, IkType);//The popup menu is displayed simple as that
			if (EditorGUI.EndChangeCheck())
			{
				if (myTarget.numberIkBonesIndex == 0)
					resetIK(myTarget);
				else
				{
					myTarget.bindPose = new List<Vector3>();
					myTarget.bindBones = new List<Transform>();

					myTarget.bindPose.Clear();
					myTarget.bindBones.Clear();

					Transform node = myTarget.bottomJointTransform;
					while (true)
					{

						if (node == null)
							break;

						myTarget.bindBones.Add(node);
						myTarget.bindPose.Add(node.localEulerAngles);

						node = node.parent;
					}


					setEndBone(myTarget);
				}
			}

			if ((target as Puppet2D_IKHandle).numberIkBonesIndex == 0)
			{
				EditorGUI.BeginChangeCheck();
				bool f = EditorGUILayout.Toggle("Flip", myTarget.Flip);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(myTarget, "Toggle Flip");
					myTarget.Flip = f;
				}
				myTarget.SquashAndStretch = EditorGUILayout.Toggle("SquashAndStretch", myTarget.SquashAndStretch);
				myTarget.Scale = EditorGUILayout.Toggle("Scale", myTarget.Scale);


			}
			else
			{



				EditorGUI.BeginChangeCheck();
				myTarget.numberOfBones = EditorGUILayout.IntField("Number of Bones", myTarget.numberOfBones);
				if (EditorGUI.EndChangeCheck())
				{
					setEndBone(myTarget);

				}
				myTarget.iterations = EditorGUILayout.IntField("Iterations", myTarget.iterations);
				myTarget.damping = EditorGUILayout.Slider("Dampening", myTarget.damping, 0f, 1f);
				EditorGUI.BeginChangeCheck();
				myTarget.limitBones = EditorGUILayout.Toggle("Limits", myTarget.limitBones);
				if (EditorGUI.EndChangeCheck())
				{
					setEndBone(myTarget);

				}
				if (GUILayout.Button("Reset"))
				{

					resetIK(myTarget);
					myTarget.transform.localPosition = Vector3.zero;
				}

			}


			if (GUI.changed)
			{
				if (_SquashAndStretch && !myTarget.SquashAndStretch)
					myTarget.topJointTransform.localScale = myTarget.scaleStart[0];

				_SquashAndStretch = myTarget.SquashAndStretch;

				EditorUtility.SetDirty(myTarget);
			}




		}

		public void resetIK(Puppet2D_IKHandle myTarget)
		{
			myTarget.enabled = false;
			//myTarget.transform.localPosition = Vector3.zero;
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < myTarget.bindBones.Count; j++)
				{
					myTarget.bindBones[j].localRotation = Quaternion.Euler(myTarget.bindPose[j]);

				}
			}
			myTarget.enabled = true;

		}
		public void setEndBone(Puppet2D_IKHandle myTarget)
		{
			myTarget.angleLimits.Clear();
			myTarget.angleLimitTransform.Clear();



			if (myTarget.numberOfBones < 2)
				myTarget.numberOfBones = 2;

			Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();


			myTarget.endTransform = myTarget.bottomJointTransform;

			myTarget.startTransform = myTarget.endTransform;

			bool unlockedBone = true;

			for (int i = 0; i < myTarget.numberOfBones - 1; i++)
			{


				if (myTarget.startTransform.parent != null)
				{
					for (int j = 0; j < globalCtrlScripts.Length; j++)
					{
						if (myTarget.startTransform.parent.GetComponent<Puppet2D_GlobalControl>())
						{
							myTarget.numberOfBones = i + 1;
							unlockedBone = false;
						}

						foreach (Puppet2D_ParentControl ParentControl in globalCtrlScripts[j]._ParentControls)
						{
							if (ParentControl.bone.transform == myTarget.startTransform.parent)
							{
								myTarget.numberOfBones = i + 1;
								unlockedBone = false;
							}
						}
						foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[j]._SplineControls)
						{
							foreach (GameObject bone in splineCtrl.bones)
							{
								if (bone.transform == myTarget.startTransform.parent)
								{
									myTarget.numberOfBones = i + 1;
									unlockedBone = false;

								}
							}
						}
					}
					if (unlockedBone)
					{


						if (myTarget.startTransform != myTarget.endTransform && myTarget.limitBones)
						{
							Vector2 limit = new Vector2();
							Transform limitTransform = myTarget.startTransform;

							Vector3 newEulerAngle = new Vector3(limitTransform.localEulerAngles.x % 360,
																 limitTransform.localEulerAngles.y % 360,
																 limitTransform.localEulerAngles.z % 360);

							if (newEulerAngle.x < 0)
								newEulerAngle.x += 360;
							if (newEulerAngle.y < 0)
								newEulerAngle.y += 360;
							if (newEulerAngle.z < 0)
								newEulerAngle.z += 360;
							myTarget.startTransform.localEulerAngles = newEulerAngle;

							float rangedVal = limitTransform.localEulerAngles.z % 360;
							if (rangedVal > 0 && rangedVal < 180)
							{
								limit = new Vector2(0, 180);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > 180 && rangedVal < 360)
							{
								limit = new Vector2(180, 360);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > -180 && rangedVal < 0)
							{

								limit = new Vector2(-180, 0);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > -360 && rangedVal < -180)
							{
								limit = new Vector2(-360, -180);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}



						}
						myTarget.startTransform = myTarget.startTransform.parent;
					}


				}
				else
					myTarget.numberOfBones = i + 1;



			}
			//CreateNodeCache (myTarget);
			//		Debug.Log ("start " + myTarget.startTransform + " end " + myTarget.endTransform); 
		}
		void OnValidate()
		{
			//Puppet2D_IKHandle myTarget = (Puppet2D_IKHandle)target;

			//		if (myTarget.SquashAndStretch)
			//			Debug.Log ("Sqiuash");
			//		else
			//			Debug.Log ("normal");


		}


		//	void CreateNodeCache(Puppet2D_IKHandle myTarget)
		//	{
		//		// Cache optimization
		//		myTarget.nodeCache = new Dictionary<Transform, AngleLimitNode>(myTarget.angleLimits.Count);
		//		foreach (var node in myTarget.angleLimits)
		//			if (!myTarget.nodeCache.ContainsKey(node.Transform))
		//				myTarget.nodeCache.Add(node.Transform, node);
		//	}



	}
}