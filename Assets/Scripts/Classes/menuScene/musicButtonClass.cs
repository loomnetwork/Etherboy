using UnityEngine;
using System.Collections;

public class musicButtonClass : MonoBehaviour, ITouchable {
	public Sprite offSprite;

	private Sprite onSprite;
	private SpriteRenderer thisRend;

	private bool mustFocus = true;
	public bool MustFocus {
		get {
			return mustFocus;
		}
		set {
		}
	}
	private Vector3 baseScale;
	private float scale = 0.8f;

	public bool TouchBegan (Vector2 touchPosition) {
		GetComponent<AudioSource> ().Play ();
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		transform.localScale = new Vector3 (baseScale.x * scale, baseScale.y * scale, baseScale.z);
		return false;
	}
		
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		if (!isInBounds) {
			GetComponent<Renderer>().material.color = new Color(1, 1, 1);
			transform.localScale = baseScale;
			touchController.FocusObject = null;
		}
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		transform.localScale = baseScale;
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;
		if (PlayerPrefs.GetInt("volumeMusic") == 0) {
			thisRend.sprite = offSprite;
			PlayerPrefs.SetInt ("volumeMusic", 1);
		} else {
			thisRend.sprite = onSprite;
			PlayerPrefs.SetInt ("volumeMusic", 0);
		}
		return false;
	}

	void Start () {
		thisRend = GetComponent<SpriteRenderer> ();
		onSprite = thisRend.sprite;
		baseScale = transform.localScale;

		if (PlayerPrefs.GetInt("volumeMusic") == 0) {
			thisRend.sprite = onSprite;
		} else {
			thisRend.sprite = offSprite;
		}
	}
}
