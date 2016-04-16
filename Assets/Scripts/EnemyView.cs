using UnityEngine;
using System.Collections;

public class EnemyView : MonoBehaviour
{
	public static float ACCELERATION = 12f;
	public static float MIN_VELOCITY = 0.3f;
	public float velocity = 0f;
	public float targetVelocity = 0f;

	void Awake()
	{
	}

	void Update()
	{	
		// Adjust velocity quickly
		velocity = Mathf.Lerp(velocity, targetVelocity, ACCELERATION * Time.deltaTime);
		if(Mathf.Abs(velocity) < MIN_VELOCITY)
		{
			velocity = 0f;
			return;
		}
			
		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
	}

	public void setMovement(float speed)
	{
		targetVelocity = speed;
	}
}