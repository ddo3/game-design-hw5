﻿using UnityEngine;
using System.Collections;

public class MemoryCard : MonoBehaviour {
	[SerializeField] private GameObject cardBack;
	[SerializeField] private SceneController controller;

	private int _id;
	public int id {
		get {return _id;}
	}

	public void SetCard(int id, Sprite image) {
		_id = id;
		GetComponent<SpriteRenderer>().sprite = image;
	}

	public void OnMouseDown() {
		if (cardBack.activeSelf && controller.canReveal) {
			cardBack.SetActive(false);
			controller.CardRevealed(this);
		}
	}

	public void Unreveal() {
		cardBack.SetActive(true);
	}


	public void MakeEndGameCard(){

		GetComponent<SpriteRenderer>().sprite = null;
		Destroy (this.gameObject);
		//destroy this game object
	}


	public void Scale(){

	}

	public void Translate(){

	}

}
