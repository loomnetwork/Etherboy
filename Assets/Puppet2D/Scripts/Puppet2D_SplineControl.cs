using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_SplineControl : MonoBehaviour
	{


		public List<Transform> _splineCTRLS = new List<Transform>();
		public int numberBones = 4;

		private List<Vector3> outCoordinates = new List<Vector3>();
		public List<GameObject> bones = new List<GameObject>();
		static private string _puppet2DPath;

#if UNITY_EDITOR
		public List<GameObject> Create()
		{
			RecursivelyFindFolderPath("Assets");

			bones.Clear();

			CatmullRom(_splineCTRLS, out outCoordinates, numberBones);

			int j = 0;
			foreach (Vector3 outCoord in outCoordinates)
			{
				GameObject newGO = new GameObject(GetUniqueName("bone"));
				Undo.RegisterCreatedObjectUndo(newGO, "Created bone");
				SpriteRenderer spriteRenderer = newGO.AddComponent<SpriteRenderer>();
				string path = "";

				path = (_puppet2DPath + "/Textures/GUI/BoneNoJoint.psd");

				Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
				spriteRenderer.sprite = sprite;
				newGO.transform.position = outCoord;
				bones.Add(newGO);

				if (j > 0)
					newGO.transform.parent = bones[j - 1].transform;


				GameObject newInvisibleBone = new GameObject(GetUniqueName("hiddenBone"));
				Undo.RegisterCreatedObjectUndo(newInvisibleBone, "Created new invisible Bone");
				SpriteRenderer spriteRendererInvisbile = newInvisibleBone.AddComponent<SpriteRenderer>();
				newInvisibleBone.transform.position = new Vector3(10000, 10000, 10000);

				path = (_puppet2DPath + "/Textures/GUI/BoneJoint.psd");
				Sprite boneHiddenSprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
				spriteRendererInvisbile.sprite = boneHiddenSprite;
				newInvisibleBone.transform.parent = newGO.transform.parent;
				Puppet2D_HiddenBone hiddenBoneComp = newInvisibleBone.AddComponent<Puppet2D_HiddenBone>();
				hiddenBoneComp.boneToAimAt = newGO.transform;
				hiddenBoneComp.Refresh();


				j++;
			}
			return bones;
		}
#endif
		string GetUniqueName(string name)
		{
			string nameToAdd = name;
			int nameToAddLength = nameToAdd.Length + 1;
			int index = 0;
			foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (go.name.StartsWith(nameToAdd))
				{
					string endOfName = go.name.Substring(nameToAddLength, go.name.Length - nameToAddLength);

					int indexTest = 0;
					if (int.TryParse(endOfName, out indexTest))
					{
						if (int.Parse(endOfName) > index)
						{
							index = int.Parse(endOfName);
						}
					}


				}
			}
			index++;
			return (name + "_" + index);

		}
		public void Run()
		{
			Quaternion angleOffset = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
			CatmullRom(_splineCTRLS, out outCoordinates, numberBones);

			for (int i = 0; i < outCoordinates.Count; i++)
			{
				Vector3 outCoord = outCoordinates[i];

				bones[i].transform.position = outCoord;
				if (i < outCoordinates.Count - 1)
				{
					if (i == 0 && outCoordinates.Count > 2)
						bones[i].transform.rotation = _splineCTRLS[1].transform.rotation;
					else
						bones[i].transform.rotation = Quaternion.LookRotation(outCoordinates[i] - outCoordinates[i + 1], Vector3.forward) * Quaternion.AngleAxis(90, Vector3.left) * angleOffset;

					//bones[i].transform.rotation =Quaternion.Inverse(_splineCTRLS[1].transform.rotation) * transform.rotation*Quaternion.LookRotation(outCoordinates [i] - outCoordinates [i+1], Vector3.forward)* Quaternion.AngleAxis(90, Vector3.left);

				}
				else
				{
					bones[i].transform.rotation = _splineCTRLS[_splineCTRLS.Count - 2].transform.rotation;

				}
			}
		}
		public static bool CatmullRom(List<Transform> inCoordinates, out List<Vector3> outCoordinates, int samples)
		{
			if (inCoordinates.Count < 4)
			{
				outCoordinates = null;
				return false;
			}

			List<Vector3> results = new List<Vector3>();

			for (int n = 1; n < inCoordinates.Count - 2; n++)
				for (int i = 0; i < samples; i++)
					results.Add(PointOnCurve(inCoordinates[n - 1].position, inCoordinates[n].position, inCoordinates[n + 1].position, inCoordinates[n + 2].position, (1f / samples) * i));

			results.Add(inCoordinates[inCoordinates.Count - 2].position);

			outCoordinates = results;
			return true;
		}


		public static Vector3 PointOnCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			Vector3 result = new Vector3();

			float t0 = ((-t + 2f) * t - 1f) * t * 0.5f;
			float t1 = (((3f * t - 5f) * t) * t + 2f) * 0.5f;
			float t2 = ((-3f * t + 4f) * t + 1f) * t * 0.5f;
			float t3 = ((t - 1f) * t * t) * 0.5f;

			result.x = p0.x * t0 + p1.x * t1 + p2.x * t2 + p3.x * t3;
			result.y = p0.y * t0 + p1.y * t1 + p2.y * t2 + p3.y * t3;
			result.z = p0.z * t0 + p1.z * t1 + p2.z * t2 + p3.z * t3;

			return result;
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
	}
}