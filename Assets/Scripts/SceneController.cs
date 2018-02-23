﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
	public const int gridRows = 2;
	public const int gridCols = 4;
	public const float offsetX = 2f;
	public const float offsetY = 2.5f;

	[SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
	
	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	private int _score = 0;
	private bool gameOver = false;
	private bool endGame = false;

	private List<MemoryCard> cards;

	void OnGUI() {

		if(gameOver){

			//GUIStyle style = new GUIStyle();
			//style.fontSize = 70;
			//style.font.material.color = Color.red;
			//GUI.Label (new Rect (275, 100, 150, 100), "GAME OVER ", style);
			GUI.Label (new Rect (275, 100, 150, 100), "GAME OVER ");
			

			if (GUI.Button (new Rect (150, 150, 150, 100), "Start Over")) {
				
				Restart ();
			}

			if (GUI.Button (new Rect (400, 150, 150, 100), "End Game")) {
				endGame = true;
				EndGame ();
				gameOver = false;
			}
		}
	}


	public bool canReveal {
		get {return _secondRevealed == null;}
	}

	// Use this for initialization
	void Start() {
		//create cards array 
		cards = new List<MemoryCard>();

		Vector3 startPos = originalCard.transform.position;

		// create shuffled list of cards
		int[] numbers = {0, 0, 1, 1, 2, 2, 3, 3};
		numbers = ShuffleArray(numbers);

		// place cards in a grid
		for (int i = 0; i < gridCols; i++) {
			for (int j = 0; j < gridRows; j++) {
				MemoryCard card;

				// use the original for the first grid space
				if (i == 0 && j == 0) {
					card = originalCard;
				} else {
					card = Instantiate(originalCard) as MemoryCard;
				}

				// next card in the list for each grid space
				int index = j * gridCols + i;
				int id = numbers[index];
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);

				cards.Add(card);
			}
		}
	}


	// Knuth shuffle algorithm
	private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard card) {
		if (_firstRevealed == null) {
			_firstRevealed = card;
		} else {
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}
	
	private IEnumerator CheckMatch() {
		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id) {
			_score++;
			scoreLabel.text = "Score: " + _score;
		}

		// otherwise turn them back over after .5s pause
		else {
			yield return new WaitForSeconds(.5f);

			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();
		}
		
		_firstRevealed = null;
		_secondRevealed = null;


		checkIfGameIsOver ();
	}


	private void checkIfGameIsOver(){

		if(!endGame){
			int numberOfMatchedCards = (gridRows * gridCols)/2 ;

			if (_score == numberOfMatchedCards){

				gameOver = true;
			}
		}


	}

	public void EndGame(){
		//Application.LoadLevel("2X4 game");
		//clear the scene 

		for (int i = 0 ; i < (gridCols * gridRows); i++){
			cards[i].MakeEndGameCard();
		}

	}

	public void Restart() {
		Application.LoadLevel("2X4 game");
	}
}
