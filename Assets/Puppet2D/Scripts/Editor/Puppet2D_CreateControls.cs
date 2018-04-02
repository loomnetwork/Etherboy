using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet2D
{
	public class Puppet2D_CreateControls : Editor
	{

		[MenuItem("GameObject/Puppet2D/Rig/Create IK Control")]
		public static void IKCreateToolMenu()
		{
			IKCreateTool();
		}
		public static void IKCreateTool(bool worldSpace = false)
		{

			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (bone.GetComponent<SpriteRenderer>())
				{
					if (!bone.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
					{
						Debug.LogWarning("This is not a Puppet2D Bone");
						return;
					}
				}
				else
				{
					Debug.LogWarning("This is not a Puppet2D Bone");
					return;
				}
			}
			else
			{
				Debug.LogWarning("This is not a Puppet2D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (Puppet2D_ParentControl parentControl in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform) || (parentControl.bone.transform == bone.transform.parent.transform))
				{
					Debug.LogWarning("Can't create a IK Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			foreach (Puppet2D_IKHandle ikhandle in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._Ikhandles)
			{
				if ((ikhandle.bottomJointTransform == bone.transform) || (ikhandle.middleJointTransform == bone.transform) || (ikhandle.topJointTransform == bone.transform))
				{
					Debug.LogWarning("Can't create a IK Control on Bone; it alreay has an IK handle");
					return;
				}
			}

			GameObject IKRoot = null;
			if (bone.transform.parent && bone.transform.parent.transform.parent && bone.transform.parent.transform.parent.GetComponent<SpriteRenderer>() && bone.transform.parent.transform.parent.GetComponent<SpriteRenderer>().sprite != null && bone.transform.parent.transform.parent.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
				IKRoot = bone.transform.parent.transform.parent.gameObject;
			if (IKRoot == null)
			{
				Debug.LogWarning("You need to select the end of a chain of three bones");
				return;
			}
			// CHECK IF TOP BONE HAS AN IK ATTACHED

			Puppet2D_GlobalControl[] globalCtrls = GameObject.FindObjectsOfType<Puppet2D_GlobalControl>();
			foreach (Puppet2D_GlobalControl glblCtrl in globalCtrls)
			{
				foreach (Puppet2D_IKHandle ik in glblCtrl._Ikhandles)
				{
					if (ik.topJointTransform == bone.transform.parent.transform.parent)
					{
						Debug.LogWarning(bone.transform.parent.transform.parent.name + " already has an IK control");
						return;
					}
				}
				foreach (Puppet2D_SplineControl splineCtrl in glblCtrl._SplineControls)
				{
					foreach (GameObject splineBone in splineCtrl.bones)
					{
						if (splineBone.transform == bone.transform.parent.transform.parent)
						{
							Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached, please make sure there are at least 3 bones after the spline bone");
							return;
						}
					}
				}
			}


			// CHECK TO SEE IF THE BOTTOM BONE IS POINTING AT THE MIDDLE BONE
			if (bone.transform.parent.transform.parent.rotation != Quaternion.LookRotation(bone.transform.parent.transform.position - bone.transform.parent.transform.parent.position, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.right))
			{           //if(bone.transform.parent.transform.parent);

				Puppet2D_BoneCreation.sortOutBoneHierachy(bone.transform.parent.gameObject, true);
			}
			if (bone.transform.parent.rotation != Quaternion.LookRotation(bone.transform.position - bone.transform.parent.position, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.right))
			{           //if(bone.transform.parent.transform.parent);

				Puppet2D_BoneCreation.sortOutBoneHierachy(bone, true);
			}


			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "new control grp");
			controlGroup.name = (bone.name + "_CTRL_GRP");

			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			if (!worldSpace)
				controlGroup.transform.rotation = bone.transform.rotation;

			GameObject poleVector = new GameObject();
			Undo.RegisterCreatedObjectUndo(poleVector, "Created polevector");
			poleVector.name = (bone.name + "_POLE");

			SpriteRenderer spriteRenderer = control.AddComponent<SpriteRenderer>();
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/IKControl.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;

			// store middle bone position to check if it needs flipping

			Vector3 middleBonePos = bone.transform.parent.transform.position;

			Puppet2D_IKHandle ikHandle = control.AddComponent<Puppet2D_IKHandle>();
			ikHandle.topJointTransform = IKRoot.transform;
			ikHandle.middleJointTransform = bone.transform.parent.transform;
			ikHandle.bottomJointTransform = bone.transform;
			ikHandle.poleVector = poleVector.transform;
			ikHandle.scaleStart[0] = IKRoot.transform.localScale;
			ikHandle.scaleStart[1] = IKRoot.transform.GetChild(0).localScale;
			ikHandle.OffsetScale = bone.transform.localScale;

			if (worldSpace)
				ikHandle.Offset = ikHandle.bottomJointTransform.rotation;

			if (bone.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
			{
				ikHandle.AimDirection = Vector3.forward;
				ikHandle.UpDirection = Vector3.right;
			}
			else
			{
				Debug.LogWarning("This is not a Puppet2D Bone");
				ikHandle.AimDirection = Vector3.right;
				ikHandle.UpDirection = Vector3.up;
			}


			//if (bone.transform.parent.transform.position.x < IKRoot.transform.position.x)

			Selection.activeObject = ikHandle;

			controlGroup.transform.parent = globalCtrl.transform;
			poleVector.transform.parent = globalCtrl.transform;
			if (globalCtrl.GetComponent<Puppet2D_GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<Puppet2D_GlobalControl>().Init();
			else
				globalCtrl.GetComponent<Puppet2D_GlobalControl>()._Ikhandles.Add(ikHandle);


			//fix from now on for 180 flip
			globalCtrl.GetComponent<Puppet2D_GlobalControl>()._flipCorrection = -1;
			globalCtrl.GetComponent<Puppet2D_GlobalControl>().Run();
			if ((Vector3.Distance(bone.transform.parent.transform.position, middleBonePos) > 0.0001f))
			{
				ikHandle.Flip = true;
			}

		}
		[MenuItem("GameObject/Puppet2D/Rig/Create Parent Control")]
		public static void CreateParentControl()
		{
			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (bone.GetComponent<SpriteRenderer>())
				{
					if (!bone.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
					{
						Debug.LogWarning("This is not a Puppet2D Bone");
						return;
					}
				}
				else
				{
					Debug.LogWarning("This is not a Puppet2D Bone");
					return;
				}
			}
			else
			{
				Debug.LogWarning("This is not a Puppet2D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (Puppet2D_IKHandle ikhandle in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._Ikhandles)
			{
				if ((ikhandle.bottomJointTransform == bone.transform) || (ikhandle.middleJointTransform == bone.transform))
				{
					Debug.LogWarning("Can't create a parent Control on Bone; it alreay has an IK handle");
					return;
				}
			}
			foreach (Puppet2D_ParentControl parentControl in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform))
				{
					Debug.LogWarning("Can't create a Parent Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			foreach (Puppet2D_SplineControl splineCtrl in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._SplineControls)
			{
				foreach (GameObject splineBone in splineCtrl.bones)
				{
					if (splineBone.transform == bone.transform)
					{
						Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached");
						return;
					}
				}
			}
			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "CreatedControlGrp");
			controlGroup.name = (bone.name + "_CTRL_GRP");
			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			controlGroup.transform.rotation = bone.transform.rotation;

			SpriteRenderer spriteRenderer = control.AddComponent<SpriteRenderer>();
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/ParentControl.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;
			Puppet2D_ParentControl parentConstraint = control.AddComponent<Puppet2D_ParentControl>();
			parentConstraint.IsEnabled = true;
			parentConstraint.Orient = true;
			parentConstraint.Point = true;
			parentConstraint.bone = bone;
			parentConstraint.OffsetScale = bone.transform.localScale;
			Selection.activeObject = control;


			controlGroup.transform.parent = globalCtrl.transform;

			if (globalCtrl.GetComponent<Puppet2D_GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<Puppet2D_GlobalControl>().Init();
			else
				globalCtrl.GetComponent<Puppet2D_GlobalControl>()._ParentControls.Add(parentConstraint);


		}
		public static GameObject CreateGlobalControl()
		{
			GameObject globalCtrl = GameObject.Find("Global_CTRL");

			if (globalCtrl)
			{
				return globalCtrl;
			}
			else
			{
				globalCtrl = new GameObject("Global_CTRL");
				Undo.RegisterCreatedObjectUndo(globalCtrl, "Created globalCTRL");

				globalCtrl.AddComponent<Puppet2D_GlobalControl>();

				return globalCtrl;
			}

		}
		[MenuItem("GameObject/Puppet2D/Rig/Create Orient Control")]
		public static void CreateOrientControl()
		{
			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (bone.GetComponent<SpriteRenderer>())
				{
					if (!bone.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
					{
						Debug.LogWarning("This is not a Puppet2D Bone");
						return;
					}
				}
				else
				{
					Debug.LogWarning("This is not a Puppet2D Bone");
					return;
				}
			}
			else
			{
				Debug.LogWarning("This is not a Puppet2D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (Puppet2D_IKHandle ikhandle in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._Ikhandles)
			{
				if ((ikhandle.bottomJointTransform == bone.transform) || (ikhandle.middleJointTransform == bone.transform))
				{
					Debug.LogWarning("Can't create a orient Control on Bone; it alreay has an IK handle");
					return;
				}
			}
			foreach (Puppet2D_ParentControl parentControl in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform))
				{
					Debug.LogWarning("Can't create a Parent Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			foreach (Puppet2D_SplineControl splineCtrl in globalCtrl.GetComponent<Puppet2D_GlobalControl>()._SplineControls)
			{
				foreach (GameObject splineBone in splineCtrl.bones)
				{
					if (splineBone.transform == bone.transform)
					{
						Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached");
						return;
					}
				}
			}

			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "Created controlGroup");
			controlGroup.name = (bone.name + "_CTRL_GRP");
			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			controlGroup.transform.rotation = bone.transform.rotation;
			SpriteRenderer spriteRenderer = control.AddComponent<SpriteRenderer>();
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/OrientControl.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;
			Puppet2D_ParentControl parentConstraint = control.AddComponent<Puppet2D_ParentControl>();
			parentConstraint.IsEnabled = true;
			parentConstraint.Orient = true;
			parentConstraint.Point = false;
			parentConstraint.bone = bone;
			Selection.activeObject = control;
			parentConstraint.OffsetScale = bone.transform.localScale;

			controlGroup.transform.parent = globalCtrl.transform;

			if (globalCtrl.GetComponent<Puppet2D_GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<Puppet2D_GlobalControl>().Init();
			else
				globalCtrl.GetComponent<Puppet2D_GlobalControl>()._ParentControls.Add(parentConstraint);
		}
		public static void CreateAvatar()
		{
			GameObject go = Selection.activeGameObject;

			if (go != null && go.GetComponent("Animator") != null)
			{
				HumanDescription hd = new HumanDescription();

				Dictionary<string, string> boneName = new System.Collections.Generic.Dictionary<string, string>();
				boneName["Chest"] = "spine";
				boneName["Head"] = "head";
				boneName["Hips"] = "hip";
				boneName["LeftFoot"] = "footL";
				boneName["LeftHand"] = "handL";
				boneName["LeftLowerArm"] = "elbowL";
				boneName["LeftLowerLeg"] = "kneeL";
				boneName["LeftShoulder"] = "clavL";
				boneName["LeftUpperArm"] = "armL";
				boneName["LeftUpperLeg"] = "legL";
				boneName["RightFoot"] = "footR";
				boneName["RightHand"] = "handR";
				boneName["RightLowerArm"] = "elbowR";
				boneName["RightLowerLeg"] = "kneeR";
				boneName["RightShoulder"] = "clavR";
				boneName["RightUpperArm"] = "armR";
				boneName["RightUpperLeg"] = "legR";
				boneName["Spine"] = "spine2";
				string[] humanName = HumanTrait.BoneName;
				HumanBone[] humanBones = new HumanBone[boneName.Count];
				int j = 0;
				int i = 0;
				while (i < humanName.Length)
				{
					if (boneName.ContainsKey(humanName[i]))
					{
						HumanBone humanBone = new HumanBone();
						humanBone.humanName = humanName[i];
						humanBone.boneName = boneName[humanName[i]];
						humanBone.limit.useDefaultValues = true;
						humanBones[j++] = humanBone;
					}
					i++;
				}

				hd.human = humanBones;

				//hd.skeleton = new SkeletonBone[18];
				//hd.skeleton[0].name = ("Hips") ;
				Avatar avatar = AvatarBuilder.BuildHumanAvatar(go, hd);

				avatar.name = (go.name + "_Avatar");
				Debug.Log(avatar.isHuman ? "is human" : "is generic");

				Animator animator = go.GetComponent("Animator") as Animator;
				animator.avatar = avatar;

				string path = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Animation/" + avatar.name + ".asset");
				AssetDatabase.CreateAsset(avatar, path);

			}

		}
	}
}