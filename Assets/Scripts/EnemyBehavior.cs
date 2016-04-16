using UnityEngine;
using System.Collections;
using Spewnity;

public class EnemyBehavior : MonoBehaviour
{
	public static float WORRY_THRESHOLD = 5f;
	public static float ALARM_THRESHOLD = 10f;
	public static float MAX_ANXIETY = 15f;
	public float anxiety = 0f;
	public float stateTimer = 0f;
	public EnemyView enemy;

	void Awake()
	{
		enemy = gameObject.GetComponent<EnemyView>();
	}

	void Start()
	{
//		enemy.setMovement(-1f);	
	}

	void Update()
	{
		stateTimer -= Time.deltaTime;
		if(stateTimer > 0)
			return;

		switch(getState())
		{
			case AnxietyState.Calm:
				if(enemy.targetVelocity == 0)
				{
					enemy.setMovement(-1f); // walking
					stateTimer = Random.Range(2f, 20f);
				}
				else
				{
					enemy.setMovement(0f); // standing
					stateTimer = Random.Range(2f, 10f);
				}
				break;

			case AnxietyState.Worried:
				break;
			case AnxietyState.Alarmed:
				break;
		}
	}

	public AnxietyState getState()
	{
		if(anxiety < WORRY_THRESHOLD) return AnxietyState.Calm;
		if(anxiety < ALARM_THRESHOLD) return AnxietyState.Worried;
		return AnxietyState.Alarmed;
	}

}

public enum AnxietyState
{
	Calm,
	Worried,
	Alarmed
}