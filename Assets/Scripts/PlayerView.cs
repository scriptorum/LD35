using UnityEngine;
using System.Collections;
using Spewnity;

public class PlayerView : MonoBehaviour
{
	public static float ACCELERATION = 8f;
	public static float SLOW_FACTOR = 2f;
	public static float MIN_VELOCITY = 1f;
	public static float MAX_VELOCITY = 5f;
	public bool moving = false;
	public float facing = -1.0f;
	public float velocity = 0;
	private SpriteRenderer sr;

	void Awake()
	{
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{						
		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
		sr.flipX = facing < 0;
	}

	public void onMovement(InputEvent evt)
	{
		// No motion - slow down player gradually
		if(evt.axis.x == 0)
		{
			velocity -= Mathf.Sign(velocity) * SLOW_FACTOR * Time.deltaTime;

			// Come to stop if going slow enough
			moving = (Mathf.Abs(velocity) >= MIN_VELOCITY);
			if(!moving)
				velocity = 0f;
		}

		// Speed up (or hard brake) - player controls velocity change
		else
		{
			// Adjust velocity
			velocity += evt.axis.x * ACCELERATION * Time.deltaTime;

			// Enforce maximum velocity
			float absVelocity = Mathf.Abs(velocity);
			float signVelocity = Mathf.Sign(velocity);
			if(absVelocity > MAX_VELOCITY)
				velocity = signVelocity * MAX_VELOCITY;

			// Change facing if moving fast enough, otherwise assume old facing
			if(absVelocity > MIN_VELOCITY)
				facing = signVelocity;
		}
	}
}
