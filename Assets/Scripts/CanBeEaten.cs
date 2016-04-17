using UnityEngine;
using System.Collections;

public class CanBeEaten : MonoBehaviour
{
	public EnemyBehavior eb;

	void Awake()
	{
		eb = gameObject.GetComponent<EnemyBehavior>();
		Debug.Assert(eb != null);
	}

	void Update()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{ 		
		var canEatEnemies = other.GetComponent<CanEatEnemies>();
		if(canEatEnemies == null)
			return;

		// Can't eat while disguised
		if(canEatEnemies.isDisguised())
			return;

//		Debug.Log(gameObject.name + " is death colliding with " + other.gameObject.name);
		eb.die();
	}
}
