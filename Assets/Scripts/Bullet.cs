using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public static float BULLET_SPEED = 7f;
	public Vector3 velocity;

	void Update()
	{
		transform.Translate(velocity * Time.deltaTime);
	}

	public void fire(bool flipped, bool shootStraight = false)
	{
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		sr.flipX = flipped;
		velocity = new Vector3(flipped ? -BULLET_SPEED : BULLET_SPEED, (shootStraight ? 0f : Random.Range(-BULLET_SPEED, BULLET_SPEED)/4), 0);

		Invoke("selfDestruct", 1f);
	}

	public void selfDestruct()
	{
		GameObject.Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var hunter = other.gameObject.GetComponent<Hunter>();
		if(hunter == null)
			return;

		hunter.gotShot();
		selfDestruct();
	}
}
