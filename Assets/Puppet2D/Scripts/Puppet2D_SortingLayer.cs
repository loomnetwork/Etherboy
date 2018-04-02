using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Puppet2D
{

	public class Puppet2D_SortingLayer : MonoBehaviour
	{
		[HideInInspector]
		public Vector2 offsetAmount;
		[HideInInspector]
		public Vector2[] uvsDefault;
		[HideInInspector]
		public bool initialized = false;
		[HideInInspector]
		public Bounds bounds;
		//	public Material[] swappedMaterials;
		//	[HideInInspector]
		//	public int swappedMaterialIndex = 0;
		//	[HideInInspector]
		//	public List<String> swappedMaterialStrings = new List<string>();
		//	public void OnValidate()
		//	{
		//		swappedMaterialStrings.Clear ();
		//		if (swappedMaterials != null) 
		//		{
		//			foreach (Material m in swappedMaterials)
		//			{
		//				if(m!=null)
		//					swappedMaterialStrings.Add (m.name);
		//			}
		//		}
		//
		//
		//	}
	}
}
