using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

	public bool taken = false;

	// if the player touches the victory object, it has not already been taken, and the player can move (not dead or victory)
	// then the player has reached the victory point of the level
	void OnTriggerEnter2D (Collider2D other)
	{
		if ((other.tag == "Player" ) && (!taken) && (other.gameObject.GetComponent<CharacterController2D>().playerCanMove) && (GameManager.gm.countPunchCards == 5))
		{
			// mark as taken so doesn't get taken multiple times
			taken=true;

			// do the player victory thing
			other.gameObject.GetComponent<CharacterController2D>().Victory();

			// destroy the victory gameobject
			Destroy(this.gameObject);
		}
	}

}
