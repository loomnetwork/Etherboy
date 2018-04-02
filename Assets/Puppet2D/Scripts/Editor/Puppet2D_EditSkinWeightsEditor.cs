using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Puppet2D_EditSkinWeights))]
	public class Puppet2D_EditSkinWeightsEditor : Editor
	{
		public int bone0, bone1, bone2, bone3;
		Puppet2D_EditSkinWeights myTarget;
		//public bool autoUpdate = false;
		public void OnEnable()
		{
			myTarget = (Puppet2D_EditSkinWeights)target;

		}
		public override void OnInspectorGUI()
		{

			serializedObject.Update();
			Undo.RecordObject(myTarget, "changed weights");
			string[] _choices = new string[myTarget.meshRenderer.bones.Length + 1];
			for (int i = 0; i < myTarget.meshRenderer.bones.Length; i++)
			{

				_choices[i] = myTarget.meshRenderer.bones[i].name;

			}
			//_choices[myTarget.meshRenderer.bones.Length] = "None";

			BoneWeight[] boneWeights = myTarget.mesh.boneWeights;

			myTarget.boneIndex0 = EditorGUILayout.Popup(myTarget.boneIndex0, _choices);

			myTarget.Weight0 = EditorGUILayout.FloatField("Weight 0", myTarget.Weight0);

			myTarget.boneIndex1 = EditorGUILayout.Popup(myTarget.boneIndex1, _choices);

			myTarget.Weight1 = EditorGUILayout.FloatField("Weight 1", myTarget.Weight1);

			myTarget.boneIndex2 = EditorGUILayout.Popup(myTarget.boneIndex2, _choices);

			myTarget.Weight2 = EditorGUILayout.FloatField("Weight 2", myTarget.Weight2);

			myTarget.boneIndex3 = EditorGUILayout.Popup(myTarget.boneIndex3, _choices);

			myTarget.Weight3 = EditorGUILayout.FloatField("Weight 3", myTarget.Weight3);

			myTarget.meshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(myTarget.meshRenderer, typeof(SkinnedMeshRenderer), true);




			if (myTarget.boneIndex0 != myTarget.meshRenderer.bones.Length)
				boneWeights[myTarget.vertNumber].boneIndex0 = myTarget.boneIndex0;
			if (myTarget.boneIndex1 != myTarget.meshRenderer.bones.Length)
				boneWeights[myTarget.vertNumber].boneIndex1 = myTarget.boneIndex1;
			if (myTarget.boneIndex2 != myTarget.meshRenderer.bones.Length)
				boneWeights[myTarget.vertNumber].boneIndex2 = myTarget.boneIndex2;
			if (myTarget.boneIndex3 != myTarget.meshRenderer.bones.Length)
				boneWeights[myTarget.vertNumber].boneIndex3 = myTarget.boneIndex3;

			boneWeights[myTarget.vertNumber].weight0 = myTarget.Weight0;
			boneWeights[myTarget.vertNumber].weight1 = myTarget.Weight1;
			boneWeights[myTarget.vertNumber].weight2 = myTarget.Weight2;
			boneWeights[myTarget.vertNumber].weight3 = myTarget.Weight3;

			myTarget.mesh.boneWeights = boneWeights;
			Undo.RecordObject(myTarget.meshRenderer, "changed weights");
			myTarget.meshRenderer.sharedMesh = myTarget.mesh;
			SpriteRenderer[] sprs = FindObjectsOfType<SpriteRenderer>();

			foreach (SpriteRenderer spr in sprs)
			{
				if (spr.sprite)
				{
					if (spr.sprite.name.Contains("Bone"))
					{
						spr.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						if (_choices[myTarget.boneIndex0] == spr.gameObject.name)
							spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.green, myTarget.Weight0);
						else if (spr.gameObject.GetComponent<Puppet2D_HiddenBone>())
						{
							if (_choices[myTarget.boneIndex0] == spr.gameObject.transform.parent.name)
								spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.green, myTarget.Weight0);
						}

						if (_choices[myTarget.boneIndex1] == spr.gameObject.name)
							spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.blue, myTarget.Weight1);
						else if (spr.gameObject.GetComponent<Puppet2D_HiddenBone>())
						{
							if (_choices[myTarget.boneIndex1] == spr.gameObject.transform.parent.name)
								spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.blue, myTarget.Weight1);
						}

						if (_choices[myTarget.boneIndex2] == spr.gameObject.name)
							spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, myTarget.Weight2);
						else if (spr.gameObject.GetComponent<Puppet2D_HiddenBone>())
						{
							if (_choices[myTarget.boneIndex2] == spr.gameObject.transform.parent.name)
								spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, myTarget.Weight2);
						}

						if (_choices[myTarget.boneIndex3] == spr.gameObject.name)
							spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.yellow, myTarget.Weight3);
						else if (spr.gameObject.GetComponent<Puppet2D_HiddenBone>())
						{
							if (_choices[myTarget.boneIndex3] == spr.gameObject.transform.parent.name)
								spr.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.yellow, myTarget.Weight3);
						}


					}

				}
			}


			if (GUILayout.Button("Update Skin Weights"))
			{
				GameObject[] handles = Selection.gameObjects;
				//GameObject[] handles = GameObject.FindGameObjectsWithTag ("handle");

				for (int i = 0; i < handles.Length; i++)
				{
					if (myTarget.boneIndex0 < myTarget.meshRenderer.bones.Length)
						handles[i].GetComponent<Puppet2D_EditSkinWeights>().boneIndex0 = myTarget.boneIndex0;
					if (myTarget.boneIndex1 < myTarget.meshRenderer.bones.Length)
						handles[i].GetComponent<Puppet2D_EditSkinWeights>().boneIndex1 = myTarget.boneIndex1;
					if (myTarget.boneIndex2 < myTarget.meshRenderer.bones.Length)
						handles[i].GetComponent<Puppet2D_EditSkinWeights>().boneIndex2 = myTarget.boneIndex2;
					if (myTarget.boneIndex3 < myTarget.meshRenderer.bones.Length)
						handles[i].GetComponent<Puppet2D_EditSkinWeights>().boneIndex3 = myTarget.boneIndex3;

					handles[i].GetComponent<Puppet2D_EditSkinWeights>().Weight0 = myTarget.Weight0;
					handles[i].GetComponent<Puppet2D_EditSkinWeights>().Weight1 = myTarget.Weight1;
					handles[i].GetComponent<Puppet2D_EditSkinWeights>().Weight2 = myTarget.Weight2;
					handles[i].GetComponent<Puppet2D_EditSkinWeights>().Weight3 = myTarget.Weight3;

					handles[i].GetComponent<Puppet2D_EditSkinWeights>().Refresh();
				}

			}
			//EditorGUILayout.LabelField("Auto Update:");
			//autoUpdate = EditorGUILayout.Toggle(autoUpdate);


			serializedObject.ApplyModifiedProperties();

		}
		/*
		public void Update()
		{
			if (autoUpdate)
				myTarget.Refresh();
		}
		*/

	}
}