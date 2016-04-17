using UnityEngine;
using System.Collections;
using Spewnity;

public class PlayerController : MonoBehaviour
{
	private HunterView view;
	private CanEatEnemies canEatEnemies;

	public void Awake()
	{
		view = gameObject.GetComponent<HunterView>();
		canEatEnemies = gameObject.GetComponent<CanEatEnemies>();
	}

	public void onMovement(InputEvent evt)
	{
		// No motion - slow down player gradually
		if(evt.axis.x == 0)
			canEatEnemies.isMoving = view.decelerate();

		// Speed up (or hard brake) - player controls velocity change
		else canEatEnemies.isMoving = view.accelerate(evt.axis.x);
	}
}
