using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class multiTouchController : MonoBehaviour {

	//Ragdog Studios SRL SEMPLIFICATA 2016
	//Touch Controller for Unity
	//This controller allows an easy implementation of buttons and touchable objects
	//The ITouchable interface is included, and required for each class wishing to leverage this class capabilities.
	//Currently it supports only single touches, multi-touch support coming soon.
	//It also works on both mobile and desktop platform, as well as editor.

	private Vector2 touchPosition = new Vector2(0, 0);
	private int touchCount;
	private TouchPhase touchPhase;
	public static Collider2D[] focusObject; 

	void OnMouseDown (){
		Update ();
	}

	void OnMouseDrag (){
		Update ();
	}

	void OnMouseUp (){
		Update ();
	}

	void Start () {
		focusObject = new Collider2D[10];
	}

	// Update is called once per frame
	void Update () {

		for (int z = 0; z < 10; z++) {
			if (Application.isMobilePlatform) {
				touchCount = Input.touchCount;

				if (touchCount == 0) {
					for (int x = 0; x < focusObject.Length; x++) {
						focusObject [x] = null;
					}
				}

				if (touchCount > 0) {
					touchPhase = Input.GetTouch (z).phase;
				} else {
					touchPhase = TouchPhase.Stationary;
				}
				if (touchCount > 0) {
					touchCount = 1;
					touchPosition =  Input.GetTouch(z).position;
				}
				if (touchPhase == TouchPhase.Ended) {
					touchCount = 0;
					if (touchCount > 0) {
						touchPosition = Input.GetTouch (z).position;
					}
				}
			} else {
				if (Input.GetMouseButton (0)) {
					touchCount = 1;
				} else {
					touchCount = 0;
				}
				if (Input.GetMouseButtonDown (0)) {
					touchPhase = TouchPhase.Began;
				} else if (Input.GetMouseButtonUp (0)) {
					touchPhase = TouchPhase.Ended;
				} else {
					touchPhase = TouchPhase.Moved;
				}

				if (touchCount >= 0) {
					touchPosition = Input.mousePosition;
				}
			}

			if (touchCount > 0 && touchPhase == TouchPhase.Began) {
				Vector2 touchRay = Camera.main.ScreenToWorldPoint (touchPosition);
				RaycastHit2D[] hit = Physics2D.RaycastAll (touchRay, Vector2.zero).OrderBy (h => h.collider.GetComponent<Renderer>().sortingOrder).ToArray ();
				if (hit != null) {
					for (int i = hit.Length-1; i >= 0; i--) {
						Collider2D obj = hit [i].collider;
						if (obj != null) {
							ITouchable touchable = (ITouchable)obj.GetComponent (typeof(ITouchable));
							if (touchable != null) {
								bool found = false;
								for (int y = 0; y < focusObject.Length; y++) {
									if (focusObject [y] == obj) {
										found = true;
										break;
									}
								}
								if (found == false) {
									if (touchable.MustFocus) {
										focusObject[z] = obj;
									}
									touchable.TouchBegan (touchPosition);
								}
								break;
							}
						}
					}
				}
			} else if (touchCount >= 0 && touchPhase == TouchPhase.Moved) {
				Vector2 touchRay = Camera.main.ScreenToWorldPoint (touchPosition);
				RaycastHit2D[] hit = Physics2D.RaycastAll (touchRay, Vector2.zero).OrderBy (h => h.collider.GetComponent<Renderer>().sortingOrder).ToArray ();
				if (focusObject != null) {
					bool found = false;
					for (int i = hit.Length-1; i >= 0; i--) {
						if (hit [i].collider == focusObject[z]) {
							found = true;
							break;
						}
					}
					if (focusObject [z] != null) {
						ITouchable touchable = (ITouchable)focusObject [z].GetComponent (typeof(ITouchable));
						if (touchable != null) {
							touchable.TouchMoved (touchPosition, found);
						}
					}
				}
			} else if (touchCount == 0 && touchPhase == TouchPhase.Ended) {
				if (focusObject != null) {
					Collider2D obj = focusObject[z];
					if (obj != null) {
						ITouchable touchable = (ITouchable)obj.GetComponent (typeof(ITouchable));
						if (touchable != null) {
							touchable.TouchEnded (touchPosition);
						}
					}
				}
				for (int x = z+1; x < focusObject.Length; x++) {
					if (x > 0) {
						focusObject [x - 1] = focusObject [x];
					}
				}
			}

			if (!Application.isMobilePlatform) {
				break;
			}
		}
	}
}
