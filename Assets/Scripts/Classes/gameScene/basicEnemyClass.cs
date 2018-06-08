using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anim_Sys;

public class basicEnemyClass : MonoBehaviour, IEnemy {
	public Collider2D assignedPlatform;

	public float movLimitLeft;
	public float movLimitRight;
	public float movSpeedX;

	public float attackRadius;

	private Animator thisAnimator;
	private string state;
	private float hitTime;
	private float attackTime;
	private Anim_GlobalControl thisPuppetControl;
	private Rigidbody2D thisBody;
	private ContactFilter2D thisContact;

	private GameObject character;

	private float life = 4;
	private int attackPoints;

	private Vector2 basePosition;
	private float attackDuration;

	private float multiTaskTimer;
	private float attackDelayTime;

	private float platformWaitTime;

	private int awardedCoins;

	private float frozenTime;
	[HideInInspector]
	public GameObject frozenObj;
	[HideInInspector]
	public GameObject lightObj;

	private int highestSortingOrder;

	public int AttackPoints {
		get {
			return attackPoints;
		}
		set {
		}
	}

	void findSpritesRecursive (Transform obj) {
		Renderer objRend = obj.GetComponent<Renderer> ();
		if (objRend != null) {
			if (objRend.sortingLayerName == "Enemies") {
				objRend.sortingOrder += globalScript.startingOrderEnemies;

				if (objRend.sortingOrder > highestSortingOrder) {
					highestSortingOrder = objRend.sortingOrder;
				}
			}
		}

		if (obj.childCount > 0) {
			for (int i = 0; i < obj.childCount; i++) {
				findSpritesRecursive (obj.GetChild (i));
			}
		}
	}

	// Use this for initialization
	void Start () {
		platformWaitTime = 0;
		highestSortingOrder = -9999;
		findSpritesRecursive (transform);
		highestSortingOrder++;
		globalScript.startingOrderEnemies = highestSortingOrder;

		basePosition = transform.localPosition;
		thisAnimator = transform.GetChild(0).GetComponent<Animator> ();
		thisBody = GetComponent<Rigidbody2D> ();
		thisPuppetControl = transform.GetChild(0).GetComponent<Anim_GlobalControl> ();

		state = "normal";

		if (transform.name == "snail") {
			attackDuration = 1.6f;
			life = 1;
			attackDelayTime = 1;
			attackPoints = 10;
			awardedCoins = 5;
		} else if (transform.name == "slime") {
			attackDuration = 1;
			life = 2;
			attackDelayTime = 0;
			attackPoints = 12;
			awardedCoins = 5;
		} else if (transform.name == "leaf") {
			attackDuration = 1;
			life = 1;
			attackDelayTime = 0;
			attackPoints = 18;
			awardedCoins = 5;
		} else if (transform.name == "spike") {
			attackDuration = 2.55f;
			life = 1;
			attackDelayTime = 0;
			attackPoints = 15;
			awardedCoins = 5;
		} else if (transform.name == "armadillo") {
			attackDuration = 1.5f;
			life = 2;
			attackDelayTime = 1;
			attackPoints = 20;
			awardedCoins = 5;
		} else if (transform.name == "caterpillar") {
			attackDuration = 1.5f;
			life = 2;
			attackDelayTime = 0.2f;
			attackPoints = 20;
			awardedCoins = 5;
		} else if (transform.name == "cactus") {
			attackDuration = 1.5f;
			life = 2;
			attackDelayTime = 0.2f;
			attackPoints = 20;
			awardedCoins = 5;
		} else if (transform.name == "spider") {
			attackDuration = 1.5f;
			life = 2;
			attackDelayTime = 0.2f;
			attackPoints = 20;
			awardedCoins = 5;
		} else if (transform.name == "fluff") {
			attackDuration = 1.3f;
			life = 3;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 5;
		} else if (transform.name == "octopus") {
			attackDuration = 1.3f;
			life = 3;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 5;
		} else if (transform.name == "bee") {
			attackDuration = 1.3f;
			life = 3;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 5;
		} else if (transform.name == "spikey") {
			attackDuration = 1.3f;
			life = 3;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 5;
		} else if (transform.name == "darkOctopus") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkSpikey") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkBee") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkFluff") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkArmadillo") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkCaterpillar") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkCactus") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkSpider") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkSpike") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkSlime") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkLeaf") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		} else if (transform.name == "darkSnail") {
			attackDuration = 1.3f;
			life = 5;
			attackDelayTime = 0.2f;
			attackPoints = 30;
			awardedCoins = 10;
		}
			
		if (movSpeedX > 0) {
			changeFaceDirection (true);
		}

		multiTaskTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == "normal") {
			if (platformWaitTime > 0) {
				if (!thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
					thisAnimator.Play ("Idle", -1, 0f);
				}
			} else {
				if (!thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Running")) {
					thisAnimator.Play ("Running", -1, 0f);
				}
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
				currPos.x -= movSpeedX;
				platformWaitTime += Time.deltaTime;
				if (platformWaitTime > 2.5f) {
					platformWaitTime = 0;
					movSpeedX = Mathf.Abs (movSpeedX);
				}
			} else if (currPos.x > basePosition.x + movLimitRight) {
				currPos.x -= movSpeedX;
				platformWaitTime += Time.deltaTime;
				if (platformWaitTime > 2.5f) {
					platformWaitTime = 0;
					movSpeedX = -Mathf.Abs (movSpeedX);
				}
			}

			transform.localPosition = currPos;

			if (transform.name == "leaf" && platformWaitTime <= 0) {
				multiTaskTimer += Time.deltaTime;
				if (multiTaskTimer > 1f) {
					multiTaskTimer = 0;
					thisBody.velocity = new Vector2 (0, 6);
				} 
			}

			if (character != null) {
				if (character.transform.localPosition.x + 0.3f < transform.localPosition.x) {
					movSpeedX = -Mathf.Abs (movSpeedX);
					platformWaitTime = 0;
				} else if (character.transform.localPosition.x - 0.3f > transform.localPosition.x) {
					movSpeedX = Mathf.Abs (movSpeedX);
					platformWaitTime = 0;
				}

				if ((getFaceDirection () && character.transform.position.x > transform.position.x) || (!getFaceDirection () && character.transform.position.x < transform.position.x)) {
					if ((Mathf.Abs (Mathf.Abs (transform.position.x) - Mathf.Abs (character.transform.position.x))) < attackRadius) {
						platformWaitTime = 0;
						state = "attack";
						thisAnimator.Play ("Attack", -1, 0f);
						attackTime = 0;
					}
				}
			}

			if (movSpeedX > 0) {
				changeFaceDirection (true);
			} else if (movSpeedX < 0) {
				changeFaceDirection (false);
			}

		} else if (state == "hit") {
			platformWaitTime = 0;
			if (!thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Hit")) {
				thisAnimator.Play ("Hit", -1, 0f);

				if (getFaceDirection ()) {
					thisBody.velocity = new Vector2 (0.5f, 0);
				} else {
					thisBody.velocity = new Vector2 (-0.5f, 0);
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
				} else if (transform.name == "cactus") {
					thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
				}
			} else if (attackTime > 0.5f) {
				if (transform.name == "leaf") {
					if (getFaceDirection ()) {
						thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
					}
				}
			} else if (attackTime > 0.3f) {
				if (transform.name == "snail") {
					if (getFaceDirection ()) {
						thisBody.velocity = new Vector2 (6, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (-6, thisBody.velocity.y);
					}
				} else if (transform.name == "leaf") {
					if (getFaceDirection ()) {
						thisBody.velocity = new Vector2 (2, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (-2, thisBody.velocity.y);
					}
				} else if (transform.name == "cactus") {
					if (getFaceDirection ()) {
						thisBody.velocity = new Vector2 (3, thisBody.velocity.y);
					} else {
						thisBody.velocity = new Vector2 (-3, thisBody.velocity.y);
					}
				}
			}
		} else if (state == "attackDelay") {
			platformWaitTime = 0;
			attackTime += Time.deltaTime;
			if (attackTime > attackDelayTime) {
				state = "normal";
				attackTime = 0;
				if (transform.name == "leaf") {
					multiTaskTimer = 0;
				}
			}
		} else if (state == "frozen") {
			platformWaitTime = 0;
			frozenTime += Time.deltaTime;
			if (frozenTime > 5) {
				thisAnimator.enabled = true;
				state = "normal";
				DestroyImmediate (frozenObj);
				frozenObj = null;
			}
		} else if (state == "light") {
			platformWaitTime = 0;
			frozenTime += Time.deltaTime;
			if (frozenTime > 3) {
				thisAnimator.enabled = true;
				life = 0;
				whenLifeZero ();
				DestroyImmediate (lightObj);
				lightObj = null;
			}
		}
	}

	bool getFaceDirection () {
		if (thisPuppetControl != null) {
			return thisPuppetControl.flip;
		} else {
			if (transform.GetChild (0).localScale.x > 0) {
				return false;
			} else {
				return true;
			}
		}
	}

	void changeFaceDirection (bool direction) {
		if (thisPuppetControl != null) {
			thisPuppetControl.flip = direction;
		} else {
			Vector2 scale = transform.GetChild (0).localScale;
			if (direction == true) {
				scale.x = -Mathf.Abs (scale.x);
			} else {
				scale.x = Mathf.Abs (scale.x);
			}
			transform.GetChild (0).localScale = scale;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.collider.name == "sword1" || collision.collider.name == "characterArrow" || collision.collider.name == "fire_explosion_1" || collision.collider.name == "characterStone") {
			if (state != "hit") {
				if (state != "frozen" && state != "light") {
					state = "hit";
					hitTime = 0;
				}

				if (collision.contacts != null && collision.contacts.Length > 0) {
					if (collision.contacts [0].point.x > transform.position.x) {
						changeFaceDirection (true);
					} else {
						changeFaceDirection (false);
					}
				}

				if (collision.collider.name == "characterArrow") {
					life -= 0.5f*1.3f;

					if (globalScript.equippedSword == "bow2") {
						life -= 0.5f*1.3f;
					} else if (globalScript.equippedSword == "bow3") {
						life -= 1*1.3f;
					} else if (globalScript.equippedSword == "bow4") {
						life -= 1.5f*1.3f;
					} else if (globalScript.equippedSword == "bow5") {
						life -= 2*1.3f;
					}
				} else if (collision.collider.name == "fire_explosion_1") {
					life = 0;
				} else if (collision.collider.name == "characterStone") {
					life = 0;
				} else {
					life -= 1*1.3f;

					if (globalScript.equippedSword == "sword2") {
						life -= 1*1.3f;
					} else if (globalScript.equippedSword == "sword3") {
						life -= 2*1.3f;
					} else if (globalScript.equippedSword == "sword4") {
						life -= 3*1.3f;
					} else if (globalScript.equippedSword == "sword5") {
						life -= 4*1.3f;
					}
				}

				if (life <= 0) {
					Physics2D.IgnoreCollision (collision.collider, collision.otherCollider);
					whenLifeZero ();
				}
			}
			if (collision.collider.name == "characterArrow") {
				Destroy (collision.collider.gameObject);
			}
		}
	}

	void whenLifeZero () {
		if (state == "death") {
			return;
		}
		if (frozenObj) {
			DestroyImmediate (frozenObj);
			frozenObj = null;
		}
		if (state == "frozen") {
			thisAnimator.enabled = true;
		}
		state = "death";
		thisAnimator.Play ("Death", -1, 0f);
		gameObject.layer = LayerMask.NameToLayer ("Default");

		GameObject coin = Resources.Load<GameObject> ("coin");

		for (int i = 0; i < awardedCoins; i++) {
			GameObject c = Instantiate (coin);
			c.transform.parent = transform.parent.parent;
			c.transform.position = transform.position;
		}

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
			if (getFaceDirection () == true) {
				LeanTween.rotateZ (gameObject, 90, 0.25f);
			} else if (getFaceDirection () == false) {
				LeanTween.rotateZ (gameObject, -90, 0.25f);
			}
		}

		thisBody.velocity = new Vector2 (0, thisBody.velocity.y);
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.name == "etherBoy") {
			character = collider.gameObject;
			/*if ((thisPuppetControl.flip && character.transform.position.x > transform.position.x) || (!thisPuppetControl.flip && character.transform.position.x < transform.position.x)) {
				character = collider.gameObject;
			} else {
				character = null;
			}*/
		} else if (collider.gameObject.name == "fire_explosion_1") {
			life = 0;
			whenLifeZero ();
		} else if (collider.gameObject.name == "beam3" && globalScript.equippedMagic == "ice" && state != "frozen" && state != "death") {
			state = "frozen";
			frozenTime = 0;

			thisAnimator.enabled = false;

			GameObject frozen = new GameObject ();
			frozen.transform.parent = transform;

			SpriteRenderer frozenRend = frozen.AddComponent<SpriteRenderer> ();
			frozenRend.sprite = Resources.Load<Sprite> ("ice_freeze_block");
			frozen.layer = LayerMask.NameToLayer ("Character");
			frozenRend.sortingLayerName = "Character";
			frozen.transform.localPosition = new Vector2 (0, 0);

			frozenObj = frozen;
		} else if (collider.gameObject.name == "beam3" && globalScript.equippedMagic == "air" && state != "light" && state != "death") {
			state = "light";
			frozenTime = 0;

			thisAnimator.enabled = false;

			lightObj = Instantiate (Resources.Load<GameObject> ("lightAttack"));
			lightObj.layer = LayerMask.NameToLayer ("Character");
			lightObj.transform.localPosition = new Vector2 (0, 0.5f);
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.name == "etherBoy") {
			character = null;
		}
	}
}
