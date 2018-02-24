using UnityEngine;using System.Collections;
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
	
	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	private int _score = 0;

	//
	private bool gameOver = false;
	private bool endGame = false;
	//
	private List<MemoryCard> cards;

	private Vector3 startPos = new Vector3(-3f, 1.45f, 0f);

	void OnGUI() {

		if(gameOver){
			int width = camera.pixelWidth;

			int height = camera.pixelHeight;


			int centerX = width /2;

			int centerY = height/2;
			//GUIStyle style = new GUIStyle();
			//style.fontSize = 70;
			//style.font.material.color = Color.red;
			//GUI.Label (new Rect (275, 100, 150, 100), "GAME OVER ", style);
			GUI.Label (new Rect (centerX, centerY - 50, 150, 100), "GAME OVER ");
			

			if (GUI.Button (new Rect (centerX - 200, centerY, 150, 100), "Start Over")) {
				gameOver = false;
				changeSizeRestart ();
				startGame ();
			}

			if (GUI.Button (new Rect (centerX + 50, centerY, 150, 100), "End Game")) {
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
		startGame ();
	}

	private void startGame(){
		//create cards array 
		cards = new List<MemoryCard>();

		Vector3 startPos = originalCard.transform.position;

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
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);

				cards.Add(card);
			}
		}
	}


	private int[] createShuffledArray(){
		int[] ans = new int[gridCols * gridRows];

		int count = 0;
		bool countAdded = false;


		for (int i = 0; i < (gridCols * gridRows); i++) {

			if (!countAdded) {
				ans [i] = count;
				countAdded = true;
			} else {
				ans [i] = count;

				count++;

				countAdded = false;
			}
				
		}

		//shuffle array
		ans = ShuffleArray(ans);

		return ans;
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


	private Vector3 getCardTrans(int size){

		Vector3 trans;

		switch (size){

		case 8:
			trans = startPos;
			break;

		case 10:
			trans = new Vector3(startPos.x - 5, startPos.y, startPos.z);
			break;

		case 12:
			trans = new Vector3(startPos.x - 5f, startPos.y + 5f, startPos.z);
			break;

		case 16:
			trans = new Vector3(startPos.x - 5f, startPos.y + 5f, startPos.z);
			break;

		case 20:
			trans = new Vector3(startPos.x - 5f, startPos.y+ 5f, startPos.z);
			break;

		default:
			trans = new Vector3(startPos.x - 5f, startPos.y+ 5f, startPos.z);
			break;

		}
		return trans;
	}


	public void LoadNewGame(int size){
		gameOver = false;

		changeSizeRestart();

		setNewDimensions (size);

		//create cards array 
		cards = new List<MemoryCard>();

		Vector3 startPos = originalCard.transform.position;

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
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);

				cards.Add(card);
			}
		}


		/*
		gameOver = false;

		_firstRevealed = null;
		_secondRevealed = null;

		changeSizeRestart();

		setNewDimensions (size);

		setNewOffsetValues (size);

		//create cards array 
		this.cards = new List<MemoryCard>();

		float cardScale = getCardScale (size);


		//Vector3 startPos = originalCard.transform.position;

		//translate the card if needed
		//originalCard.gameObject.transform.position = getCardTrans(size);

		//scale the card 
		originalCard.gameObject.transform.localScale = new Vector2(cardScale, cardScale);


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
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;

				card.transform.position = new Vector3(posX, posY, startPos.z);

				card.transform.localScale = new Vector2(cardScale, cardScale);

				if(id = 1 || id == 2){
					card.Unreveal ();
				}


				cards.Add(card);

			}
		}
		*/
	}

	private void changeSizeRestart(){
		
		for (int i = 1 ; i < (gridCols * gridRows); i++){

			Destroy (cards [i].gameObject);

		}

		//now make state changes to card 0
		originalCard.Unreveal();

		_score = 0;

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
		Application.LoadLevel("2X4 game");
	}
}
