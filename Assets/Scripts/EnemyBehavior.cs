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
	public float lastAnxietyFacing;
	public EnemyView enemy;
	public AnxietyState state;

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


		var nextState = getState();
		switch(nextState)
		{
			case AnxietyState.Calm:
				if(stateTimer > 0) return; // nothing to do if state timer is 0
				
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
				if(nextState > state) NoiseManager.instance.addNoise("huh", gameObject);
				gameObject.GetComponent<SpriteRenderer>().color = Color.green;
				enemy.stand();

				break;
			case AnxietyState.Alarmed:
				if(nextState > state) NoiseManager.instance.addNoise("scream", gameObject);
				gameObject.GetComponent<SpriteRenderer>().color = Color.red;
				enemy.stand(lastAnxietyFacing);
				break;
		}

		state = nextState;
	}

	public AnxietyState getState()
	{
		if(anxiety < WORRY_THRESHOLD) return AnxietyState.Calm;
		if(anxiety < ALARM_THRESHOLD) return AnxietyState.Worried;
		return AnxietyState.Alarmed;
	}

	public void die()
	{
		enemy.setMovement(0f);

		gameObject.GetComponent<SpriteRenderer>().color = Color.blue;

		GameObject.Destroy(gameObject.GetComponent<EnemyBehavior>());
		GameObject.Destroy(gameObject.GetComponent<CanBeEaten>());
		GameObject.Destroy(gameObject.GetComponent<CanHear>());

		NoiseManager.instance.addNoise("ooo", gameObject);
	}

	public void addAnxiety(float amount, float facing)
	{
		StartCoroutine(delayAddAnxiety(amount, facing, Random.Range(0f, 0.35f)));
	}

	IEnumerator delayAddAnxiety(float amount, float facing, float delay)
	{
			yield return new WaitForSeconds(delay);
		anxiety += amount;
		lastAnxietyFacing = facing;		
	}
}

public enum AnxietyState
{
	Calm,
	Worried,
	Alarmed
}