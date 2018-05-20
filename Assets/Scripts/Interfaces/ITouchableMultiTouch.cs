using UnityEngine;
using System.Collections;

public interface ITouchableMultiTouch {

	bool MustFocus {
		get;
		set;
	}

	bool TouchBegan (Vector2 touchPosition);

	bool TouchMoved (Vector2 touchPosition, bool isInBounds);
		
	bool TouchEnded (Vector2 touchPosition);
}
