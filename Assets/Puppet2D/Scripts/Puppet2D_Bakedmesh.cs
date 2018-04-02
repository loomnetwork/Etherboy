using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_Bakedmesh : MonoBehaviour
	{

		// Use this for initialization
		public SkinnedMeshRenderer skin;
		//public Mesh bakedMesh;

		void Start()
		{
			skin = transform.GetComponent<SkinnedMeshRenderer>();

		}

		// Update is called once per frame
		void Update()
		{
			if (skin)
			{
				Mesh baked = new Mesh();
				skin.BakeMesh(baked);
				//bakedMesh = baked;
				//GameObject[] handles = GameObject.FindGameObjectsWithTag ("handle");
				int i = 0;
				foreach (Transform child in transform)
				{
					if (!System.Single.IsNaN(baked.vertices[i].x))
						child.localPosition = baked.vertices[i];
					else
						Debug.LogWarning("vertex " + i + " is corrupted");
					i++;
				}
				DestroyImmediate(baked);
			}
		}
	}

}