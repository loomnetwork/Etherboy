using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Puppet2D_FFDLineDisplay))]
	public class Puppet2D_FFDLineDisplayEditor : Editor
	{
		/*Puppet2D_FFDLineDisplay myTarget;
		public int bone0,bone1,bone2,bone3;

		List<string> boneNames = new List<string>();

		public void OnEnable()
		{
			myTarget = (Puppet2D_FFDLineDisplay) target;
			if (boneNames.Count == 0)
			{    

				for (int i = 0; i < myTarget.allBones.Count; i++)
				{
					boneNames.Add(myTarget.allBones[i].name);
				}

			}
		}*/
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			/*for (int j = 0; j < myTarget.bones.Count; j++)
			{

				myTarget.bones[j] = EditorGUILayout.Popup(myTarget.bones[j], boneNames.ToArray());
			}*/

		}


	}
}