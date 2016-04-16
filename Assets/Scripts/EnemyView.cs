using UnityEngine;
using System.Collections;

public class EnemyView : MonoBehaviour
{
	public static float ACCELERATION = 12f;
	public static float MIN_VELOCITY = 0.3f;
	public float velocity = 0f;
	public float targetVelocity = 0f;
	public SpriteRenderer sr;

	void Awake()
	{
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{	
		// Adjust velocity quickly
		velocity = Mathf.Lerp(velocity, targetVelocity, ACCELERATION * Time.deltaTime);

		// Clamp minimum velocity
		if(Mathf.Abs(velocity) < MIN_VELOCITY)
		{
			velocity = 0f;
			return;
		}
		else
		{
			// Set facing
			sr.flipX = (velocity < 0);
		}
			
		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
	}

	public void setMovement(float speed)
	{
		targetVelocity = speed;
	}

	public void stand()
	{
		setMovement(0f);
	}

	public void stand(float facing) // -1 face left, 1 face right
	{
		stand();
		sr.flipX = (facing < 0);
	}
}