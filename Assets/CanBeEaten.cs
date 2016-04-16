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
		if(other.GetComponent<CanEatEnemies>() == null || gameObject.GetComponent<CanBeEaten>() == null)
			return;

		Debug.Log(gameObject.name + " has gone circle!");
		eb.die();
	}
}
