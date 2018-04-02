using UnityEngine;
using System.Collections;

namespace Puppet2D
{
	public class Puppet2D_ParentControl : MonoBehaviour
	{
		public GameObject bone;

		public bool IsEnabled;
		public bool Point;
		public bool Orient;
		public bool Scale;
		public bool ConstrianedPosition;
		public bool ConstrianedOrient;
		public bool MaintainOffset;
		public Vector3 OffsetPos;
		public Vector3 OffsetScale = new Vector3(1, 1, 1);
		public Quaternion OffsetOrient;

		/* void LateUpdate () 
		 {
			 if (!IsEnabled)
			 {
				 return;
			 }
			 ParentControlRun();
		 }
		 */
		public void ParentControlRun()
		{

			if (Orient)
			{
				if (MaintainOffset)
					bone.transform.rotation = transform.rotation * OffsetOrient;
				else
					bone.transform.rotation = transform.rotation;
			}
			if (Point)
			{
				if (MaintainOffset)
					bone.transform.position = transform.TransformPoint(OffsetPos);
				else
					bone.transform.position = transform.position;

			}
			if (Scale)
				bone.transform.localScale = new Vector3(transform.localScale.x * OffsetScale.x, transform.localScale.y * OffsetScale.y, transform.localScale.z * OffsetScale.z);

			if (ConstrianedPosition)
				if (!Point)
					transform.position = bone.transform.position;
		}
	}

}