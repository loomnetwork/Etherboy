using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
namespace Puppet2D
{
	public class Puppet2D_AutoRig : Editor
	{

		static private string _puppet2DPath;

		[MenuItem("GameObject/Puppet2D/MakeGuides")]
		public static void MakeGuides()
		{
			GameObject[] gos = Selection.gameObjects;
			if (gos.Length == 0)
				return;

			GameObject go = gos[0];


			Bounds bounds = new Bounds();

			foreach (GameObject goer in gos)
			{
				if (goer.GetComponent<Renderer>() != null)
					bounds.Encapsulate(goer.GetComponent<Renderer>().bounds);
			}

			RecursivelyFindFolderPath("Assets");
			string path = (_puppet2DPath + "/Prefabs/Guides.prefab");
			GameObject guides = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			GameObject guideGO = Instantiate(guides) as GameObject;
			Undo.RegisterCreatedObjectUndo(guideGO, "Create guide object");
			guideGO.name = (go.name + " _GUIDES");
			Puppet2D_Guides guideComp = guideGO.AddComponent<Puppet2D_Guides>();

			guideComp.Biped = gos;
			guideComp.Bounds = bounds;
			guideGO.transform.localScale = new Vector3(bounds.size.y / 200f, bounds.size.y / 200f, bounds.size.y / 200f);

			guideGO.transform.position = new Vector3((bounds.max.x + bounds.min.x) / 2f, bounds.min.y, 0f);

		}
		public static Bounds GetBounds(GameObject go)
		{
			Sprite spr = go.GetComponent<SpriteRenderer>().sprite;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spr)) as TextureImporter;
			SpriteMetaData[] smdArray = textureImporter.spritesheet;
			Bounds newBounds = go.GetComponent<SpriteRenderer>().bounds;
			for (int k = 0; k < smdArray.Length; k++)
			{
				if (smdArray[k].name == spr.name)
				{

					float XProp = (smdArray[k].rect.center.x / spr.texture.width) - .5f;
					float YProp = (smdArray[k].rect.center.y / spr.texture.height) - .5f;
					float XScaleProp = smdArray[k].rect.width / spr.texture.width;
					float YScaleProp = smdArray[k].rect.height / spr.texture.height;
					Vector3 newSize = new Vector3(newBounds.size.x / XScaleProp, newBounds.size.y / YScaleProp, newBounds.size.z);
					newBounds.size = newSize;
					newBounds.center -= new Vector3(XProp * newSize.x, YProp * newSize.y, 0f);

				}
			}
			//        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//        cube.transform.position = newBounds.min;
			//        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//        cube.transform.position = newBounds.min + newBounds.size;
			return newBounds;
		}
		private static void RecursivelyFindFolderPath(string dir)
		{
			string[] paths = Directory.GetDirectories(dir);
			foreach (string s in paths)
			{
				if (s.Contains("Puppet2D"))
				{
					_puppet2DPath = s;
					break;
				}
				else
				{
					RecursivelyFindFolderPath(s);
				}
			}
		}


		[MenuItem("GameObject/Puppet2D/AutoRig")]
		public static void AutoRig()
		{
			Puppet2D_Guides[] guides = FindObjectsOfType<Puppet2D_Guides>();

			foreach (Puppet2D_Guides guide in guides)
			{
				Undo.RegisterCompleteObjectUndo(guides, "guides");

				// Set controls
				Transform _hipPoint = guide.transform.Find("hip_guide").transform;
				Transform _chestPoint = guide.transform.Find("chest_guide").transform;

				Transform _thighLPoint = guide.transform.Find("thighL_guide").transform;
				Transform _footLPoint = guide.transform.Find("footL_guide").transform;

				Transform _thighRPoint = guide.transform.Find("thighR_guide").transform;
				Transform _footRPoint = guide.transform.Find("footR_guide").transform;

				Transform _armLPoint = guide.transform.Find("armL_guide").transform;
				Transform _handLPoint = guide.transform.Find("handL_guide").transform;

				Transform _armRPoint = guide.transform.Find("armR_guide").transform;
				Transform _handRPoint = guide.transform.Find("handR_guide").transform;

				Transform _headPoint = guide.transform.Find("head_guide").transform;

				Transform _elbowLPoint = guide.transform.Find("elbowL_guide").transform;
				Transform _kneeLPoint = guide.transform.Find("kneeL_guide").transform;

				Transform _elbowRPoint = guide.transform.Find("elbowR_guide").transform;
				Transform _kneeRPoint = guide.transform.Find("kneeR_guide").transform;


				// Hide other global controls

				Puppet2D_GlobalControl[] globalControls = FindObjectsOfType<Puppet2D_GlobalControl>();
				foreach (Puppet2D_GlobalControl gc in globalControls)
					gc.gameObject.SetActive(false);

				GameObject[] gos = guide.Biped;
				Bounds[] bounds = new Bounds[gos.Length];
				string[] geoNames = new string[gos.Length];


				MakeSpine(_hipPoint, _chestPoint, _footLPoint.position);

				Vector3 _footLEnd = new Vector3(_footLPoint.position.x, guide.Bounds.min.y * .95f, _footLPoint.position.z);
				MakeLimb(_thighLPoint, _footLPoint, _kneeLPoint, _footLEnd, GameObject.Find("Spine_01"), "legL", "thighL", "kneeL", "footL", true);

				Vector3 _footREnd = new Vector3(_footRPoint.position.x, guide.Bounds.min.y * .95f, _footRPoint.position.z);
				MakeLimb(_thighRPoint, _footRPoint, _kneeRPoint, _footREnd, GameObject.Find("Spine_01"), "legR", "thighR", "kneeR", "footR", true);

				MakeClav(GameObject.Find("Spine_05"), "clavL");
				Vector3 _handLEnd = new Vector3(guide.Bounds.max.x * .95f, _handLPoint.position.y, _handLPoint.position.z);
				MakeLimb(_armLPoint, _handLPoint, _elbowLPoint, _handLEnd, GameObject.Find("clavL"), "armL", "shoulderL", "elbowL", "handL", false);
				//            Selection.activeGameObject = GameObject.Find("clavL");
				//            Puppet2D_CreateControls.CreateOrientControl();

				MakeClav(GameObject.Find("Spine_05"), "clavR");
				Vector3 _handREnd = new Vector3(guide.Bounds.min.x * .95f, _handRPoint.position.y, _handRPoint.position.z);
				MakeLimb(_armRPoint, _handRPoint, _elbowRPoint, _handREnd, GameObject.Find("clavR"), "armR", "shoulderR", "elbowR", "handR", false);
				//            Selection.activeGameObject = GameObject.Find("clavR");
				//            Puppet2D_CreateControls.CreateOrientControl();


				Vector3 headTop = new Vector3(_headPoint.position.x, guide.Bounds.max.y * .95f, _headPoint.position.z);
				MakeHead(_headPoint, _chestPoint, headTop);


				//SORTING ORDER ON BONES
				GameObject.Find("clavL").GetComponent<SpriteRenderer>().sortingOrder = -20;
				GameObject.Find("shoulderL").GetComponent<SpriteRenderer>().sortingOrder = -30;
				GameObject.Find("elbowL").GetComponent<SpriteRenderer>().sortingOrder = -40;
				GameObject.Find("handL").GetComponent<SpriteRenderer>().sortingOrder = -50;
				GameObject.Find("handLEnd").GetComponent<SpriteRenderer>().sortingOrder = -50;

				GameObject.Find("thighL").GetComponent<SpriteRenderer>().sortingOrder = -10;
				GameObject.Find("kneeL").GetComponent<SpriteRenderer>().sortingOrder = -20;
				GameObject.Find("footL").GetComponent<SpriteRenderer>().sortingOrder = -30;
				GameObject.Find("footLEnd").GetComponent<SpriteRenderer>().sortingOrder = -30;

				GameObject.Find("clavR").GetComponent<SpriteRenderer>().sortingOrder = 20;
				GameObject.Find("shoulderR").GetComponent<SpriteRenderer>().sortingOrder = 50;
				GameObject.Find("elbowR").GetComponent<SpriteRenderer>().sortingOrder = 40;
				GameObject.Find("handR").GetComponent<SpriteRenderer>().sortingOrder = 30;
				GameObject.Find("handREnd").GetComponent<SpriteRenderer>().sortingOrder = 30;

				GameObject.Find("thighR").GetComponent<SpriteRenderer>().sortingOrder = 30;
				GameObject.Find("kneeR").GetComponent<SpriteRenderer>().sortingOrder = 20;
				GameObject.Find("footR").GetComponent<SpriteRenderer>().sortingOrder = 10;
				GameObject.Find("footREnd").GetComponent<SpriteRenderer>().sortingOrder = 10;

				SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
				List<Object> objList = new List<Object>();

				for (int i = 0; i < sprites.Length; i++)
				{
					if (sprites[i] && sprites[i].sprite && sprites[i].sprite.name.Contains("Bone") && sprites[i].GetComponent<Puppet2D_HiddenBone>() == null && !sprites[i].name.Contains("clav"))
					{
						objList.Add(sprites[i].gameObject);
					}
				}
				for (int i = 0; i < gos.Length; i++)
				{

					string originalSpriteName = gos[i].name;
					bounds[i] = gos[i].GetComponent<SpriteRenderer>().bounds;

					Selection.activeGameObject = gos[i];
					bool isSkinned = false;

					foreach (Object obj in objList)
					{
						GameObject bone = obj as GameObject;
						if (bounds[i].Contains(bone.transform.position))
						{
							if (Vector3.Distance(bounds[i].center, bone.transform.position) < .8f * (bounds[i].size.x / 2f) && Vector3.Distance(bounds[i].center, bone.transform.position) < .8f * (bounds[i].size.y / 2f))
							{
								isSkinned = true;
								break;
							}

						}
					}
					if (isSkinned)
					{
						Puppet2D_Skinning.ConvertSpriteToMesh(5);
						geoNames[i] = (originalSpriteName + "_GEO");

						GameObject geo = GameObject.Find(geoNames[i]);
						if (i == 0)
							objList.Add(geo);
						else
							objList[objList.Count - 1] = geo;

						Selection.objects = objList.ToArray();

						Puppet2D_Skinning.BindSmoothSkin();

						Selection.activeGameObject = geo;

					}
					else
					{
						if (i == 0)
							objList.Add(gos[i]);
						else
							objList[objList.Count - 1] = gos[i];


						Selection.objects = objList.ToArray();

						Puppet2D_Skinning.BindRigidSkin();


					}
				}

				Puppet2D_GlobalControl globalControl = FindObjectOfType<Puppet2D_GlobalControl>();
				Animator animator = globalControl.gameObject.AddComponent<Animator>();
				Puppet2d_AnimatorController P2dController = globalControl.gameObject.AddComponent<Puppet2d_AnimatorController>();
				P2dController.speed = 2f * (_hipPoint.position.y - _footLPoint.position.y);
				P2dController.enabled = false;

				animator.runtimeAnimatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Animation/AutoRig/P2D_AnimatorController.controller", typeof(UnityEditor.Animations.AnimatorController));
				//globalControl.BonesVisiblity = false;
				//globalControl.UpdateVisibility();
				foreach (Puppet2D_GlobalControl gc in globalControls)
					gc.gameObject.SetActive(true);


				Undo.DestroyObjectImmediate(guide.gameObject);


			}
		}
		static void MakeClav(GameObject parentTo, string boneName)
		{
			Selection.activeObject = parentTo;
			Puppet2D_BoneCreation.CreateBoneTool();
			Puppet2D_BoneCreation.BoneCreationMode(parentTo.transform.position + Vector3.one * .1f);
			Puppet2D_BoneCreation.BoneFinishCreation();
			GameObject clav = GameObject.Find("bone_1");
			clav.name = boneName;

		}
		static void MakeLimb(Transform _thighLPoint, Transform _footLPoint, Transform _kneePoint, Vector3 FootEnd, GameObject parentTo, string controlName, string controlName3, string controlName2, string controlName1, bool flip = false)
		{
			Selection.activeObject = parentTo;

			float limbScale = Vector3.Distance(_footLPoint.position, _thighLPoint.position) * .01f;
			Vector3 scaledFootPos = (_footLPoint.position - _thighLPoint.position) / limbScale;
			scaledFootPos += _thighLPoint.position;

			Vector3 scaledKneePos = (_kneePoint.position - _thighLPoint.position) / limbScale;
			scaledKneePos += _thighLPoint.position;

			Puppet2D_BoneCreation.CreateBoneTool();
			Puppet2D_BoneCreation.BoneCreationMode(_thighLPoint.position);

			Puppet2D_BoneCreation.BoneCreationMode(scaledKneePos);
			GameObject endLimbGO = Puppet2D_BoneCreation.BoneCreationMode(scaledFootPos);

			Vector3 scaledFinalEndLimbGO = (FootEnd - _thighLPoint.position) / limbScale;
			scaledFinalEndLimbGO += _thighLPoint.position;

			GameObject finalEndLimbGO = Puppet2D_BoneCreation.BoneCreationMode(scaledFinalEndLimbGO);
			finalEndLimbGO.name = (controlName1 + "End");

			Puppet2D_BoneCreation.BoneFinishCreation();

			Selection.activeGameObject = endLimbGO;
			endLimbGO.name = controlName1;
			Puppet2D_CreateControls.IKCreateTool(true);
			GameObject limbControlParent = GameObject.Find(controlName1 + "_CTRL_GRP");

			Transform elbow = endLimbGO.transform.parent;
			Transform shoulderBone = endLimbGO.transform.parent.parent;
			elbow.name = (controlName2);
			shoulderBone.name = (controlName3);

			Transform limbParent = limbControlParent.transform.parent;
			limbControlParent.transform.parent = shoulderBone;
			shoulderBone.localScale = shoulderBone.localScale * limbScale;
			limbControlParent.transform.parent = limbParent;
			GameObject.Find(controlName1 + "_CTRL").GetComponent<Puppet2D_IKHandle>().Flip = flip;
		}


		static void MakeSpine(Transform _hipPoint, Transform _chestPoint, Vector3 GroundPos)
		{
			//Spine
			float hips2GroundScale = (_hipPoint.position.y - GroundPos.y);
			float spineScale = (.01f * hips2GroundScale) / 2.5f;
			Vector3 scaledChestPos = (_chestPoint.position - _hipPoint.position) / spineScale;
			scaledChestPos += _hipPoint.position;
			Puppet2D_Spline.CreateSplineTool();
			Puppet2D_Spline.SplineCreationMode(_hipPoint.position);
			Puppet2D_Spline.SplineCreationMode(scaledChestPos);
			Puppet2D_Spline.SplineFinishCreation();
			Transform splineRoot = GameObject.Find("spline_GRP_1").transform;
			Transform splineRootChild1 = GameObject.Find("spline_Ctrl_GRP_1").transform;
			Transform splineRootChild2 = GameObject.Find("spline_Ctrl_GRP_2").transform;
			splineRootChild1.parent = null;
			splineRootChild2.parent = null;
			splineRoot.position = _hipPoint.position;
			splineRootChild1.parent = splineRoot;
			splineRootChild2.parent = splineRoot;
			splineRoot.localScale = new Vector3(spineScale, spineScale, spineScale);
			Undo.RegisterCompleteObjectUndo(splineRoot, "spineScale");

			GameObject.Find("bone_1").name = "Spine_01";
			GameObject.Find("bone_2").name = "Spine_02";
			GameObject.Find("bone_3").name = "Spine_03";
			GameObject.Find("bone_4").name = "Spine_04";
			GameObject.Find("bone_5").name = "Spine_05";
		}

		static void MakeHead(Transform _headPoint, Transform _chestPoint, Vector3 _headTop)
		{
			float headScale = Vector3.Distance(_chestPoint.position, _headPoint.position) * .01f;


			Selection.activeObject = GameObject.Find("Spine_05");

			Puppet2D_BoneCreation.CreateBoneTool();
			Puppet2D_BoneCreation.BoneCreationMode(_headPoint.position);
			Puppet2D_BoneCreation.BoneCreationMode(_headTop);

			Puppet2D_BoneCreation.BoneFinishCreation();
			GameObject head = GameObject.Find("bone_1");
			head.name = "Head";
			Selection.activeGameObject = head;
			GameObject headEnd = GameObject.Find("bone_2");
			headEnd.name = "HeadEnd";

			Puppet2D_CreateControls.CreateOrientControl();
			GameObject headControl = GameObject.Find("Head_CTRL");
			headControl.GetComponent<Puppet2D_ParentControl>().ConstrianedPosition = true;
			GameObject headControlParent = GameObject.Find("Head_CTRL_GRP");
			headControlParent.transform.localScale = Vector3.one * headScale * 2f;
			//GameObject.Find ("bone_22").name = ("a");


		}
	}
}
