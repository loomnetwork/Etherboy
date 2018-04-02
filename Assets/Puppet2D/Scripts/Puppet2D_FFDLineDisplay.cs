
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_FFDLineDisplay : MonoBehaviour
	{
		[HideInInspector]
		public Transform target;
		[HideInInspector]
		public Transform target2;
		[HideInInspector]
		public SkinnedMeshRenderer skinnedMesh;
		//[HideInInspector]
		public SkinnedMeshRenderer outputSkinnedMesh;
		//[HideInInspector]
		public int vertNumber;


		public List<Transform> bones = new List<Transform>();
		public List<float> weight = new List<float>();
		public List<Vector3> delta = new List<Vector3>();

		private List<float> internalWeights = new List<float>();
		public void Init()
		{
			bones.Clear();
			weight.Clear();
			delta.Clear();
			internalWeights.Clear();

			Mesh mesh = skinnedMesh.sharedMesh;


			Vector3 position = mesh.vertices[vertNumber];
			//position = transform.TransformPoint(position);

			BoneWeight weights = mesh.boneWeights[vertNumber];

			int[] boneIndices = new int[] { weights.boneIndex0, weights.boneIndex1, weights.boneIndex2, weights.boneIndex3 };
			float[] boneWeights = new float[] { weights.weight0, weights.weight1, weights.weight2, weights.weight3 };
			boneWeights[1] = 1f - boneWeights[0];

			for (int j = 0; j < 4; j++)
			{
				if (boneWeights[j] > 0)
				{

					bones.Add(skinnedMesh.bones[boneIndices[j]]);
					weight.Add(boneWeights[j]);
					internalWeights.Add(boneWeights[j]);
					delta.Add(bones[bones.Count - 1].InverseTransformPoint(position));

				}
			}


		}
#if UNITY_EDITOR
		void OnEnable()
		{
			internalWeights.Clear();
			for (int i = 0; i < weight.Count; i++)
			{
				internalWeights.Add(weight[i]);

			}
		}
		void Awake()
		{
			internalWeights.Clear();
			for (int i = 0; i < weight.Count; i++)
			{
				internalWeights.Add(weight[i]);

			}
		}
#endif
		void OnValidate()
		{
			float TotalWeight = 0;
			float RemainingWeight = 1;

			for (int i = 0; i < weight.Count; i++)
			{
				if (internalWeights.Count > i)
				{
					if (internalWeights[i] == weight[i])
						TotalWeight += weight[i];
					else
						RemainingWeight -= weight[i];
				}
			}
			for (int i = 0; i < weight.Count; i++)
			{
				if (internalWeights.Count > i)
				{


					if (internalWeights[i] == weight[i])
					{
						if (TotalWeight <= 0)
							weight[i] = 0;
						else
							weight[i] = (weight[i] / TotalWeight) * RemainingWeight;
					}

					internalWeights[i] = weight[i];
				}


			}
		}

		public void Run()
		{
			if (bones.Count > 0)
			{

				Vector3 position = Vector3.zero;
				for (int i = 0; i < bones.Count; i++)
				{

					position += bones[i].TransformPoint(delta[i]) * weight[i];
				}
				transform.parent.position = position;
			}

		}
#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			if (GetComponent<Renderer>().enabled)
			{
				transform.GetComponent<SpriteRenderer>().color = Color.green;
			}
		}
#endif
		void OnDrawGizmos()
		{
			if (GetComponent<Renderer>().enabled)
			{
				transform.GetComponent<SpriteRenderer>().color = Color.white;
				if (target != null)
				{
					Gizmos.color = Color.white;
					Gizmos.DrawLine(transform.position, target.position);
				}
				if (target2 != null)
				{
					Gizmos.color = Color.white;
					Gizmos.DrawLine(transform.position, target2.position);
				}
			}
		}
	}
}