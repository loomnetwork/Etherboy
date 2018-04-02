using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet2D
{
	public class Puppet2D_FFD : Editor
	{
		public static GameObject FFDControlsGrp;
		public static Puppet2D_FFDStoreData ffdStoreData;
		public static void FFDCreationMode(Vector3 mousePos)
		{
			string newCtrlName = "FFD_CTRL";
			string newCtrlGRPName = "FFD_CTRL_GRP";

			if (Puppet2D_Editor.FFDGameObject)
			{
				newCtrlName = Puppet2D_Editor.FFDGameObject.name + "_Ctrl";
				newCtrlGRPName = Puppet2D_Editor.FFDGameObject.name + "_Ctrl_GRP";
			}

			GameObject newCtrl = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName(newCtrlName));
			GameObject newCtrlGRP = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName(newCtrlGRPName));
			newCtrl.transform.parent = newCtrlGRP.transform;

			Undo.RegisterCreatedObjectUndo(newCtrl, "Created newCtrl");
			Undo.RegisterCreatedObjectUndo(newCtrlGRP, "Created newCtrlGRP");

			Undo.RecordObject(ffdStoreData, "Adding FFD Control");
			ffdStoreData.FFDCtrls.Add(newCtrl.transform);


			Puppet2D_FFDLineDisplay ffdline = newCtrl.AddComponent<Puppet2D_FFDLineDisplay>();

			if (ffdStoreData.FFDCtrls.Count > 1)
			{
				if (ffdStoreData.FFDPathNumber.Count > 0)
				{
					if (ffdStoreData.FFDCtrls.Count - 1 > ffdStoreData.FFDPathNumber[ffdStoreData.FFDPathNumber.Count - 1])
						ffdline.target = ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 2];
				}
				else
					ffdline.target = ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 2];
			}



			newCtrlGRP.transform.position = new Vector3(mousePos.x, mousePos.y, 0);

			SpriteRenderer spriteRenderer = newCtrl.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/ffdBone.psd");

			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = Puppet2D_Editor._controlSortingLayer;


		}

		public static void FFDSetFirstPath()
		{
			FFDControlsGrp = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("FFD_Ctrls_GRP"));
			Undo.RegisterCreatedObjectUndo(FFDControlsGrp, "undo create FFD");
			ffdStoreData = FFDControlsGrp.AddComponent<Puppet2D_FFDStoreData>();
			ffdStoreData.OriginalSpritePosition = Puppet2D_Editor.FFDGameObject.transform.position;
			Puppet2D_Editor.FFDGameObject.transform.position = new Vector3(ffdStoreData.OriginalSpritePosition.x, ffdStoreData.OriginalSpritePosition.y, 0);
			if ((Puppet2D_Editor.FFDGameObject != null) && Puppet2D_Editor.FFDGameObject.GetComponent<PolygonCollider2D>())
			{
				Vector2[] firstPath = Puppet2D_Editor.FFDGameObject.GetComponent<PolygonCollider2D>().GetPath(0);
				foreach (Vector2 pos in firstPath)
				{
					FFDCreationMode(pos);
				}
				CloseFFDPath();
			}

		}


		public static void CloseFFDPath()
		{
			if (ffdStoreData != null && ffdStoreData.FFDCtrls.Count > 2)
			{
				if (ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 1] && ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 1].GetComponent<Puppet2D_FFDLineDisplay>().target2 == null)
				{
					if (ffdStoreData.FFDPathNumber.Count > 0)
						ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 1].GetComponent<Puppet2D_FFDLineDisplay>().target2 = ffdStoreData.FFDCtrls[ffdStoreData.FFDPathNumber[ffdStoreData.FFDPathNumber.Count - 1]];
					else
						ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 1].GetComponent<Puppet2D_FFDLineDisplay>().target2 = ffdStoreData.FFDCtrls[0];
					Undo.RecordObject(ffdStoreData, "Adding FFD Count");
					Undo.RegisterCompleteObjectUndo(ffdStoreData, "FFDPathChange");
					ffdStoreData.FFDPathNumber.Add(ffdStoreData.FFDCtrls.Count);
				}
			}

		}

		public static void FFDFinishCreation()
		{
			if (ffdStoreData == null)
				return;
			Puppet2D_Editor.FFDCreation = false;
			CloseFFDPath();



			Texture spriteTexture = null;

			//GameObject FFDControlsGrp = new GameObject(Puppet2D_BoneCreation.GetUniqueBoneName("FFD_Ctrls_GRP"));

			if (Puppet2D_Editor.FFDGameObject && Puppet2D_Editor.FFDGameObject.GetComponent<SpriteRenderer>() && Puppet2D_Editor.FFDGameObject.GetComponent<SpriteRenderer>().sprite)
			{
				spriteTexture = Puppet2D_Editor.FFDGameObject.GetComponent<SpriteRenderer>().sprite.texture;

				foreach (Transform FFDCtrl in ffdStoreData.FFDCtrls)
					FFDCtrl.transform.position = Puppet2D_Editor.FFDGameObject.transform.InverseTransformPoint(FFDCtrl.transform.position);


				FFDControlsGrp.transform.position = Puppet2D_Editor.FFDGameObject.transform.position;
				FFDControlsGrp.transform.rotation = Puppet2D_Editor.FFDGameObject.transform.rotation;
				FFDControlsGrp.transform.localScale = Puppet2D_Editor.FFDGameObject.transform.localScale;

				//Puppet2D_Editor.FFDGameObject.transform.position = Vector3.zero;
				//Puppet2D_Editor.FFDGameObject.transform.rotation = Quaternion.identity;
				//Puppet2D_Editor.FFDGameObject.transform.localScale = Vector3.one;

			}

			if (ffdStoreData.FFDCtrls.Count < 3)
			{
				//Undo.DestroyObjectImmediate(ffdStoreData);
				return;
			}

			Puppet2D_CreatePolygonFromSprite polyFromSprite = ScriptableObject.CreateInstance("Puppet2D_CreatePolygonFromSprite") as Puppet2D_CreatePolygonFromSprite;

			List<Vector3> verts = new List<Vector3>();

			for (int i = 0; i < ffdStoreData.FFDCtrls.Count(); i++)
			{
				if (ffdStoreData.FFDCtrls[i])
					verts.Add(new Vector3(ffdStoreData.FFDCtrls[i].position.x, ffdStoreData.FFDCtrls[i].position.y, 0));
				else
				{
					//                Debug.LogWarning("A FFD control point has been removed, no mesh created");
					//                Undo.DestroyObjectImmediate(ffdStoreData);
					//                return;
				}

			}
			GameObject newMesh;


			if (ffdStoreData.FFDPathNumber.Count > 0 && verts.Count > 2)
			{
				if (Puppet2D_Editor.FFDGameObject == null)
				{
					Puppet2D_Editor.FFDGameObject = new GameObject();
					Undo.RegisterCreatedObjectUndo(Puppet2D_Editor.FFDGameObject, "newGameObject");
				}


				Puppet2D_Editor._numberBonesToSkinToIndex = 0;

				string sortingLayer = "";
				int sortingOrder = 0;
				if (Puppet2D_Editor.FFDGameObject.GetComponent<Renderer>())
				{

					sortingLayer = Puppet2D_Editor.FFDGameObject.GetComponent<Renderer>().sortingLayerName;
					sortingOrder = Puppet2D_Editor.FFDGameObject.GetComponent<Renderer>().sortingOrder;

				}



				newMesh = polyFromSprite.MakeFromVerts(true, verts.ToArray(), ffdStoreData.FFDPathNumber, Puppet2D_Editor.FFDGameObject);

				if (Puppet2D_Editor.FFDGameObject.GetComponent<Renderer>())
				{
					newMesh.GetComponent<Renderer>().sortingLayerName = sortingLayer;
					newMesh.GetComponent<Renderer>().sortingOrder = sortingOrder;
				}
				Puppet2D_Editor._numberBonesToSkinToIndex = 1;

			}
			else
			{
				//Undo.DestroyObjectImmediate(ffdStoreData);
				return;
			}



			Undo.DestroyObjectImmediate(polyFromSprite);



			if (Puppet2D_Editor.FFDGameObject)
			{
				if (spriteTexture != null)
					newMesh.GetComponent<Renderer>().sharedMaterial.mainTexture = spriteTexture;
				else
					newMesh.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
				newMesh.name = Puppet2D_Editor.FFDGameObject.name;

				ffdStoreData.FFDCtrls.Add(newMesh.transform);

				Undo.DestroyObjectImmediate(Puppet2D_Editor.FFDGameObject);
			}



			GameObject globalCtrl = Puppet2D_CreateControls.CreateGlobalControl();
			Undo.SetTransformParent(FFDControlsGrp.transform, globalCtrl.transform, "parentToGlobal");
			Undo.SetTransformParent(newMesh.transform, globalCtrl.transform, "parentToGlobal");

			List<Object> newObjs = new List<Object>();
			foreach (Transform tr in ffdStoreData.FFDCtrls)
			{
				if (tr)
					newObjs.Add(tr.gameObject);
			}
			Selection.objects = newObjs.ToArray();

			Puppet2D_Editor._numberBonesToSkinToIndex = 1;


			//Undo.RecordObjects (newObjs.ToArray(), "recordingStuff");
			//Undo.RegisterCompleteObjectUndo (newObjs.ToArray(), "recordingStuff");

			Puppet2D_Skinning.BindSmoothSkin(1);

			for (int i = 0; i < ffdStoreData.FFDCtrls.Count - 1; i++)
			{
				//Debug.Log(ffdStoreData.FFDCtrls[i]);
				if (ffdStoreData.FFDCtrls[i])
				{
					Puppet2D_FFDLineDisplay ffdLine = ffdStoreData.FFDCtrls[i].GetComponent<Puppet2D_FFDLineDisplay>();
					ffdLine.outputSkinnedMesh = newMesh.GetComponent<SkinnedMeshRenderer>();
					for (int j = 0; j < ffdLine.outputSkinnedMesh.sharedMesh.vertices.Length; j++)
					{
						Vector3 vert = ffdLine.outputSkinnedMesh.sharedMesh.vertices[j];
						if (Vector3.Distance(vert, ffdStoreData.FFDCtrls[i].transform.position) < .001f)
							ffdStoreData.FFDCtrls[i].GetComponent<Puppet2D_FFDLineDisplay>().vertNumber = j;

					}
					Undo.SetTransformParent(ffdStoreData.FFDCtrls[i].parent.transform, FFDControlsGrp.transform, "parentFFDControls");

					ffdStoreData.FFDCtrls[i].transform.localPosition = Vector3.zero;

				}
			}
			FFDControlsGrp.transform.position = new Vector3(FFDControlsGrp.transform.position.x, FFDControlsGrp.transform.position.y, ffdStoreData.OriginalSpritePosition.z);


			Selection.activeGameObject = ffdStoreData.FFDCtrls[ffdStoreData.FFDCtrls.Count - 1].gameObject;
			ffdStoreData.FFDCtrls.RemoveAt(ffdStoreData.FFDCtrls.Count - 1);
			Undo.RegisterCompleteObjectUndo(ffdStoreData, "changinEditable");
			ffdStoreData.Editable = false;
			//        Undo.DestroyObjectImmediate(ffdStoreData);
			if (globalCtrl.GetComponent<Puppet2D_GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<Puppet2D_GlobalControl>().Init();



		}
		public static int GetIndexOfVector3(List<GameObject> checkList, Vector3 match)
		{
			float dist = 100000000f;
			int closestIndex = 0;
			for (int i = 0; i < checkList.Count; i++)
			{
				Vector3 check = checkList[i].transform.position;
				float distCheck = Vector3.Distance(check, match);
				if (distCheck < dist)
				{
					dist = distCheck;
					closestIndex = i;

				}

			}
			return closestIndex;
		}
	}
}