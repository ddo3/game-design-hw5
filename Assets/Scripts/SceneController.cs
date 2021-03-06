﻿using UnityEngine;using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
	public int gridRows = 2;
	public int gridCols = 4;
	public float offsetX = 2f;
	public float offsetY = 2.5f;

	[SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
	[SerializeField] private Camera camera;
	[SerializeField] private GameObject gameOverObject;

	private Sprite[] actualSprites;

	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	private int _score = 0;

	//
	private bool gameOver = false;
	private bool endGame = false;
	private bool displayGameOverObject = false;
	private float speed = 15.0f; //how fast it shakes

	//
	private List<MemoryCard> cards;

	private Vector3 startPos;//= new Vector3(-3f, 1.45f, 0f);
	private Vector3 originalStartPos = new Vector3(-3f, 1.45f, 0f);

	private float timer = 0.0f;

	void OnGUI() {

		if(gameOver && !displayGameOverObject){
			int width = camera.pixelWidth;

			int height = camera.pixelHeight;


			int centerX = width /2;

			int centerY = height/2;
			//GUIStyle style = new GUIStyle();
			//style.fontSize = 70;
			//style.font.material.color = Color.red;
			//GUI.Label (new Rect (275, 100, 150, 100), "GAME OVER ", style);
			//GUI.Label (new Rect (centerX, centerY - 50, 150, 100), "GAME OVER ");
			

			if (GUI.Button (new Rect (centerX - 200, centerY, 150, 100), "Start Over")) {
				
				//gameOverObject.SetActive (false);
				changeSizeRestart ();
				gameOver = false;
				startGame ();
			}

			if (GUI.Button (new Rect (centerX + 50, centerY, 150, 100), "End Game")) {
				endGame = true;
				EndGame ();

				//gameOverObject.SetActive (false);
				gameOver = false;
			}
		}
	}


	public bool canReveal {
		get {return _secondRevealed == null;}
	}
		
	// Use this for initialization
	void Start() {
		
		startPos = new Vector3(-3f, 1.45f, 0f);

		Object[] spriteObjects = Resources.LoadAll( "", typeof(Sprite));

		actualSprites = new Sprite[spriteObjects.Length];

		//populate the actaul sprites array

		for(int i = 0; i < spriteObjects.Length; i++){
			actualSprites [i] = spriteObjects [i] as Sprite;
		}


		startGame ();
	}


	void Update(){
		
		if ( displayGameOverObject && timer <= 2.5f){
			
			float x = Mathf.Sin(Time.time * speed)*.005f;

			gameOverObject.transform.Translate(new Vector3(x,0,0));

			timer += Time.deltaTime;
		}else{
			
			displayGameOverObject = false;
			timer = 0.0f;
			gameOverObject.SetActive (false);
		}
	}


	private void startGame(){
		//create cards array 

		gameOverObject.SetActive(false);

		cards = new List<MemoryCard>();

		//Vector3 startPos = originalCard.transform.position;

		// create shuffled list of cards
		int[] numbers = createShuffledArray();

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
				card.SetCard(id, actualSprites[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);

				cards.Add(card);
			}
		}
	}


	private int[] createShuffledArray(){
		List<int> list = new List<int> ();

		for(int i = 0; i < (gridCols * gridRows)/2 ; i ++){

			//get a random number between 0 and 51
			int num = Random.Range(0,51);

			//Add that number to list twice
			list.Add(num);
			list.Add (num);

		}

		int[] arrayOfNums = list.ToArray();

		//shuffle array
		arrayOfNums = ShuffleArray(arrayOfNums);

		return arrayOfNums;
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

			_firstRevealed.shakeAndPoof ();
			_secondRevealed.shakeAndPoof ();

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


				gameOverObject.SetActive (true);
				displayGameOverObject = true;
				gameOver = true;
			}
		}


	}


	private void changeSizeRestart(){

		for (int i = 1 ; i < (gridCols * gridRows); i++){

			Destroy (cards [i].gameObject);

		}

		//cards[0]..GetComponent<SpriteRenderer>().sprite = images[0];

		//now make state changes to card 0
		originalCard.Unreveal();

		//make sprite for original card not null
		originalCard.GetComponent<SpriteRenderer>().sprite = actualSprites[0];

		//reset score 
		_score = 0;

		//reset label
		scoreLabel.text = "Score: ";

	}

	public void EndGame(){
		//Application.LoadLevel("2X4 game");
		//clear the scene 

		for (int i = 0 ; i < (gridCols * gridRows); i++){
			//cards[i].MakeEndGameCard();
			Destroy(cards[i].gameObject);
		}


	}

	public void Restart() {
		//Application.LoadLevel("2X4 game");
		gameOverObject.SetActive (false);
		changeSizeRestart ();
		gameOver = false;
		startGame ();
	}

	private void setNewDimensions(int size){
		switch (size){

		case 8:
			this.gridRows = 2;
			this.gridCols = 4;
			break;

		case 10:
			this.gridRows = 2;
			this.gridCols = 5;
			break;

		case 12:
			this.gridRows = 3;
			this.gridCols = 4;
			break;

		case 16:
			this.gridRows = 4;
			this.gridCols = 4;
			break;

		case 20:
			this.gridRows = 4;
			this.gridCols = 5;
			break;

		default:
			break;
		}

	}

	private void setNewOffsetValues(int size){
		switch (size){

		case 8:
			this.offsetX = 2f;
			this.offsetY = 2.5f;
			break;

		case 10:
			this.offsetX = 1.8f;
			this.offsetY = 2.3f;
			break;

		case 12:
			this.offsetX = 1.8f;
			this.offsetY = 1.7f;
			break;

		case 16:
			this.offsetX = 1.7f;
			this.offsetY = 1.3f;
			break;


		case 20:
			this.offsetX = 1.5f;
			this.offsetY = 1.3f;
			break;

		default:
			break;
		}

	}


	private float getCardScale(int size){

		float cardScale = 0.0f;

		switch (size){

		case 8:
			cardScale = 1.0f;
			break;

		case 10:
			cardScale = .8f;
			break;

		case 12:
			cardScale = .7f;
			break;

		case 16:
			cardScale = .6f;
			break;

		case 20:
			cardScale = .6f;
			break;

		default:
			cardScale = 1.0f;
			break;

		}
		return cardScale;
	}

	private void setCardTrans(int size){
		switch (size){

		case 8:
			startPos = originalStartPos;
			break;

		case 10:
			startPos = new Vector3(originalStartPos.x - .8f, originalStartPos.y, 0f);
			break;

		case 12:
			startPos = new Vector3(originalStartPos.x + .1f, originalStartPos.y + .4f, 0f);
			break;

		case 16:
			startPos = new Vector3(originalStartPos.x + .2f, originalStartPos.y + .7f, 0f);
			break;

		case 20:
			startPos = new Vector3(originalStartPos.x + .3f, originalStartPos.y + .7f, 0f);
			break;

		default:
			startPos = originalStartPos;
			break;

		}
	}


	public void LoadNewGame(int size){
		//leave gameOver State
		gameOver = false;

		//remove game over stuff from board
		gameOverObject.SetActive (false);

		_firstRevealed = null;
		_secondRevealed = null;

		changeSizeRestart();

		setNewDimensions (size);

		setNewOffsetValues (size);

		//create cards array 
		cards = new List<MemoryCard>();

		float cardScale = getCardScale (size);

		//translate the card if needed
		setCardTrans (size);

		//scale the card 
		originalCard.gameObject.transform.localScale = new Vector3(cardScale, cardScale, 1);


		// create shuffled list of cards
		int[] numbers = createShuffledArray();

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
				card.SetCard(id, actualSprites[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, 0);

				cards.Add(card);
			}
		}
			
	}


}
