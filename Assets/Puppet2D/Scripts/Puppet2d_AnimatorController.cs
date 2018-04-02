using UnityEngine;
using System.Collections;
namespace Puppet2D
{
	public class Puppet2d_AnimatorController : MonoBehaviour
	{
		private Animator _animator;
		private Puppet2D_GlobalControl _globalControl;

		public float speed = 1.0f;

		private string axisName = "Horizontal";
		// Use this for initialization
		void Start()
		{
			_animator = gameObject.GetComponent<Animator>();
			_globalControl = gameObject.GetComponent<Puppet2D_GlobalControl>();
		}

		// Update is called once per frame
		void Update()
		{


			if (Input.GetAxis(axisName) < 0)
			{
				_globalControl.flip = true;
				_animator.SetFloat("Speed", 1);
				transform.position += transform.right * speed * Time.deltaTime;

			}
			else if (Input.GetAxis(axisName) > 0)
			{
				_animator.SetFloat("Speed", 1);
				_globalControl.flip = false;
				transform.position += transform.right * speed * Time.deltaTime;

			}
			else
				_animator.SetFloat("Speed", 0);





		}


	}

}