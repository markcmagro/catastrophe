using UnityEngine;
using System.Collections;

public class TurnOnOff : MonoBehaviour {
	public Sprite spriteOn;
	public Sprite spriteOff;
	public bool state;
	private BoxCollider2D boxCollider = null;
	private SpriteRenderer myRender;

	void Start () {
		myRender = GetComponent<SpriteRenderer> ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}
	void Update () {
		if(boxCollider == null) return;
		if (Input.GetMouseButtonDown (0)) {
			if (boxCollider.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
				state = !state;
			}
		} else if (Input.touchCount > 0) {
			if (boxCollider.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position))) {
				state = !state;
			}
		}
		if (myRender != null) {
			if (state == true) {
				myRender.sprite = spriteOn;
			} else {
				myRender.sprite = spriteOff;
			}
		}
	}
}
