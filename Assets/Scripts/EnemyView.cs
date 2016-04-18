using UnityEngine;
using System.Collections;

public class EnemyView : MonoBehaviour
{
	public static string SORTING_LAYER = "Enemies";
	public static float ACCELERATION = 12f;
	public static float MIN_VELOCITY = 0.3f;
	public static int sortingOrder;
	public float velocity = 0f;
	public float targetVelocity = 0f;
	public bool flipped;
	public GameObject bulletPrefab;
	private SpriteRenderer bodySR;
	private BoxCollider2D visionCollider;
	private Eyes eyes;
	private Mouth mouth;
	private Garment garment;
	private GarmentType delayedGarment = GarmentType.None;

	void Start()
	{
		bodySR = transform.Find("Body").GetComponent<SpriteRenderer>();
		eyes = transform.Find("Face/Eyes").GetComponent<Eyes>();
		mouth = transform.Find("Face/Mouth").GetComponent<Mouth>();
		garment = transform.Find("Garment").GetComponent<Garment>();
		visionCollider = transform.Find("Vision").GetComponent<BoxCollider2D>();

		Debug.Assert(bodySR != null);
		Debug.Assert(visionCollider != null);
		Debug.Assert(mouth != null);
		Debug.Assert(eyes != null);
		Debug.Assert(garment != null);

		bodySR.sortingOrder = sortingOrder++;
		mouth.setLayer(SORTING_LAYER, sortingOrder++);
		eyes.setLayer(SORTING_LAYER, sortingOrder++);
		garment.setLayer(SORTING_LAYER, sortingOrder++);
		if(delayedGarment != GarmentType.None)
			garment.setGarment(delayedGarment);
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
		eyes.setFlipped(flipped);
		mouth.setFlipped(flipped);
		garment.setFlipped(flipped);
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

	public void setEyes(EyeType type)
	{
		eyes.setEyes(type);
	}

	public void setMouth(MouthType type)
	{
		mouth.setMouth(type);
	}

	public void setGarment(GarmentType type)
	{
		if(garment == null)
			delayedGarment = type;
		else garment.setGarment(type);
	}
}
		
