using UnityEngine;
using System.Collections;
using Spewnity;

public class Hunter : MonoBehaviour
{
	public static int START_HEALTH = 3;

	private static float DISGUISE_TIMER = 5.0f;
	public bool isMoving;
	public float spottedTimer;
	public int health = START_HEALTH;
	private HunterView view;
	private bool wasDisguised;

	public void Awake()
	{
		view = gameObject.GetComponent<HunterView>();
	}

	public bool isDisguised()
	{
		return !isMoving && spottedTimer <= 0;
	}

	public void spotted()
	{
		spottedTimer = DISGUISE_TIMER;
	}

	public void gotShot()
	{
		health--;
		if(health <= 0)
		{
			SoundManager.instance.play("player-dead");
			view.die();
		}
		else
		{
			SoundManager.instance.play("player-hurt");
			view.hurt();
		}
	}

	public void Update()
	{
		bool disguised = isDisguised();
		if(wasDisguised != disguised)
		{
			wasDisguised = disguised;
			view.setDisguise(disguised);
		}

		if(spottedTimer <= 0)
			return;

		spottedTimer -= Time.deltaTime;
	}
}
