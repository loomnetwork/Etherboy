using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Puppet2D
{
	[CustomEditor(typeof(Puppet2D_HiddenBone))]
	public class Puppet2D_HiddenBoneEditor : Editor
	{

		Puppet2D_HiddenBone handle;
		GameObject[] newSelection;
		void OnEnable()
		{
			handle = (Puppet2D_HiddenBone)target;
		}
		public void OnSceneGUI()
		{
			GameObject[] newSelection = Selection.gameObjects;
			for (int i = 0; i < newSelection.Length; i++)
			{
				if (newSelection[i].name == handle.gameObject.name)
					newSelection[i] = handle.boneToAimAt.parent.gameObject;
			}
			Selection.objects = newSelection;

		}

	}
}