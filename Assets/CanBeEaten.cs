using UnityEngine;
using System.Collections;

public class CanBeEaten : MonoBehaviour
{
	public EnemyView enemy;

	void Awake()
	{
		enemy = gameObject.GetComponent<EnemyView>();
	}

	void Update()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{ 		
		if(other.GetComponent<CanEatEnemies>() == null || gameObject.GetComponent<CanBeEaten>() == null)
			return;
		
		enemy.setMovement(0f);
		Debug.Log("Eaten!");

		gameObject.GetComponent<SpriteRenderer>().color = Color.blue;

		GameObject.Destroy(gameObject.GetComponent<EnemyBehavior>());
		GameObject.Destroy(gameObject.GetComponent<CanBeEaten>());
	}
}
