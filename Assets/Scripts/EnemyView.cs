using UnityEngine;
using System.Collections;

public class EnemyView : MonoBehaviour
{
	public static float ACCELERATION = 12f;
	public static float MIN_VELOCITY = 0.3f;
	public static int sortingOrder;
	public float velocity = 0f;
	public float targetVelocity = 0f;
	public bool flipped;
	public GameObject bulletPrefab;
	public Sprite mouthClosed;
	public Sprite mouthTalk;
	public Sprite mouthYell;
	public Sprite eyesClosed;
	public Sprite eyesCalm;
	public Sprite eyesSquint;
	public Sprite eyesOpen;
	private SpriteRenderer bodySR;
	private SpriteRenderer eyesSR;
	private SpriteRenderer mouthSR;
	private BoxCollider2D visionCollider;

	void Awake()
	{
		bodySR = transform.Find("Body").GetComponent<SpriteRenderer>();
		eyesSR = transform.Find("Face/Eyes").GetComponent<SpriteRenderer>();
		mouthSR = transform.Find("Face/Mouth").GetComponent<SpriteRenderer>();
		visionCollider = transform.Find("Vision").GetComponent<BoxCollider2D>();

		bodySR.sortingOrder = sortingOrder++;
		eyesSR.sortingOrder = sortingOrder++;
		mouthSR.sortingOrder = sortingOrder++;

		Debug.Assert(bodySR != null);
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
		eyesSR.flipX = mouthSR.flipX = flipped;
		this.flipped = flipped;

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
		bodySR.color = color;
	}

	public void fireGun()
	{
		// TODO Raise Gun

		// Shoot bullet
		GameObject go = (GameObject) Instantiate(bulletPrefab, transform.position, Quaternion.identity);

		// Firebullet
		Bullet bullet = go.GetComponent <Bullet>();
		bullet.fire(flipped);
	}

	public void setMouth(MouthType type)
	{
		Sprite sprite = null;
		switch(type)
		{
			case MouthType.Closed:
				sprite = mouthClosed;
				break;
			case MouthType.Talk:
				sprite = mouthTalk;
				break;
			case MouthType.Yell:
				sprite = mouthYell;
				break;
			default: throw new UnityException("WTF");
		}

		mouthSR.sprite = sprite;
	}
	public void setEyes(EyeType type)
	{
		Sprite sprite = null;
		switch(type)
		{
			case EyeType.Calm:
				sprite = eyesCalm;
				break;
			case EyeType.Closed:
				sprite = eyesClosed;
				break;
			case EyeType.Open:
				sprite = eyesOpen;
				break;
			case EyeType.Squint:
				sprite = eyesSquint;
				break;
			default: throw new UnityException("WTF");
		}

		eyesSR.sprite = sprite;
	}
}

public enum MouthType
{
	Closed,
	Talk,
	Yell
}
		
public enum EyeType
{
	Closed,
	Calm,
	Squint,
	Open
}
