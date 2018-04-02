using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_HiddenBone : MonoBehaviour
	{
		public Transform boneToAimAt;
		public bool InEditBoneMode = false;
		public GameObject[] _newSelection;

#if UNITY_EDITOR
		void LateUpdate()
		{
			if (!Application.isPlaying)
			{

				if (GetComponent<Renderer>().enabled)
				{
					if ((boneToAimAt) && (transform.parent))
					{
						Transform myParent = transform.parent;
						transform.parent = null;

						float dist = Vector3.Distance(boneToAimAt.position, transform.position);
						if (dist > 0)
							transform.rotation = Quaternion.LookRotation(boneToAimAt.position - transform.position, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.right);

						float length = (boneToAimAt.position - transform.position).magnitude;

						// float length = (boneToAimAt.position - transform.position).magnitude;
						transform.localScale = new Vector3(length, length, length);
						if (myParent)
						{
							transform.parent = myParent;
							transform.position = myParent.position;
							if (myParent.GetComponent<SpriteRenderer>())
								transform.GetComponent<SpriteRenderer>().sortingLayerName = myParent.GetComponent<SpriteRenderer>().sortingLayerName;

						}
						transform.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
						//transform.hideFlags = HideFlags.None;
					}
					else
					{
						DestroyImmediate(gameObject);
					}


				}
			}

		}
#endif
		public void Refresh()
		{
			if (boneToAimAt)
			{
				Transform myParent = transform.parent;
				transform.parent = null;

				float dist = Vector3.Distance(boneToAimAt.position, transform.position);
				if (dist > 0)
					transform.rotation = Quaternion.LookRotation(boneToAimAt.position - transform.position, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.right);

				float length = (boneToAimAt.position - transform.position).magnitude;

				// float length = (boneToAimAt.position - transform.position).magnitude;
				transform.localScale = new Vector3(length, length, length);
				if (myParent)
				{
					transform.parent = myParent;
					transform.position = myParent.position;
					if (myParent.GetComponent<SpriteRenderer>())
						transform.GetComponent<SpriteRenderer>().sortingLayerName = myParent.GetComponent<SpriteRenderer>().sortingLayerName;

				}

			}
			else
			{
				return;
			}
		}


	}

}