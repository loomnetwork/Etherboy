using UnityEngine;
using System.Collections;
using System.Linq;

public class touchController : MonoBehaviour {

	//Ragdog Studios SRL SEMPLIFICATA 2016
	//Touch Controller for Unity
	//This controller allows an easy implementation of buttons and touchable objects
	//The ITouchable interface is included, and required for each class wishing to leverage this class capabilities.
	//Currently it supports only single touches, multi-touch support coming soon.
	//It also works on both mobile and desktop platform, as well as editor.

	private Vector2 touchPosition = new Vector2(0, 0);
	private int touchCount;
	private TouchPhase touchPhase;
	private static Collider2D focusObject;
	public static Collider2D FocusObject {
		get {
			return focusObject;
		}
		set {
			focusObject = value;
		}
	} 

	void OnMouseDown (){
		Update ();
	}

	void OnMouseDrag (){
		Update ();
	}

	void OnMouseUp (){
		Update ();
	}

	// Update is called once per frame
	void Update () {
		if (Application.isMobilePlatform) {
			touchCount = Input.touchCount;
			if (touchCount > 0) {
				touchPhase = Input.GetTouch (0).phase;
			} else {
				touchPhase = TouchPhase.Stationary;
			}
			if (touchCount > 0) {
				touchCount = 1;
				touchPosition =  Input.GetTouch(0).position;
			}
			if (touchPhase == TouchPhase.Ended) {
				touchCount = 0;
				if (touchCount > 0) {
					touchPosition = Input.GetTouch (0).position;
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
			RaycastHit2D[] hit = Physics2D.RaycastAll (touchRay, Vector2.zero).OrderBy ((h) => { 
				Renderer colliderRend = h.collider.GetComponent<Renderer>();
				if (colliderRend != null) {
					if (colliderRend.sortingLayerName == "UI") {
						return colliderRend.sortingOrder;
					} else {
						return -1;
					}
				} else {
					return -1;
				}
			}).ToArray ();
	
			if (hit != null) {
				for (int i = hit.Length-1; i >= 0; i--) {
					Collider2D obj = hit [i].collider;
					if (obj != null) {
						ITouchable touchable = (ITouchable)obj.GetComponent (typeof(ITouchable));
						if (touchable != null) {
							if (touchable.MustFocus) {
								focusObject = obj;
							}
							touchable.TouchBegan (touchPosition);
							break;
						}
					}
				}
			}
		} else if (touchCount >= 0 && touchPhase == TouchPhase.Moved) {
			Vector2 touchRay = Camera.main.ScreenToWorldPoint (touchPosition);
			RaycastHit2D[] hit = Physics2D.RaycastAll (touchRay, Vector2.zero).OrderBy ((h) => { 
				Renderer colliderRend = h.collider.GetComponent<Renderer>();
				if (colliderRend != null) {
					if (colliderRend.sortingLayerName == "UI") {
						return colliderRend.sortingOrder;
					} else {
						return -1;
					}
				} else {
					return -1;
				}
			}).ToArray ();
			if (focusObject != null) {
				bool found = false;
				for (int i = hit.Length-1; i >= 0; i--) {
					if (hit [i].collider == focusObject) {
						found = true;
						break;
					}
				}
				ITouchable touchable = (ITouchable)focusObject.GetComponent (typeof(ITouchable));
				if (touchable != null) {
					touchable.TouchMoved (touchPosition, found);
				}
			}
		} else if (touchCount == 0 && touchPhase == TouchPhase.Ended) {
			if (focusObject != null) {
				Collider2D obj = focusObject;
				if (obj != null) {
					ITouchable touchable = (ITouchable)obj.GetComponent (typeof(ITouchable));
					if (touchable != null) {
						touchable.TouchEnded (touchPosition);
					}
				}
			} else {
				/*
				Vector2 touchRay = Camera.main.ScreenToWorldPoint (touchPosition);
				RaycastHit2D[] hit = Physics2D.RaycastAll (touchRay, Vector2.zero).OrderBy (h => h.collider.GetComponent<Renderer>().sortingOrder).ToArray ();
				if (hit != null) {
					for (int i = hit.Length-1; i >= 0; i--) {
						Collider2D obj = hit [i].collider;
						if (obj != null) {
							ITouchable touchable = (ITouchable)obj.GetComponent (typeof(ITouchable));
							if (touchable != null) {
								touchable.TouchEnded (touchPosition);
							}
						}
					}
				}
*/
			}
			focusObject = null;
		}
	}
}
