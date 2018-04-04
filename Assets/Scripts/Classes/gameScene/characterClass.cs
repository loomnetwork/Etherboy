using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Puppet2D;

public class characterClass : MonoBehaviour {
	public SpriteRenderer keepRendInBounds;

	private GameObject thisParent;
	private Vector2 leftBottomScreen;
	private Vector2 rightTopScreen;

	private Rigidbody2D thisBody;
	private Collider2D thisCollider;

	private LTDescr cameraLT;

	private int canJump;

	private Collision2D collisionOccured;
	private Collider2D triggerOccured;

	private Collider2D lastPlatform;

	private Transform baseParent;

	private float jumpStrength = 9;
	private float movSpeedX;
	private float movSpeedY;

	private Puppet2D_GlobalControl characterAnimScript;
	private Animator characterAnimator;
	private float attackTime; 
	private float hitTime; 
	private bool playJumpOnce;

	private Animator ropeAnimator;

	private float prevScreenWidth;

	public string state;

	// Use this for initialization
	void Start () {
		thisParent = transform.parent.gameObject;
		leftBottomScreen = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
		rightTopScreen = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));

		prevScreenWidth = Screen.width;

		thisBody = GetComponent<Rigidbody2D> ();
		thisCollider = GetComponent<Collider2D> ();

		canJump = 0;

		baseParent = transform.parent;

		movSpeedX = 0;
		movSpeedY = 0;

		state = "normal";

		characterAnimScript = transform.GetChild (1).GetComponent<Puppet2D_GlobalControl> ();
		characterAnimator = transform.GetChild (1).GetComponent<Animator> ();

		ropeAnimator = transform.GetChild (2).GetComponent<Animator> ();

		if (globalScript.previousScene != "") {
			GameObject spawnPoints = GameObject.Find ("spawnPoints");
			if (spawnPoints != null) {
				for (int i = 0; i < spawnPoints.transform.childCount; i++) {
					GameObject c = spawnPoints.transform.GetChild (i).gameObject;
					if (c.name == globalScript.previousScene) {
						transform.position = c.transform.position;
						GameObject orient = c.transform.GetChild (0).gameObject;
						if (orient.name == "back") {
							characterAnimScript.flip = true;
						}
						break;
					}
				}
			}
		}

		keepCameraOnCharacter (true);
	}
	
	// Update is called once per frame
	void Update () {
		keepCameraOnCharacter (false);

		leftBottomScreen = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
		rightTopScreen = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));

		if (state == "normal") {
			Vector2 currPos = transform.localPosition;
			float movementX = Input.GetAxis ("Horizontal");

			string animationToPlay = "";

			if (movementX > 0.1f) {
				movSpeedX += 0.01f;

				if (movSpeedX > 0.085f) {
					movSpeedX = 0.085f;
				}

				if (canJump > 0) {
					animationToPlay = "Running";
				}
			} else if (movementX < -0.1f) {
				movSpeedX -= 0.01f;

				if (movSpeedX < -0.085f) {
					movSpeedX = -0.085f;
				}

				if (canJump > 0) {
					animationToPlay = "Running";
				}
			} else {
				movSpeedX *= 0.7f;

				if (Mathf.Abs (movSpeedX) <= 0.05f) {
					if (canJump > 0) {
						animationToPlay = "Idle";
					}
				}
			}

			if (thisBody.velocity.y > 0.1f) {
				if (!playJumpOnce) {
					animationToPlay = "Jump";
					playJumpOnce = true;
				} else {
					animationToPlay = "";
				}
			} else if (thisBody.velocity.y < -0.1f) {
				if (!playJumpOnce) {
					animationToPlay = "Jump";
					playJumpOnce = true;
				} else {
					animationToPlay = "";
				}
			} else if (canJump > 0) {
				playJumpOnce = false;
			}

			bool pressedAttack = Input.GetButton ("Fire3");

			if (pressedAttack) {
				state = "attack";
				animationToPlay = "Attack";
				attackTime = 0;
			}
				
			currPos.x += movSpeedX; 
			transform.localPosition = currPos;

			if (movementX > 0) {
				characterAnimScript.flip = false;
			} else if (movementX < 0) {
				characterAnimScript.flip = true;
			}
				
			if (animationToPlay != "" && !characterAnimator.GetCurrentAnimatorStateInfo (0).IsName (animationToPlay)) {
				characterAnimator.Play (animationToPlay, -1, 0f);
			}

			float movementY = Input.GetAxis ("Vertical");

			if (movementY > 0f || movementY < 0f) {
				checkOverlapWithItem ();
			}
		} else if (state == "attack") {
			if (characterAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Attack")) {
				if (canJump <= 0) {
					Vector2 currPos = transform.localPosition;
					currPos.x += movSpeedX; 
					transform.localPosition = currPos;
				} else {
					movSpeedX = 0;
				}

				attackTime += Time.deltaTime;
				if (attackTime > 0.3f) {
					attackTime = 0;
					state = "normal";
					if (canJump <= 0) {
						characterAnimator.Play ("Jump");
					} else {
						characterAnimator.Play ("Idle", -1, 0f);
					}
				}
			}
				
		} else if (state == "hit") {
			if (!characterAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Hit")) {
				characterAnimator.Play ("Hit", -1, 0f);
				if (characterAnimScript.flip) {
					thisBody.velocity = new Vector2 (5, 5);
				} else {
					thisBody.velocity = new Vector2 (-5, 5);
				}
			} else {
				hitTime += Time.deltaTime;
				if (hitTime > 0.7f && canJump > 0) {
					thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
					hitTime = 0;
					state = "normal";
					characterAnimator.Play ("Idle", -1, 0f);
				}
			}
		} else if (state == "rope") {

			Vector2 currPos = transform.localPosition;

			float movementY = Input.GetAxis ("Vertical");

			if (movementY > 0.1f) {
				ropeAnimator.enabled = true;
				movSpeedY += 0.01f;

				if (movSpeedY > 0.07f) {
					movSpeedY = 0.07f;
				}
			} else if (movementY < -0.1f) {
				ropeAnimator.enabled = true;
				movSpeedY -= 0.01f;

				if (movSpeedY < -0.07f) {
					movSpeedY = -0.07f;
				}
			} else {
				ropeAnimator.enabled = false;
				movSpeedY *= 0.7f;
			}

			currPos.y += movSpeedY;
			transform.localPosition = currPos;

			checkOverlapWithItem ();

			float movementX = Input.GetAxis ("Horizontal");

			if (movementX != 0) {
				state = "normal";
				resetState ();
			}
		}
	}

	void resetState () {
		if (state == "normal") {
			playJumpOnce = false;
			thisBody.gravityScale = 1;
			movSpeedY = 0;
			movSpeedX = 0;
			ropeAnimator.gameObject.SetActive (false);
			ropeAnimator.enabled = true;
			characterAnimator.gameObject.SetActive (true);
		} else if (state == "rope") {
			ropeAnimator.gameObject.SetActive (true);
			characterAnimator.gameObject.SetActive (false);
			movSpeedY = 0;
			movSpeedX = 0;
			thisBody.gravityScale = 0;
			thisBody.velocity = new Vector2 (0, 0);
		}
	}

	void FixedUpdate () {
		if (state == "normal") {
			if (canJump > 0) {
				bool pressedJump = Input.GetButton ("Fire1");

				if (pressedJump) {
					canJump = 0;
					thisBody.velocity = new Vector2 (0, jumpStrength);
				}

				if (thisBody.velocity.y < -0.2f) {
					canJump = 0;
				}
			}
		}
	}

	void checkOverlapWithItem () {
		RaycastHit2D[] hit = Physics2D.RaycastAll (thisCollider.bounds.center, Vector2.zero, Mathf.Infinity,  1 << LayerMask.NameToLayer("Environment")).OrderBy (h => h.collider.GetComponent<Renderer>().sortingOrder).ToArray ();

		if (hit.Length > 0) {
			if (hit [0].transform.name == "ropeMiddle" || hit [0].transform.name == "ropeTop" || hit [0].transform.name == "ropeDown") {
				if (state == "normal") {
					Vector2 posNew = transform.parent.InverseTransformPoint (hit [0].transform.position);
					LeanTween.moveLocalX (gameObject, posNew.x, 0.125f);

					float movementY = Input.GetAxis ("Vertical");

					bool confirmRope = false;

					if (movementY > 0f && hit [0].transform.name == "ropeDown") {
						LeanTween.moveLocalY (gameObject, transform.localPosition.y + 0.2f, 0.05f);
						confirmRope = true;
					} else if (movementY < 0f && hit [0].transform.name == "ropeTop") {
						LeanTween.moveLocalY (gameObject, transform.localPosition.y - 0.2f, 0.05f);
						confirmRope = true;
					} else if (hit [0].transform.name == "ropeMiddle") {
						confirmRope = true;
					}

					if (confirmRope) {
						state = "rope";
						resetState ();
						if (lastPlatform != null) {
							Physics2D.IgnoreCollision (lastPlatform, thisCollider);
						}
					}
				}
			} else if (hit [0].transform.name == "door") {
				LeanTween.scaleX (hit [0].transform.gameObject, 0.2f, 0.5f);
				GetComponent<characterClass> ().enabled = false;
				LeanTween.value (0, 1, 0.2f).setOnComplete (() => {
					globalScript.changeScene (hit [0].transform.GetChild (0).name);
				});
			}
		} else if (hit.Length <= 0) {
			if (state == "rope") {
				state = "normal";
				resetState ();
				if (lastPlatform != null) {
					Physics2D.IgnoreCollision (lastPlatform, thisCollider, false);
				}
			}
		}
	}

	void keepCameraOnCharacter (bool skipAnim) {
		Vector2 currPos = thisParent.transform.position;

		Vector2 localPos = thisParent.transform.InverseTransformPoint (transform.position);
		currPos.x = -localPos.x;
		thisParent.transform.position = currPos;

		if (canJump > 0 || (thisBody.velocity.y < 0 && transform.position.y < -0.5f) || state == "rope" || skipAnim) {
			if (cameraLT != null) {
				LeanTween.cancel (cameraLT.id);
			}
			cameraLT = LeanTween.value (currPos.y, -localPos.y-0.5f, 0.25f).setOnUpdate ((value) => {
				if (thisParent == null) {
					LeanTween.cancel(cameraLT.id);
					return;
				}
				currPos = thisParent.transform.position;
				currPos.y = value;
				thisParent.transform.position = currPos;
				keepCameraInBounds();
			});

			if (skipAnim) {
				cameraLT.time = 0;
			}
		}

		keepCameraInBounds ();
	}


	void keepCameraInBounds () {
		Vector2 currPos = thisParent.transform.position;
		if (keepRendInBounds.transform.position.x - keepRendInBounds.bounds.extents.x > leftBottomScreen.x) {
			currPos.x = leftBottomScreen.x + keepRendInBounds.bounds.extents.x;
			thisParent.transform.position = currPos;
		} else if (keepRendInBounds.transform.position.x + keepRendInBounds.bounds.extents.x < rightTopScreen.x) {
			currPos.x = rightTopScreen.x - keepRendInBounds.bounds.extents.x;
			thisParent.transform.position = currPos;
		}

		if (keepRendInBounds.transform.position.y - keepRendInBounds.bounds.extents.y > leftBottomScreen.y) {
			currPos.y = leftBottomScreen.y + keepRendInBounds.bounds.extents.y;
			thisParent.transform.position = currPos;
		} else if (keepRendInBounds.transform.position.y + keepRendInBounds.bounds.extents.y < rightTopScreen.y) {
			currPos.y = rightTopScreen.y - keepRendInBounds.bounds.extents.y;
			thisParent.transform.position = currPos;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (triggerOccured != null) {
			if (collision.collider.name == "platform") {
				lastPlatform = collision.collider;
				canJump++;
			} else if (collision.collider.name == "movingPlatform") {
				lastPlatform = collision.collider;
				canJump++;
				transform.parent = collision.collider.transform;
			} else if (collision.collider.name == "death") {
				LeanTween.cancelAll ();
				SceneManager.LoadScene ("gameScene");
			} else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
				if (collision.contacts [0].point.x > transform.position.x) {
					characterAnimScript.flip = false;
				} else {
					characterAnimScript.flip = true;
				}
				if (state != "hit") {
					state = "normal";
					resetState ();
					state = "hit";
					hitTime = 0;
					Update ();
				}
			}
		} else {
			collisionOccured = collision;
		}
	}

	void OnCollisionExit2D (Collision2D collision) {
		if (collision.collider.name == "platform") {
			canJump--;

			if (canJump < 0) {
				canJump = 0;
			}

			Physics2D.IgnoreCollision (collision.collider, thisCollider);
		} else if (collision.collider.name == "movingPlatform") {
			canJump--;

			if (canJump < 0) {
				canJump = 0;
			}

			transform.parent = baseParent;

			Physics2D.IgnoreCollision (collision.collider, thisCollider);
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		triggerOccured = collider;
		if (collider.name == "platform" || collider.name == "movingPlatform") {
			bool willMiss = false;

			if (thisBody.velocity.y > 0.1f || canJump > 0) {
				if (collider.bounds.center.y + collider.bounds.extents.y > thisCollider.bounds.center.y - thisCollider.bounds.extents.y) {
					willMiss = true;
				}
			} else if (collider.bounds.center.x + collider.bounds.extents.x < thisCollider.bounds.center.x - thisCollider.bounds.extents.x) {
				willMiss = true;
			} else if (collider.bounds.center.x - collider.bounds.extents.x > thisCollider.bounds.center.x + thisCollider.bounds.extents.x) {
				willMiss = true;
			} else if (state == "rope") {
				if (collider.bounds.center.y - collider.bounds.extents.y > thisCollider.bounds.center.y + thisCollider.bounds.extents.y) {
					willMiss = true;
					lastPlatform = collider;
				}
			}

			if (willMiss) {
				Physics2D.IgnoreCollision (collider, thisCollider);
			} else {
				Physics2D.IgnoreCollision (collider, thisCollider, false);
			}
		} else if (collider.name == "path") {
			GetComponent<characterClass> ().enabled = false;
			globalScript.changeScene (collider.transform.GetChild (0).name);
		}

		if (collisionOccured != null) {
			OnCollisionEnter2D (collisionOccured);
			collisionOccured = null;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		triggerOccured = null;
	}
}
