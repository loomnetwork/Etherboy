using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_SkinCluster : MonoBehaviour
	{
		public List<GameObject> bones = new List<GameObject>();



		void Update()
		{
			Mesh mesh = transform.GetComponent<MeshFilter>().sharedMesh;
			Vector3[] verts = mesh.vertices;
			for (int i = 0; i < verts.Length; i++)
			{
				Vector3 vert = verts[i];
				float testdist = 1000000;
				GameObject closestBone = null;
				foreach (GameObject bone in bones)
				{
					float dist = Vector2.Distance(new Vector2(bone.GetComponent<Renderer>().bounds.center.x, bone.GetComponent<Renderer>().bounds.center.y), new Vector2(vert.x, vert.y));
					if (dist < testdist)
					{
						testdist = dist;
						//Debug.Log("closest bone to " + mesh.name + " is " + bone.name + " distance " + dist);
						closestBone = bone;
					}
				}
				verts[i].x = closestBone.transform.position.x;
				verts[i].y = closestBone.transform.position.y;
			}

			mesh.vertices = verts;
		}
	}
}