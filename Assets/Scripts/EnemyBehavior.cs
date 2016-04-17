using UnityEngine;
using System.Collections;
using Spewnity;

public class EnemyBehavior : MonoBehaviour
{
	public static float WORRY_THRESHOLD = 2f;
	public static float ALARM_THRESHOLD = 10f;
	public static float MAX_ANXIETY = 14f;
	private float anxiety = 0f;
	private float stateTimer = 0f;
	private float lastRange;
	private EnemyView view;
	private  AnxietyGroup anxietyGroup;
	private System.Action stateFunc;
	public string currentState;
	private AnxietyGroup lastAnxietyGroup;
	private bool stateIsInitializing = true;
	public float bravery = 0.5f;
	public bool armed = false;
	public float anxietyDropRate = 2f;
	public float speedModifier = 1f;

	void Awake()
	{
		view = gameObject.GetComponent<EnemyView>();
	}

	void Start()
	{
		switchState(walkingState);
	}

	void Update()
	{
		lastAnxietyGroup = anxietyGroup;
		anxietyGroup = getState();

		// See if anxiety has raised or lowered status
		if(anxietyGroup != lastAnxietyGroup)
		{
			if(anxietyGroup == AnxietyGroup.Calm) switchState(walkingState);

			if(anxietyGroup == AnxietyGroup.Worried) if(bravery > Random.value) switchState(searchingState);
			else switchState(nervousState);

			if(anxietyGroup == AnxietyGroup.Alarmed) if(bravery > Random.value) switchState(fightingState);
			else switchState(fleeingState);
		}
		
		// Update state
		else updateState();

		// Reduce anxiety over time
		anxiety = Mathf.Max(0, anxiety - anxietyDropRate * Time.deltaTime);
	}

	public void setMovement(float speed)
	{
		view.setMovement(speed * speedModifier);
	}

	public void switchState(System.Action func)
	{
		stateIsInitializing = true;
		stateFunc = func;
		currentState = func.Method.Name;
		Debug.Log(gameObject.name + " is entering " + func + " lastAnxietyFacing:" + lastRange);
		stateFunc();
	}

	public void updateState()
	{
		stateIsInitializing = false;
		stateFunc();
	}

	public AnxietyGroup getState()
	{
		if(anxiety < WORRY_THRESHOLD) return AnxietyGroup.Calm;
		if(anxiety < ALARM_THRESHOLD) return AnxietyGroup.Worried;
		return AnxietyGroup.Alarmed;
	}

	public void die()
	{
		setMovement(0f);

		gameObject.GetComponent<SpriteRenderer>().color = Color.blue;

		GameObject.Destroy(gameObject.GetComponent<EnemyBehavior>());
		GameObject.Destroy(gameObject.GetComponent<CanBeEaten>());
		GameObject.Destroy(gameObject.GetComponent<CanHear>());

		NoiseManager.instance.addNoise("dying", gameObject);
	}

	public void addAnxiety(float amount, float range)
	{
		// TODO If delay already running, add anxiet together and adjust facing
		StartCoroutine(delayAddAnxiety(amount, range, Random.Range(0.1f, 0.8f)));
	}

	IEnumerator delayAddAnxiety(float amount, float range, float delay)
	{
		yield return new WaitForSeconds(delay);
		anxiety += amount;
		lastRange = range;		
	}

	//====

	public void walkingState()
	{
		if(stateIsInitializing)
		{ 
			gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			setMovement(-1f); // walking toward base
			stateTimer = Random.Range(2f, 20f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(smokingState);
	}

	public void smokingState()
	{
		if(stateIsInitializing)
		{
			view.stand();
			stateTimer = Random.Range(2f, 10f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(walkingState);
	}

	public void chattingState()
	{
	}

	public void nervousState()
	{
		if(stateIsInitializing)
		{
			NoiseManager.instance.addNoise("nervous", gameObject);
			gameObject.GetComponent<SpriteRenderer>().color = Color.green;
			view.stand(lastRange);
			stateTimer = Random.Range(0.0f, 2.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(nervousWalkState);
	}

	public void nervousWalkState()
	{
		if(stateIsInitializing)
		{			
			gameObject.GetComponent<SpriteRenderer>().color = Color.green;
			setMovement(-1f);
			stateTimer = Random.Range(2.0f, 6.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(nervousState);
	}

	public void searchingState()
	{
		if(stateIsInitializing)
		{
			NoiseManager.instance.addNoise("searching", gameObject);
			gameObject.GetComponent<SpriteRenderer>().color = Color.green;
			view.stand(lastRange);
			stateTimer = Random.Range(0f, 2f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(searchingWalkState);
	}

	public void searchingWalkState()
	{
		if(stateIsInitializing)
		{			
			gameObject.GetComponent<SpriteRenderer>().color = Color.green;
			setMovement(0.5f * Mathf.Sign(lastRange)); // walk slowly toward noise
			stateTimer = Random.Range(2.0f, 6.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(nervousState);
	}


	public void fleeingState()
	{
		if(stateIsInitializing)
		{
			NoiseManager.instance.addNoise("fleeing", gameObject);
			gameObject.GetComponent<SpriteRenderer>().color = Color.red;
			setMovement(-1.5f); // Run
			return;
		}
	}
		
	public void fightingState()
	{
		if(stateIsInitializing)
		{
			NoiseManager.instance.addNoise("fighting", gameObject);
			gameObject.GetComponent<SpriteRenderer>().color = Color.red;
			setMovement(1.2f * Mathf.Sign(lastRange)); // walk quickly toward noise
			stateTimer = lastRange / 1.2f;
			return;
			// TODO If you SEE circle, make circle position your target instead of last noise
			// TODO Fire fire fire!!
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(fightingState);
	}

	// TODO Add stunned state, stunned moment before taking/resuming action
}

public enum AnxietyGroup
{
	Calm,
	Worried,
	Alarmed
}