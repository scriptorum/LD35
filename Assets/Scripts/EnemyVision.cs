using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
	public EnemyBehavior eb;

	void Awake()
	{
		eb = transform.parent.GetComponent<EnemyBehavior>();
		Debug.Assert(eb != null);
	}

	void OnTriggerStay2D(Collider2D other)
	{ 				
		var canEatEnemies = other.GetComponent<CanEatEnemies>();
		if(canEatEnemies == null) return; // Vision cone only collides with player 

		// A moving player is not visible (unless they get caught)
		if(canEatEnemies.isDisguised())
			return;

		// TODO Also a player that is hidden is not spottable except from up close

		// You've been spotted!
		canEatEnemies.spotted();

		// Determine target position and distance
		Vector3 pathToNoise = other.transform.position - transform.position;
		float dist = pathToNoise.magnitude;

		Debug.Log(gameObject.name + " spotted player at " + dist);

		// Notify enemy behavior
		eb.spottedPlayer(other.gameObject, dist);
	}
}
