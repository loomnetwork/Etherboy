using UnityEngine;
using System.Collections;
namespace Puppet2D
{
	[System.Serializable]
	public class AngleLimitNode
	{
		public Transform Transform;
		public float min;
		public float max;
	}
}