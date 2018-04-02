using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_EditSkinWeights : MonoBehaviour
	{
		public GameObject Bone0, Bone1, Bone2, Bone3;
		public int boneIndex0, boneIndex1, boneIndex2, boneIndex3;
		public float Weight0, Weight1, Weight2, Weight3;
		public Mesh mesh;
		public SkinnedMeshRenderer meshRenderer;
		public int vertNumber;
		GameObject[] handles;
		public Vector3[] verts;
		static private Mesh skinnedMesh;
		public bool autoUpdate = false;
#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			if (GetComponent<Renderer>().enabled)
			{
				transform.GetComponent<SpriteRenderer>().color = Color.green;
			}
		}

		void OnDrawGizmos()
		{
			if (GetComponent<Renderer>().enabled)
			{
				transform.GetComponent<SpriteRenderer>().color = Color.white;

			}
		}
#endif
		void Update()
		{
			Refresh();
		}
		public void Refresh()
		{
			if (transform.parent)
				if (transform.parent.GetComponent<SkinnedMeshRenderer>())
					meshRenderer = transform.parent.GetComponent<SkinnedMeshRenderer>();
			BoneWeight[] boneWeights = mesh.boneWeights;

			if (Bone0)
				boneWeights[vertNumber].boneIndex0 = boneIndex0;
			if (Bone1)
				boneWeights[vertNumber].boneIndex1 = boneIndex1;
			if (Bone2)
				boneWeights[vertNumber].boneIndex2 = boneIndex2;
			if (Bone3)
				boneWeights[vertNumber].boneIndex3 = boneIndex3;

			boneWeights[vertNumber].weight0 = Weight0;
			boneWeights[vertNumber].weight1 = Weight1;
			boneWeights[vertNumber].weight2 = Weight2;
			boneWeights[vertNumber].weight3 = Weight3;

			mesh.boneWeights = boneWeights;
			if (meshRenderer != null)
				meshRenderer.sharedMesh = mesh;
		}
	}
}