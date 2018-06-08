using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Anim_Sys
{
	[ExecuteInEditMode]
	public class Anim_GlobalControl : MonoBehaviour
	{

		public float startRotationY;

		public bool flip = false;
		private bool internalFlip = false;
		public bool lateUpdate = true;

		[HideInInspector]
		public int _flipCorrection = 1;

		private Transform myTransform;

		void Awake()
		{
			//ControlsEnabled = false;
			this.myTransform = this.GetComponent<Transform>();

			internalFlip = flip;
		}

		void Update()
		{
			if (internalFlip != flip)
			{
				if (flip)
				{

					transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
					transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY + 180, transform.rotation.eulerAngles.z);

				}
				else
				{

					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
					transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY, transform.rotation.eulerAngles.z);
				}
				internalFlip = flip;
			}
		}
		void LateUpdate()
		{
			if (lateUpdate)
			{
				if (internalFlip != flip) {
					if (flip) {

						transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, -transform.localScale.z);
						transform.localEulerAngles = new Vector3 (transform.rotation.eulerAngles.x, startRotationY + 180, transform.rotation.eulerAngles.z);

					} else {

						transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), Mathf.Abs (transform.localScale.y), Mathf.Abs (transform.localScale.z));
						transform.localEulerAngles = new Vector3 (transform.rotation.eulerAngles.x, startRotationY, transform.rotation.eulerAngles.z);
					}
					internalFlip = flip;
				}
			}
		}
	}

}