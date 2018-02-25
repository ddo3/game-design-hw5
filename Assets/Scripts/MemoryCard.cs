using UnityEngine;
using System.Collections;

public class MemoryCard : MonoBehaviour {
	[SerializeField] private GameObject cardBack;
	[SerializeField] private SceneController controller;

	private float speed = 20.0f; //how fast it shakes

	private float timer = 0.0f;

	private bool cardDies = false;

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

	void Update(){
		
		if(timer <= 2.5 && cardDies){

			float x = Mathf.Sin(Time.time * speed)*.005f;

			transform.Translate(new Vector3(x,0,0));//(new Vector3(x, y,z));

			timer += Time.deltaTime;


		}


		if (timer >= 2.5) {
			//Destroy(this.gameObject);
			//GetComponent<SpriteRenderer>().sprite = null;

			GetComponent<ParticleSystem>().Stop ();
			cardDies = false;
			timer = 0.0f;
		}
	}


	public void shakeAndPoof(){
		
		cardDies = true;

		ParticleSystem p = GetComponent<ParticleSystem> ();

		//p.startColor = new Color(0.5f, 0.5f, 0.5f, 1);

		p.Play ();

	}
}
