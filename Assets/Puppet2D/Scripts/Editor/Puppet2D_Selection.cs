using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet2D
{
	public class Puppet2D_Selection : Editor
	{

		public static void SaveSelectionLoad(object index)
		{
			Puppet2D_Selection.GetSelectionString();
			string[] goNames = Puppet2D_Editor.selectedControls[(int)index].ToArray();
			List<GameObject> gos = new List<GameObject>();
			foreach (string goName in goNames)
				gos.Add(GameObject.Find(goName));
			Selection.objects = gos.ToArray();
			gos.Clear();
			SetSelectionString();
		}
		public static void SaveSelectionRemove(object index)
		{
			Puppet2D_Editor.selectedControls.RemoveAt((int)index);
			Puppet2D_Editor.selectedControlsData.RemoveAt((int)index);

			SetSelectionString();
		}
		public static void SaveSelectionAppend(object index)
		{
			foreach (GameObject go in Selection.gameObjects)
			{
				Puppet2D_Editor.selectedControls[(int)index].Add(GetGameObjectPath(go));
				Puppet2D_Editor.selectedControlsData[(int)index].Add(go.transform.localPosition.x + " " + go.transform.localPosition.y + " " + go.transform.localPosition.z + " " + go.transform.localRotation.x + " " + go.transform.localRotation.y + " " + go.transform.localRotation.z + " " + go.transform.localRotation.w + " " + go.transform.localScale.x + " " + go.transform.localScale.y + " " + go.transform.localScale.z + " ");

			}
			SetSelectionString();
		}
		public static void SetSelectionString()
		{
			string selectedControlString = "";
			string selectedControlDataString = "";
			for (int i = 0; i < Puppet2D_Editor.selectedControls.Count; i++)
			{
				for (int j = 0; j < Puppet2D_Editor.selectedControls[i].Count; j++)
				{
					if (Puppet2D_Editor.selectedControls[i][j] != "")
					{
						if (j < Puppet2D_Editor.selectedControls[i].Count - 1)
						{
							selectedControlString += Puppet2D_Editor.selectedControls[i][j] + "|";
							selectedControlDataString += Puppet2D_Editor.selectedControlsData[i][j] + "|";

						}
						else
						{
							selectedControlString += Puppet2D_Editor.selectedControls[i][j];
							selectedControlDataString += Puppet2D_Editor.selectedControlsData[i][j];
						}
					}
				}

				if (i < Puppet2D_Editor.selectedControls.Count - 1)
				{
					selectedControlString += ":";
					selectedControlDataString += ":";

				}
			}
			EditorPrefs.SetString("Puppet2D_selectedControls", selectedControlString);
			EditorPrefs.SetString("Puppet2D_selectedControlsData", selectedControlDataString);

		}
		public static void GetSelectionString()
		{
			string selectedControlsString = EditorPrefs.GetString("Puppet2D_selectedControls", "");
			string[] selectedControlsStringArrays = selectedControlsString.Split(':');

			string selectedControlsDataString = EditorPrefs.GetString("Puppet2D_selectedControlsData", "");
			string[] selectedControlsDataStringArrays = selectedControlsDataString.Split(':');

			Puppet2D_Editor.selectedControls.Clear();
			Puppet2D_Editor.selectedControlsData.Clear();

			if (selectedControlsStringArrays[0] == "")
				return;

			for (int i = 0; i < selectedControlsStringArrays.Length; i++)
			{
				string selectedControlsStringArray = selectedControlsStringArrays[i];
				Puppet2D_Editor.selectedControls.Add(new List<string>());
				Puppet2D_Editor.selectedControlsData.Add(new List<string>());

				string[] goNames = selectedControlsStringArray.Split('|');
				string[] data = selectedControlsDataStringArrays[i].Split('|');

				for (int j = 0; j < goNames.Length; j++)
				{
					string goName = goNames[j];
					Puppet2D_Editor.selectedControls[Puppet2D_Editor.selectedControls.Count - 1].Add(goName);
					Puppet2D_Editor.selectedControlsData[Puppet2D_Editor.selectedControlsData.Count - 1].Add(data[j]);
				}
			}
		}
		public static string GetGameObjectPath(GameObject obj)
		{
			if (obj == null)
				return "";
			string path = "/" + obj.name;
			while (obj.transform.parent != null)
			{
				obj = obj.transform.parent.gameObject;
				path = "/" + obj.name + path;
			}
			return path;
		}

		public static void StorePose(object index)
		{
			string[] goNames = Puppet2D_Editor.selectedControls[(int)index].ToArray();
			for (int i = 0; i < goNames.Length; i++)
			{
				string goName = goNames[i];
				GameObject go = GameObject.Find(goName);
				Puppet2D_Editor.selectedControlsData[(int)index][i] = (go.transform.localPosition.x + " " + go.transform.localPosition.y + " " + go.transform.localPosition.z + " " + go.transform.localRotation.x + " " + go.transform.localRotation.y + " " + go.transform.localRotation.z + " " + go.transform.localRotation.w + " " + go.transform.localScale.x + " " + go.transform.localScale.y + " " + go.transform.localScale.z + " ");
				//Debug.Log("0 " + go.transform.localPosition.x + " 1 " + go.transform.localPosition.y + " 2 " + go.transform.localPosition.z);
			}
			SetSelectionString();
		}
		public static void LoadPose(object index)
		{
			//Puppet2D_Selection.GetSelectionString();
			string[] goNames = Puppet2D_Editor.selectedControls[(int)index].ToArray();
			string[] data = Puppet2D_Editor.selectedControlsData[(int)index].ToArray();
			List<GameObject> gos = new List<GameObject>();
			foreach (string goName in goNames)
				gos.Add(GameObject.Find(goName));
			for (int i = 0; i < gos.Count; i++)
			{
				string[] dataSplit = data[i].Split(' ');
				//Debug.Log( "0 " + dataSplit[0] + " 1 " + dataSplit[1]  + " 2 " + dataSplit[2]);
				gos[i].transform.localPosition = new Vector3(float.Parse(dataSplit[0]), float.Parse(dataSplit[1]), float.Parse(dataSplit[2])); ;
				gos[i].transform.localRotation = new Quaternion(float.Parse(dataSplit[3]), float.Parse(dataSplit[4]), float.Parse(dataSplit[5]), float.Parse(dataSplit[6])); ;
				gos[i].transform.localScale = new Vector3(float.Parse(dataSplit[7]), float.Parse(dataSplit[8]), float.Parse(dataSplit[9])); ;

			}
			//Selection.objects = gos.ToArray(); 
			//gos.Clear();
			//SetSelectionString();
		}

	}
}