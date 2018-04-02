using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet2D
{
	public class Puppet2D_Spline : Editor
	{

		public static GameObject SplineCreationGroup;
		public static Puppet2D_FFDStoreData splineStoreData;

		public static void SplineFinishCreation()
		{
			Puppet2D_Editor.SplineCreation = false;
			if (splineStoreData == null)
				return;
			CreateSpline();
			splineStoreData.FFDCtrls.Clear();

		}

		static void CreateSpline()
		{
			if (splineStoreData.FFDCtrls.Count > 2 && splineStoreData.FFDCtrls[0] && splineStoreData.FFDCtrls[1] && splineStoreData.FFDCtrls[2])
			{

				GameObject tangentCtrl = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_Tangent"));
				Undo.RegisterCreatedObjectUndo(tangentCtrl, "Created splineTangent");
				splineStoreData.FFDCtrls.Add(tangentCtrl.transform);
				tangentCtrl.transform.parent = splineStoreData.FFDCtrls[splineStoreData.FFDCtrls.Count - 2].transform;
				tangentCtrl.transform.localPosition = Vector3.zero;
				SpriteRenderer spriteRenderer = splineStoreData.FFDCtrls[splineStoreData.FFDCtrls.Count - 2].GetComponent<SpriteRenderer>();
				string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/splineControl.psd");
				Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
				spriteRenderer.sprite = sprite;

				splineStoreData.FFDCtrls[1].position += splineStoreData.FFDCtrls[0].position - splineStoreData.FFDCtrls[2].position;

				splineStoreData.FFDCtrls[splineStoreData.FFDCtrls.Count - 1].position += splineStoreData.FFDCtrls[splineStoreData.FFDCtrls.Count - 2].position - splineStoreData.FFDCtrls[splineStoreData.FFDCtrls.Count - 3].position;

				Transform splineCtrlSwap = splineStoreData.FFDCtrls[0];
				splineStoreData.FFDCtrls[0] = splineStoreData.FFDCtrls[1];
				splineStoreData.FFDCtrls[1] = splineCtrlSwap;

				//GameObject OffsetGroup = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_GRP"));
				Puppet2D_SplineControl spline = SplineCreationGroup.AddComponent<Puppet2D_SplineControl>();

				spline._splineCTRLS.AddRange(splineStoreData.FFDCtrls);
				spline.numberBones = Puppet2D_Editor.numberSplineJoints;
				List<GameObject> splineBones = spline.Create();
				foreach (GameObject splineBone in splineBones)
				{
					splineBone.GetComponent<SpriteRenderer>().sortingLayerName = Puppet2D_Editor._boneSortingLayer;
				}
				foreach (Transform ctrl in splineStoreData.FFDCtrls)
				{
					if (!ctrl.parent.parent)
						ctrl.parent.parent = SplineCreationGroup.transform;
				}
				GameObject globalCtrl = Puppet2D_CreateControls.CreateGlobalControl();
				globalCtrl.GetComponent<Puppet2D_GlobalControl>()._SplineControls.Add(spline);
				SplineCreationGroup.transform.parent = globalCtrl.transform;

				globalCtrl.GetComponent<Puppet2D_GlobalControl>().InitializeArrays();
				globalCtrl.GetComponent<Puppet2D_GlobalControl>().Run();

				Undo.DestroyObjectImmediate(splineStoreData);

				splineStoreData.FFDCtrls.Clear();


				// parent spline bones
				Puppet2D_HiddenBone[] hiddenBones = GameObject.FindObjectsOfType<Puppet2D_HiddenBone>();

				if (globalCtrl != null)
				{
					foreach (Puppet2D_HiddenBone hiddenBone in hiddenBones)
					{
						if (hiddenBone && hiddenBone.transform.parent && hiddenBone.transform.parent.parent == null)
							hiddenBone.transform.parent.parent = globalCtrl.transform;
					}

				}

			}



		}

		public static void CreateSplineTool()
		{
			Puppet2D_Editor.SplineCreation = true;

			SplineCreationGroup = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_GRP"));
			Undo.RegisterCreatedObjectUndo(SplineCreationGroup, "undo create Spline");
			splineStoreData = SplineCreationGroup.AddComponent<Puppet2D_FFDStoreData>();

		}

		public static void SplineCreationMode(Vector3 mousePos)
		{

			GameObject newCtrl = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_Ctrl"));
			Undo.RegisterCreatedObjectUndo(newCtrl, "Created newCtrl");
			GameObject newCtrlGrp = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_Ctrl_GRP"));
			Undo.RegisterCreatedObjectUndo(newCtrlGrp, "Created newCtrlGrp");
			newCtrl.transform.parent = newCtrlGrp.transform;

			Undo.RecordObject(splineStoreData, "Adding To Spline Control");

			splineStoreData.FFDCtrls.Add(newCtrl.transform);


			// start and end
			if (splineStoreData.FFDCtrls.Count == 1)
			{
				GameObject tangentCtrl = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("spline_Tangent"));
				Undo.RegisterCreatedObjectUndo(tangentCtrl, "Created splineTangent");
				splineStoreData.FFDCtrls.Add(tangentCtrl.transform);
				tangentCtrl.transform.parent = splineStoreData.FFDCtrls[0].transform;
			}




			newCtrlGrp.transform.position = mousePos;
			newCtrlGrp.transform.position = new Vector3(newCtrlGrp.transform.position.x, newCtrlGrp.transform.position.y, 0);

			SpriteRenderer spriteRenderer = newCtrl.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/splineMiddleControl.psd");
			if (splineStoreData.FFDCtrls.Count == 2)
				path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/splineControl.psd");

			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spriteRenderer.sprite = sprite;


		}
	}
}
