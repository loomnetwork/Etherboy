using UnityEngine;
using System.Collections;
namespace Puppet2D
{
	[ExecuteInEditMode]

	public class Puppet2D_VertHandler : MonoBehaviour
	{
		Mesh mesh;
		Vector3[] verts;
		Vector3 vertPos;
		GameObject[] handles;

		void OnEnable()
		{
			mesh = GetComponent<MeshFilter>().sharedMesh;
			verts = mesh.vertices;
			foreach (Vector3 vert in verts)
			{
				vertPos = transform.TransformPoint(vert);
				GameObject handle = new GameObject("handle");
				handle.transform.position = vertPos;
				handle.transform.parent = transform;
				handle.tag = "handle";
				//handle.AddComponent<Gizmo_Sphere>();
				print(vertPos);
			}
		}

		//    void OnDisable()
		//    {
		//        GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
		//        foreach(GameObject handle in handles)
		//        {
		//            DestroyImmediate(handle);    
		//        }
		//    }

		void Update()
		{
			handles = GameObject.FindGameObjectsWithTag("handle");
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i] = handles[i].transform.localPosition;
			}
			mesh.vertices = verts;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}
	}
}