using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
	// As the player hits the coin remove it, and score a point
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.tag == "Player")
		{
			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Coin);
			GameObject.Destroy(this.gameObject);
		}
	}
}
