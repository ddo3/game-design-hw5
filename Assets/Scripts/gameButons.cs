using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameButons : MonoBehaviour {

	public Color highlightColor = Color.cyan;

	[SerializeField] private GameObject targetObject;
	[SerializeField] private string targetMessage;

	[SerializeField] private int size;

	public void OnMouseEnter() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite != null) {
			sprite.color = highlightColor;
		}
	}
	public void OnMouseExit() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite != null) {
			sprite.color = Color.white;
		}
	}

	public void OnMouseDown() {
		transform.localScale = new Vector3(0.4f, 0.3f, 0.95f);
	}

	public void OnMouseUp() {
		transform.localScale = new Vector3(0.4f, 0.3f, 0.95f);
		if (targetObject != null) {
			targetObject.SendMessage(targetMessage, size);
		}
	}



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
