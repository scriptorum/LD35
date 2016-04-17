using UnityEngine;
using System.Collections;

public class CanEatEnemies : MonoBehaviour
{
	private static float DISGUISE_TIMER = 5.0f;
	public bool isMoving;
	public float spottedTimer;

	public bool isDisguised()
	{
		return !isMoving && spottedTimer <= 0;
	}

	public void spotted()
	{
		spottedTimer = DISGUISE_TIMER;
	}

	public void Update()
	{
		if(spottedTimer <= 0)
			return;

		spottedTimer -= Time.deltaTime;
	}
}
