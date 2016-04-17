using UnityEngine;
using System.Collections;

public class EnemyView : MonoBehaviour
{
	public static float ACCELERATION = 12f;
	public static float MIN_VELOCITY = 0.3f;
	public float velocity = 0f;
	public float targetVelocity = 0f;
	private SpriteRenderer faceSR;
	private BoxCollider2D visionCollider;

	void Awake()
	{
		faceSR = transform.Find("Face").GetComponent<SpriteRenderer>();
		visionCollider = transform.Find("Vision").GetComponent<BoxCollider2D>();

		Debug.Assert(faceSR != null);
		Debug.Assert(visionCollider != null);
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
		else setFacing(velocity < 0); // only set facing when we're still moving

		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
	}

	private void setFacing(bool flipped) // flipped means left
	{
		// Set facing of face sprite
		faceSR.flipX = flipped;

		// Set position of vision cone
		if(visionCollider)
		{
			Vector2 offset = visionCollider.offset;
			offset.x = Mathf.Abs(offset.x) * (flipped ? -1 : 1);
			visionCollider.offset = offset;
		}
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
		setFacing(facing < 0);
	}

	public void setTint(Color color)
	{
		faceSR.color = color;
	}
}