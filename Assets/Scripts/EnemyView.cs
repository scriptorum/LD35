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
	public bool flipped = true;
	public GameObject bulletPrefab;
	private SpriteRenderer bodySR;
	private BoxCollider2D visionCollider;
	private Eyes eyes;
	private Mouth mouth;
	private Garment garment;
	private Animator gunMount;
	private Gun gun;
	private GarmentType delayedGarment = GarmentType.None;
	public bool armed = false;

	void Awake()
	{
		bodySR = transform.Find("Body").GetComponent<SpriteRenderer>();
		eyes = transform.Find("Face/Eyes").GetComponent<Eyes>();
		mouth = transform.Find("Face/Mouth").GetComponent<Mouth>();
		garment = transform.Find("Garment").GetComponent<Garment>();
		visionCollider = transform.Find("Vision").GetComponent<BoxCollider2D>();
		gunMount = transform.Find("GunMount").GetComponent<Animator>();
		gun = transform.Find("GunMount/Gun").GetComponent<Gun>();

		Debug.Assert(bodySR != null);
		Debug.Assert(visionCollider != null);
		Debug.Assert(mouth != null);
		Debug.Assert(eyes != null);
		Debug.Assert(garment != null);
		Debug.Assert(garment != null);
	}

	void Start()
	{
		bodySR.sortingOrder = sortingOrder++;
		mouth.setLayer(SORTING_LAYER, sortingOrder++);
		eyes.setLayer(SORTING_LAYER, sortingOrder++);
		garment.setLayer(SORTING_LAYER, sortingOrder++);
		gun.setLayer(SORTING_LAYER, sortingOrder++);
		updateGunMount();
		if(delayedGarment != GarmentType.None) garment.setGarment(delayedGarment);
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

	public void halt()
	{
		targetVelocity = velocity = 0f;
	}

	public void updateGunMount()
	{		
		bool active = this.armed && this.flipped;
		gunMount.enabled = active;
		gun.gameObject.SetActive(active);
	}

	public void setFacing(bool flipped) // flipped means left
	{
		// Set facing of face sprite
		eyes.setFlipped(flipped);
		mouth.setFlipped(flipped);
		garment.setFlipped(flipped);
		this.flipped = flipped;
		updateGunMount();

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

//	public bool wieldingGun = false;
//	public void drawGun()
//	{
//		gunMount.SetTrigger("draw");
//		wieldingGun = true;
//	}

	public void fireGun(bool shootStraight = false)
	{
//		gunMount.SetTrigger("fire");
		NoiseManager.instance.addNoise("gunshot", gameObject);

		// Shoot bullet
		GameObject go = (GameObject) Instantiate(bulletPrefab, transform.position, Quaternion.identity);
		Bullet bullet = go.GetComponent <Bullet>();
		bullet.fire(flipped, shootStraight);
	}

//	public void holsterGun()
//	{
//		wieldingGun = false;
//		gunMount.SetTrigger("holster");
//	}

//	public void gunMountEvent(AnimationEvent evt)
//	{
//		switch(evt.stringParameter)
//		{
//		}
//	}

	public void setEyes(EyeType type)
	{
		eyes.setEyes(type);
	}

	public void setMouth(MouthType type)
	{
		mouth.setMouth(type);
	}

	public MouthType getMouth()
	{
		return mouth.type;
	}

	public void setGarment(GarmentType type)
	{
		if(garment == null) delayedGarment = type;
		else garment.setGarment(type);
	}
}
		
