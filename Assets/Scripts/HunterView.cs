using UnityEngine;
using System.Collections;

public class HunterView : MonoBehaviour
{
	public static string SORTING_LAYER = "Player";
	private static float SLOW_FACTOR = 2f;
	public static float ACCELERATION = 8f;
	public static float MIN_VELOCITY = 0.5f;
	public static float MAX_VELOCITY = 5f;
	public static string MAIN_COLOR = "#E0C732";
	public static string DISGUISE_COLOR = "#84C2C4";
	public static int sortingOrder;
	public bool moving = false;
	public float facing = -1.0f;
	public float velocity = 0;
	public float cameraPosition = 0.33f;
	public Cameraman cameraman;
	private SpriteRenderer bodySR;
	private ParticleSystem bloodspray;
	private Color mainColor;
	private Color disguiseColor;
	private Eyes eyes;
	private Mouth mouth;
	private Garment garment;

	void Awake()
	{
		bodySR = transform.Find("Body").GetComponent<SpriteRenderer>();
		bloodspray = transform.Find("Bloodspray").GetComponent<ParticleSystem>();
		eyes = transform.Find("Face/Eyes").GetComponent<Eyes>();
		mouth = transform.Find("Face/Mouth").GetComponent<Mouth>();
		garment = transform.Find("Garment").GetComponent<Garment>();
		cameraman = Camera.main.GetComponent<Cameraman>();
	}

	void Start()
	{
		bodySR.sortingOrder = sortingOrder++;
		mouth.setLayer(SORTING_LAYER, sortingOrder++);
		eyes.setLayer(SORTING_LAYER, sortingOrder++);
		garment.setLayer(SORTING_LAYER, sortingOrder++);

		mainColor = getColor(MAIN_COLOR);
		disguiseColor = getColor(DISGUISE_COLOR);

		setColor(mainColor);
	}

	void Update()
	{						
		// Update position
		transform.Translate(Vector3.right * Time.deltaTime * velocity);
		bool flipped = facing < 0;
		eyes.setFlipped(flipped);
		mouth.setFlipped(flipped);
		cameraman.leading.x = (flipped ? -1 : 1) * Mathf.Abs(cameraman.leading.x);
	}

	public Color getColor(string htmlColor)
	{
		Color color = new Color();
		if(ColorUtility.TryParseHtmlString(htmlColor, out color) == false) throw new UnityException("Bad color:" + htmlColor);
		return color;
	}

	public void setColor(Color c)
	{
		bodySR.color = c;
	}

	public static float DISGUISE_TIME = 0.6f;

	public IEnumerator lerpColor(Color startColor, Color endColor)
	{
		float t = 0;

		do
		{
			t += Time.deltaTime;
			bodySR.color = Color.Lerp(startColor, endColor, t / DISGUISE_TIME);
			yield return null;
		}
		while (t < 1.0f);
	}

	public void setDisguise(bool disguised)
	{
		if(disguised)
		{
			StartCoroutine(lerpColor(mainColor, disguiseColor));
			mouth.setMouth(MouthType.Smile);
		}
		else
		{
			StartCoroutine(lerpColor(disguiseColor, mainColor));
			mouth.setMouth(MouthType.Closed);
		}
	}
		
	// Returns true if still moving
	public bool decelerate()
	{
		velocity -= Mathf.Sign(velocity) * SLOW_FACTOR * Time.deltaTime;

		// Come to stop if going slow enough
		moving = (Mathf.Abs(velocity) >= MIN_VELOCITY);
		if(!moving) velocity = 0f;

		return moving;
	}

	public bool accelerate(float amount)
	{
		// Adjust velocity
		velocity += amount * ACCELERATION * Time.deltaTime;

		// Enforce maximum velocity
		float absVelocity = Mathf.Abs(velocity);
		float signVelocity = Mathf.Sign(velocity);
		if(absVelocity > MAX_VELOCITY) velocity = signVelocity * MAX_VELOCITY;

		// Change facing if moving fast enough, otherwise assume old facing
		if(absVelocity > MIN_VELOCITY) facing = signVelocity;

		return true;
	}

	public void hurt()
	{
		bloodspray.Emit(10);
		mouth.setMouth(MouthType.Talk);
		eyes.setEyes(EyeType.Squint);
		Invoke("restoreFace", 0.25f);
	}

	public void restoreFace()
	{
		if(dead) return;
		mouth.setMouth(MouthType.Closed);
		eyes.setEyes(EyeType.Open);
	}

	public void setTint(Color color)
	{
		bodySR.color = color;
	}

	public bool dead = false;

	public void die()
	{
		// TODO Replace with death animation, culminating in dead circle
//		GameObject.Destroy(gameObject);
		dead = true;
		Debug.Log("TODO: You died");
		bloodspray.Emit(100);
		mouth.setMouth(MouthType.Yell);
		eyes.setEyes(EyeType.Closed);

		GameObject.Destroy(gameObject.GetComponent<PlayerController>());
		GameObject.Destroy(gameObject.GetComponent<Hunter>());
	}
}
