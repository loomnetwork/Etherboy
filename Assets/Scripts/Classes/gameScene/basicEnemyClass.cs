using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puppet2D;

public class basicEnemyClass : MonoBehaviour {
	public Collider2D assignedPlatform;

	public float movLimitLeft;
	public float movLimitRight;
	public float movSpeedX;

	public float attackRadius;

	private Animator thisAnimator;
	private string state;
	private float hitTime;
	private float attackTime;
	private Puppet2D_GlobalControl thisPuppetControl;
	private Rigidbody2D thisBody;
	private ContactFilter2D thisContact;

	private GameObject character;

	private int life = 4;

	private Vector2 basePosition;
	private float attackDuration;

	private float multiTaskTimer;
	private float attackDelayTime;

	// Use this for initialization
	void Start () {
		basePosition = transform.localPosition;
		thisAnimator = transform.GetChild(0).GetComponent<Animator> ();
		thisBody = GetComponent<Rigidbody2D> ();
		thisPuppetControl = transform.GetChild(0).GetComponent<Puppet2D_GlobalControl> ();

		state = "normal";

		if (transform.name == "snail") {
			attackDuration = 1.6f;
			life = 1;
			attackDelayTime = 1;
		} else if (transform.name == "slime") {
			attackDuration = 1;
			life = 2;
			attackDelayTime = 0;
		} else if (transform.name == "leaf") {
			attackDuration = 1;
			life = 1;
			attackDelayTime = 0;
		} else if (transform.name == "spike") {
			attackDuration = 2.55f;
			life = 1;
			attackDelayTime = 0;
		}

		if (movSpeedX > 0) {
			thisPuppetControl.flip = true;
		}

		multiTaskTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == "normal") {
			if (!thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Running")) {
				thisAnimator.Play ("Running", -1, 0f);
			}

			if (character != null) {
				if (character.transform.localPosition.x + 0.3f < transform.localPosition.x) {
					movSpeedX = -Mathf.Abs (movSpeedX);
				} else if (character.transform.localPosition.x - 0.3f > transform.localPosition.x) {
					movSpeedX = Mathf.Abs (movSpeedX);
				}
			}

			Vector2 currPos = transform.localPosition;
			currPos.x += movSpeedX;
			if (currPos.x < basePosition.x - movLimitLeft) {
				movSpeedX = Mathf.Abs (movSpeedX);
			} else if (currPos.x > basePosition.x + movLimitRight) {
				movSpeedX = -Mathf.Abs (movSpeedX);
			}

			transform.localPosition = currPos;

			if (transform.name == "leaf") {
				multiTaskTimer += Time.deltaTime;
				if (multiTaskTimer > 1f) {
					multiTaskTimer = 0;
					thisBody.velocity = new Vector2 (0, 6);
				} 
			}

			if (character != null) {
				if (character.transform.localPosition.x + 0.3f < transform.localPosition.x) {
					movSpeedX = -Mathf.Abs (movSpeedX);
				} else if (character.transform.localPosition.x - 0.3f > transform.localPosition.x) {
					movSpeedX = Mathf.Abs (movSpeedX);
				}

				if ((thisPuppetControl.flip && character.transform.position.x > transform.position.x) || (!thisPuppetControl.flip && character.transform.position.x < transform.position.x)) {
					if ((Mathf.Abs (Mathf.Abs (transform.position.x) - Mathf.Abs (character.transform.position.x))) < attackRadius) {
						state = "attack";
						thisAnimator.Play ("Attack", -1, 0f);
						attackTime = 0;
					}
				}
			}

			if (movSpeedX > 0) {
				thisPuppetControl.flip = true;
			} else if (movSpeedX < 0) {
				thisPuppetControl.flip = false;
			}

		} else if (state == "hit") {
			if (!thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Hit")) {
				thisAnimator.Play ("Hit", -1, 0f);
				if (thisPuppetControl.flip) {
					thisBody.velocity = new Vector2 (-2.5f, 5);
				} else {
					thisBody.velocity = new Vector2 (2.5f, 5);
				}
			} else {
				hitTime = hitTime + Time.deltaTime;
				if (hitTime > 0.5f) {
					thisAnimator.Play ("Running", -1, 0f);
					hitTime = 0;
					state = "normal";
					if (transform.name == "leaf") {
						multiTaskTimer = 0;
					}
				}
			}
		} else if (state == "attack") {
			attackTime += Time.deltaTime;
			if (attackTime > attackDuration) {
				state = "attackDelay";
				attackTime = 0;
				if (transform.name == "leaf") {
					multiTaskTimer = 0;
				}
			} else if (attackTime > 0.7f) {
				if (transform.name == "snail") {
					thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
				}
			} else if (attackTime > 0.5f) {
				if (transform.name == "leaf") {
					if (thisPuppetControl.flip) {
						thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
					}
				}
			} else if (attackTime > 0.3f) {
				if (transform.name == "snail") {
					if (thisPuppetControl.flip) {
						thisBody.velocity = new Vector2 (6, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (-6, thisBody.velocity.y);
					}
				} else if (transform.name == "leaf") {
					if (thisPuppetControl.flip) {
						thisBody.velocity = new Vector2 (2, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (-2, thisBody.velocity.y);
					}
				}
			}
		} else if (state == "attackDelay") {
			attackTime += Time.deltaTime;
			if (attackTime > attackDelayTime) {
				state = "normal";
				attackTime = 0;
				if (transform.name == "leaf") {
					multiTaskTimer = 0;
				}
			}
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.collider.name == "sword1") {
			if (state != "hit") {
				state = "hit";
				hitTime = 0;

				if (collision.contacts [0].point.x > transform.position.x) {
					thisPuppetControl.flip = true;
				} else {
					thisPuppetControl.flip = false;
				}

				life -= 1;

				if (life <= 0) {
					Physics2D.IgnoreCollision (collision.collider, collision.otherCollider);
					state = "death";
					thisAnimator.Play ("Death", -1, 0f);
					gameObject.layer = LayerMask.NameToLayer("Default");

					LeanTween.alpha (gameObject, 0, 0.15f).setDelay (5);
					if (transform.name == "leaf") {
						LeanTween.scaleX (gameObject, 0, 0.15f).setDelay (5).setOnStart (() => {
							GetComponent<Collider2D> ().isTrigger = true;
						});
					} else {
						LeanTween.scaleY (gameObject, 0, 0.15f).setDelay (5).setOnStart (() => {
							GetComponent<Collider2D> ().isTrigger = true;
						});
					}
					if (transform.name == "leaf") {
						if (thisPuppetControl.flip == true) {
							LeanTween.rotateZ (gameObject, 90, 0.25f);
						} else if (thisPuppetControl.flip == false) {
							LeanTween.rotateZ (gameObject, -90, 0.25f);
						}
					}
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.name == "etherBoy") {
			character = collider.gameObject;
			/*if ((thisPuppetControl.flip && character.transform.position.x > transform.position.x) || (!thisPuppetControl.flip && character.transform.position.x < transform.position.x)) {
				character = collider.gameObject;
			} else {
				character = null;
			}*/
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.name == "etherBoy") {
			character = null;
		}
	}
}
