using UnityEngine;
using System.Collections;

public class HunterView : MonoBehaviour
{
	private static float SLOW_FACTOR = 2f;
	public static float ACCELERATION = 8f;
	public static float MIN_VELOCITY = 0.5f;
	public static float MAX_VELOCITY = 5f;
	public bool moving = false;
	public float facing = -1.0f;
	public float velocity = 0;
	private SpriteRenderer faceSR;

	void Awake()
	{
		faceSR = transform.Find("Face").GetComponent<SpriteRenderer>();
	}

	void Update()
	{						
		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
		faceSR.flipX = facing < 0;
	}
		
	// Returns true if still moving
	public bool decelerate()
	{
		velocity -= Mathf.Sign(velocity) * SLOW_FACTOR * Time.deltaTime;

		// Come to stop if going slow enough
		moving = (Mathf.Abs(velocity) >= MIN_VELOCITY);
		if(!moving)
			velocity = 0f;

		return moving;
	}

	public bool accelerate(float amount)
	{
		// Adjust velocity
		velocity += amount * ACCELERATION * Time.deltaTime;

		// Enforce maximum velocity
		float absVelocity = Mathf.Abs(velocity);
		float signVelocity = Mathf.Sign(velocity);
		if(absVelocity > MAX_VELOCITY)
			velocity = signVelocity * MAX_VELOCITY;

		// Change facing if moving fast enough, otherwise assume old facing
		if(absVelocity > MIN_VELOCITY)
			facing = signVelocity;

		return true;
	}
}
