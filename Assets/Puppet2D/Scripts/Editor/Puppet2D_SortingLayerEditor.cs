using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

using UnityEditorInternal;

using System.Reflection;
namespace Puppet2D
{
	[CustomEditor(typeof(Puppet2D_SortingLayer)), CanEditMultipleObjects]
	public class Puppet2D_SortingLayerEditor : Editor
	{

		// FT code by Daniel Wiedemann
		// cache renderers
		private Renderer[] _renderers = null;
		private Renderer[] renderers
		{
			get
			{
				if (_renderers == null)
				{
					_renderers = new Renderer[targets.Length];

					for (int i = 0; i < targets.Length; i++)
					{
						_renderers[i] = (targets[i] as Puppet2D_SortingLayer).GetComponent<Renderer>();
					}
				}

				return _renderers;
			}
		}

		string[] sortingLayerNames;//we load here our Layer names to be displayed at the popup GUI
								   // FT code
								   // sortingLayerNames and "--" at index 0
		string[] sortingLayerNamesAndMultiObjectDifferent;

		public int popupMenuIndex;
		public int orderInLayer;

		public Vector2 offsetAmount;
		public Vector2[] uvsDefault;

		void OnEnable()
		{

			sortingLayerNames = GetSortingLayerNames(); //First we load the name of our layers

			// FT code
			// create sortingLayerNamesAndMultiObjectDifferent array for menu ("--" added at index 0 for when different values are set in multiple targets)
			sortingLayerNamesAndMultiObjectDifferent = new string[sortingLayerNames.Length + 1];
			sortingLayerNamesAndMultiObjectDifferent[0] = "--";
			sortingLayerNames.CopyTo(sortingLayerNamesAndMultiObjectDifferent, 1);


			var renderer = (target as Puppet2D_SortingLayer).gameObject.GetComponent<Renderer>();
			if (!renderer)
			{
				return;
			}
			//popupMenuIndex = renderer.sortingLayerID;
			//orderInLayer = renderer.sortingOrder;	
			SetSortingLayer(renderer.sortingLayerName, renderer.sortingOrder);

			if (!(target as Puppet2D_SortingLayer).initialized)
			{
				(target as Puppet2D_SortingLayer).uvsDefault = (target as Puppet2D_SortingLayer).gameObject.GetComponent<MeshFilter>().sharedMesh.uv;
				(target as Puppet2D_SortingLayer).initialized = true;
			}
		}

		public void SetSortingLayer(string sortingLayerName, int orderInLayerSet)
		{
			for (int i = 0; i < sortingLayerNames.Length; i++)
			{
				if (sortingLayerNames[i] == sortingLayerName)
					popupMenuIndex = i;
			}
			orderInLayer = orderInLayerSet;
		}

		public override void OnInspectorGUI()

		{
			var renderer = (target as Puppet2D_SortingLayer).gameObject.GetComponent<Renderer>();

			// If there is no renderer, we can't do anything
			if (!renderer)
			{
				return;
			}



			// single target
			if (renderers.Length == 1)
			{
				// Expose the sorting layer name
				popupMenuIndex = EditorGUILayout.Popup("Sorting Layer", popupMenuIndex, sortingLayerNames);//The popup menu is displayed simple as that

				// if (sortingLayerNames [popupMenuIndex] != renderer.sortingLayerName) {

				/*if (popupMenuIndex != renderer.sortingLayerID) {

				renderer.sortingLayerID = popupMenuIndex;

				EditorUtility.SetDirty(renderer);
				}*/

				if (sortingLayerNames[popupMenuIndex] != renderer.sortingLayerName)
				{
					Undo.RecordObject(renderer, "Edit Sorting Layer Name");
					renderer.sortingLayerName = sortingLayerNames[popupMenuIndex];
					EditorUtility.SetDirty(renderer);
				}

				int newSortingLayerOrder = orderInLayer;
				newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.sortingOrder);
				if (newSortingLayerOrder != renderer.sortingOrder)
				{
					Undo.RecordObject(renderer, "Edit Sorting Order");
					renderer.sortingOrder = newSortingLayerOrder;
					EditorUtility.SetDirty(renderer);
				}
			}
			// FT code
			// multiple targets
			else if (renderers.Length > 1)
			{
				// sorting layer

				// just use popupMenuIndex at first and shift it + 1
				int currentMenuIndex = popupMenuIndex + 1;

				// check if values are different in mutiple targets, if they are set currentMenuIndex = 0 "--"
				for (int i = 1; i < renderers.Length; i++)
				{
					if (renderers[i].sortingLayerName != renderers[i - 1].sortingLayerName)
					{
						currentMenuIndex = 0;
						break;
					}
				}

				// render GUI popup
				int newCurrentMenuIndex = EditorGUILayout.Popup("Sorting Layer", currentMenuIndex, sortingLayerNamesAndMultiObjectDifferent);

				// apply value change, when newCurrentMenuIndex is not 0 "--" and the value got actually changed
				if (newCurrentMenuIndex != 0 && newCurrentMenuIndex != currentMenuIndex)
				{
					for (int i = 0; i < renderers.Length; i++)
					{
						Undo.RecordObject(renderers[i], "Edit Sorting Layer Name");
						renderers[i].sortingLayerName = sortingLayerNames[newCurrentMenuIndex - 1];
						EditorUtility.SetDirty(renderers[i]);
					}

					// to clean up
					popupMenuIndex = newCurrentMenuIndex - 1;
				}


				//sorting layer order

				// just use popupMenuIndex at first and shift it + 1
				bool currentSortingLayerOrdersEqual = true;

				// check if values are different in mutiple targets, if they are set currentSortingLayerOrdersEqual = false
				for (int i = 1; i < renderers.Length; i++)
				{
					if (renderers[i].sortingOrder != renderers[i - 1].sortingOrder)
					{
						currentSortingLayerOrdersEqual = false;
						break;
					}
				}

				// show IntField only when values are the same
				if (currentSortingLayerOrdersEqual)
				{
					int newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", orderInLayer);

					// apply changes if value changed
					if (newSortingLayerOrder != orderInLayer)
					{
						for (int i = 0; i < renderers.Length; i++)
						{
							Undo.RecordObject(renderers[i], "Edit Sorting Order");
							renderers[i].sortingOrder = newSortingLayerOrder;
							EditorUtility.SetDirty(renderers[i]);
						}

						// to clean up
						orderInLayer = newSortingLayerOrder;
					}
				}
				else
				{
					EditorGUILayout.LabelField(new GUIContent("Sorting Layer Order varies in selected objects.", "Select objects with the same value to edit multiple objects."));
				}
			}
			EditorGUI.BeginChangeCheck();
			(target as Puppet2D_SortingLayer).offsetAmount = EditorGUILayout.Vector2Field("Offset UVs", (target as Puppet2D_SortingLayer).offsetAmount);
			if (EditorGUI.EndChangeCheck())
			{
				OffsetUVs();
			}


			//popupMenuIndex = EditorGUILayout.Popup("Sorting Layer", popupMenuIndex, sortingLayerNames);//The popup menu is displayed simple as that
		}

		public void OffsetUVs()
		{
			var renderer = (target as Puppet2D_SortingLayer).gameObject.GetComponent<Renderer>();

			Mesh meshToEdit = (target as Puppet2D_SortingLayer).gameObject.GetComponent<MeshFilter>().sharedMesh;

			Vector2[] uvs = new Vector2[meshToEdit.vertexCount];
			for (int i = 0; i < meshToEdit.vertices.Length; i++)
			{
				uvs[i] = (target as Puppet2D_SortingLayer).uvsDefault[i] + (target as Puppet2D_SortingLayer).offsetAmount;

			}
			meshToEdit.uv = uvs;
			//string path = AssetDatabase.GetAssetPath (meshToEdit);

			EditorUtility.SetDirty(meshToEdit);
			EditorUtility.SetDirty(renderer);
			EditorUtility.SetDirty(renderer.gameObject);

			AssetDatabase.SaveAssets();
			AssetDatabase.SaveAssets();
			serializedObject.ApplyModifiedProperties();
		}

		// Get the sorting layer names

		public string[] GetSortingLayerNames()

		{

			Type internalEditorUtilityType = typeof(InternalEditorUtility);

			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

			return (string[])sortingLayersProperty.GetValue(null, new object[0]);

		}
	}

}
