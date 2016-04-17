using UnityEngine;
using System.Collections;
using Spewnity;

// Rename to PreyController, in fact rename all Enemy stuff to Prey
public class EnemyBehavior : MonoBehaviour
{
	public static float WORRY_THRESHOLD = 2f;
	public static float ALARM_THRESHOLD = 10f;
	public static float MAX_ANXIETY = 14f;
	public static float ANXIETY_GROWTH = 0.5f;
	private float anxiety = 0f;
	private float targetAnxiety = 0f;
	private float stateTimer = 0f;
	private float lastRange;
	private Vector3 lastPlayerSighting;
	private Vector3 lastNoiseHeard;
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
		if(anxiety != targetAnxiety) anxiety = Mathf.Lerp(anxiety, targetAnxiety, Time.deltaTime * ANXIETY_GROWTH);

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
		targetAnxiety = Mathf.Max(0, targetAnxiety - anxietyDropRate * Time.deltaTime);
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
//		Debug.Log(gameObject.name + " is entering " + func + " lastAnxietyFacing:" + lastRange);
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

		view.setTint(Color.blue);

		GameObject.Destroy(gameObject.GetComponent<EnemyBehavior>());
		GameObject.Destroy(gameObject.GetComponent<CanBeEaten>());
		GameObject.Destroy(gameObject.GetComponent<CanHear>());
		GameObject.Destroy(transform.Find("Vision").gameObject); // Enemy no longer has vision collider

		NoiseManager.instance.addNoise("dying", gameObject);
	}

	public void spottedPlayer(GameObject playerGO, float range)
	{
//		Debug.Log("Spotted player!!!");
		lastPlayerSighting = playerGO.transform.position;

		addAnxiety(MAX_ANXIETY, range);
	}

	public void addAnxiety(float amount, float range)
	{
		targetAnxiety = Mathf.Min(MAX_ANXIETY, targetAnxiety + amount);
		lastRange = range;
	}

	//====================================================================================

	public void walkingState()
	{
		if(stateIsInitializing)
		{ 
			view.setTint(Color.white);
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
			view.setTint(Color.green);
			view.stand(1f); // face away
			stateTimer = Random.Range(0.0f, 2.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(nervousWalkState);
	}

	public void nervousWalkState()
	{
		if(stateIsInitializing)
		{			
			view.setTint(Color.green);
			setMovement(-1f); // look towards
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
			view.setTint(Color.green);
			view.stand(-lastRange);
			stateTimer = Random.Range(1f, 3f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) switchState(searchingWalkState);
	}

	public void searchingWalkState()
	{
		if(stateIsInitializing)
		{			
			view.setTint(Color.green);
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
			view.setTint(Color.red);
			setMovement(-1.5f); // Run
			return;
		}
	}

	public void fightingState()
	{
		if(stateIsInitializing)
		{
			if(anxietyGroup != lastAnxietyGroup) NoiseManager.instance.addNoise("fighting", gameObject);
			view.setTint(Color.red);
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