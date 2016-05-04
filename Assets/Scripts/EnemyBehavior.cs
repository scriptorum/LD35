using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Spewnity;

// Rename to PreyController, in fact rename all Enemy stuff to Prey
public class EnemyBehavior : MonoBehaviour
{
	public static float WORRY_THRESHOLD = 2f;
	public static float ALARM_THRESHOLD = 10f;
	public static float MAX_ANXIETY = 14f;
	public static float ANXIETY_GROWTH = 0.5f;
	public static float TIME_UNTIL_FORGET_TARGET = 5f;
	private static EnemyEvent changeRunMode = new EnemyEvent();
	private static EnemyRunMode globalRunMode = EnemyRunMode.Paused;

	private float anxiety = 0f;
	private float targetAnxiety = 0f;
	private float stateTimer = 0f;
	private float lastRange;
	private float endDirection = 1f;
	private float startDirection = -1f;
	private float targetTimer;
	private System.Nullable<Vector3> lastPlayerSighting;
	private Vector3 lastNoiseHeard;
	public EnemyView view;
	private  AnxietyGroup anxietyGroup;
	private System.Action stateFunc = null;
	private AnxietyGroup lastAnxietyGroup;
	public bool stateIsInitializing = true;
	public float anxietyDropRate = 2f;
	public float speedModifier = 1f;
	public bool armed = false;
	public EnemyRunMode runMode = EnemyRunMode.Paused;

	void Awake()
	{
		view = gameObject.GetComponent<EnemyView>();
		runMode = globalRunMode;
		changeRunMode.AddListener((mode) =>
		{
			this.runMode = mode;
			stateFunc = null;
		});
	}

	public static void setRunMode(EnemyRunMode mode)
	{
		globalRunMode = mode;
		changeRunMode.Invoke(EnemyRunMode.RunScript);
	}

	void Update()
	{
		switch(runMode)
		{
			case EnemyRunMode.Paused:
				return;

			case EnemyRunMode.RunScript:
				updateState();
				return;

			case EnemyRunMode.StopScript:
				runMode = EnemyRunMode.Running;
				setState(walkingState);
				break;
		}

		targetTimer -= Time.deltaTime;
		if(targetTimer <= 0) lastPlayerSighting = null;
		
		if(anxiety != targetAnxiety) anxiety = Mathf.Lerp(anxiety, targetAnxiety, Time.deltaTime * ANXIETY_GROWTH);

		lastAnxietyGroup = anxietyGroup;
		anxietyGroup = getState();

		// See if anxiety has raised or lowered status
		if(anxietyGroup != lastAnxietyGroup)
		{
			if(anxietyGroup == AnxietyGroup.Calm) setState(walkingState);

			if(anxietyGroup == AnxietyGroup.Worried) if(armed) setState(searchingState);
			else setState(nervousState);

			if(anxietyGroup == AnxietyGroup.Alarmed) if(armed) setState(fightingState);
			else setState(fleeingState);

//			if(anxietyGroup < lastAnxietyGroup && view.wieldingGun)
//				view.holsterGun();
		}
		
		// Update state
		updateState();

		// Reduce anxiety over time
		targetAnxiety = Mathf.Max(0, targetAnxiety - anxietyDropRate * Time.deltaTime);
	}

	public void setMovement(float speed)
	{
		// Adjust speed modifier to make some folks slower or faster
		view.setMovement(speed * speedModifier);
	}

	public void setState(System.Action func)
	{
		stateIsInitializing = true;
		stateFunc = func;
		if(stateFunc != null)
			stateFunc();
	}

	public void updateState()
	{
		stateIsInitializing = false;
		if(stateFunc != null) stateFunc();
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
		view.setMouth(MouthType.Yell);
		view.setEyes(EyeType.Closed);

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
		targetTimer = TIME_UNTIL_FORGET_TARGET;
		addAnxiety(MAX_ANXIETY, range);
	}

	public void heardNoise(float amount, float range)
	{
//		lastNoiseHeard = Noise noise
		addAnxiety(amount, range);
	}

	public void addAnxiety(float amount, float range)
	{
		targetAnxiety = Mathf.Min(MAX_ANXIETY, targetAnxiety + amount);
		lastRange = range;
	}

	public float getTargetRange()
	{
		if(lastPlayerSighting != null)
		{
			Vector3 pathToTarget = (Vector3) lastPlayerSighting - transform.position;
			float dist = pathToTarget.magnitude;
			return dist;
		}

		return lastRange;
	}
		
	//====================================================================================
	//===== Standard enemy states
	//====================================================================================

	public void walkingState()
	{
		if(stateIsInitializing)
		{ 
			view.setTint(Color.white);
			view.setMouth(MouthType.Closed);
			view.setEyes(EyeType.Calm);
			setMovement(endDirection); // walking toward base
			stateTimer = Random.Range(2f, 20f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(smokingState);
	}

	public void smokingState()
	{
		if(stateIsInitializing)
		{
			view.setEyes(EyeType.Closed);
			view.stand();
			stateTimer = Random.Range(2f, 10f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(walkingState);
	}

	public void chattingState()
	{
	}

	public void nervousState()
	{
		if(stateIsInitializing)
		{
			view.setMouth(MouthType.Talk);
			view.setEyes(EyeType.Open);
			NoiseManager.instance.addNoise("nervous", gameObject);
			view.setTint(Color.green);
			view.stand(startDirection); // face away
			stateTimer = Random.Range(0.0f, 2.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(nervousWalkState);
	}

	public void nervousWalkState()
	{
		if(stateIsInitializing)
		{			
			view.setMouth(MouthType.Closed);
			view.setTint(Color.green);
			setMovement(endDirection); // look towards
			stateTimer = Random.Range(2.0f, 6.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(nervousState);
	}

	public void searchingState()
	{
		if(stateIsInitializing)
		{
			view.setEyes(EyeType.Squint);
			view.setMouth(MouthType.Talk);
			NoiseManager.instance.addNoise("searching", gameObject);
			view.setTint(Color.green);
			view.stand(-getTargetRange());
			stateTimer = Random.Range(startDirection, 3f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(searchingWalkState);
	}

	public void searchingWalkState()
	{
		if(stateIsInitializing)
		{			
			view.setMouth(MouthType.Closed);
			view.setTint(Color.green);
			setMovement(0.5f * Mathf.Sign(getTargetRange())); // walk slowly toward noise
			stateTimer = Random.Range(2.0f, 6.0f);
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(searchingState);
	}


	public void fleeingState()
	{
		if(stateIsInitializing)
		{
			view.setMouth(MouthType.Yell);
			view.setEyes(EyeType.Open);
			NoiseManager.instance.addNoise("fleeing", gameObject);
			view.setTint(Color.red);
			setMovement(-1.5f); // Run
			return;
		}
	}

	public static float MIN_FIRING_RANGE = 3f;
	public static float MAX_FIRING_RANGE = 8f;

	public void fightingState()
	{
		if(stateIsInitializing)
		{
//			view.drawGun();
			view.setMouth(MouthType.Yell);
			if(anxietyGroup != lastAnxietyGroup) NoiseManager.instance.addNoise("fighting", gameObject);
			view.setTint(Color.red);
			float range = getTargetRange();

			// Move closer?
			if(range > MIN_FIRING_RANGE) setMovement(1.2f * Mathf.Sign(getTargetRange())); // walk quickly toward noise
			else view.stand();

			// Fire gun?
			if(range < MAX_FIRING_RANGE && lastPlayerSighting != null) view.fireGun();

			stateTimer = Random.Range(.35f, 1f);
			return;
		}

		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) setState(fightingState);
	}

	// TODO Add stunned state, stunned moment before taking/resuming action


	/***************************************************************************/

	public void talk()
	{
		Debug.Log(gameObject.name + " is talking! Init:" + stateIsInitializing + " timer:" + stateTimer);
		if(stateIsInitializing)
			stateTimer = 0f;
		else stateTimer -= Time.deltaTime;

		if(stateTimer <= 0f)
		{
			view.setMouth(view.getMouth() == MouthType.Closed ? MouthType.Talk : MouthType.Closed);
			stateTimer = Random.Range(0.5f, 0.2f);
		}
	}

	public void quiet()
	{
		if(stateIsInitializing)
			view.setMouth(MouthType.Closed);
	}

}

public enum AnxietyGroup
{
	Calm,
	Worried,
	Alarmed
}

public enum EnemyRunMode
{
	Paused,
	Running,
	RunScript,
	StopScript,
}

public class EnemyEvent: UnityEvent<EnemyRunMode>
{
}