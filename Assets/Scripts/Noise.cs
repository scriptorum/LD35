using UnityEngine;
using System.Collections;
using Spewnity;

public class Noise: MonoBehaviour
{
	public BoxCollider2D box;

	public NoiseType noiseType;
	public string owner;

	public void Awake()
	{
		box = gameObject.GetComponent<BoxCollider2D>();
		Debug.Assert(box != null);
	}

	public void create(NoiseType noiseType, string owner)
	{
		SoundManager.instance.play(noiseType.name);
		this.noiseType = noiseType;
		this.owner = owner;

		Vector2 newSize = box.size;
		newSize.x = noiseType.volume;
		box.size = newSize;

		// Eliminate noise object 
		GameObject.Destroy(gameObject, 1f);
	}
}
