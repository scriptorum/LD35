using UnityEngine;
using System.Collections;

public class CanHear : MonoBehaviour
{
	public EnemyBehavior eb;
	public static float MAX_FALLOFF = 0.25f;

	void Awake()
	{
		eb = gameObject.GetComponent<EnemyBehavior>();
		Debug.Assert(eb != null);
	}

	void OnTriggerEnter2D(Collider2D other)
	{ 		
		if(other.GetComponent<Noise>() == null) return;

		// Don't react to my own noises!
		Noise noise = other.gameObject.GetComponent<Noise>();
		if (noise.owner == gameObject.name)
			return;

		// Determine amount of anxiety felt by noise
		Vector3 pathToNoise = other.transform.position - transform.position;
		float dist = pathToNoise.magnitude;
		float volumeMod = dist / (noise.noiseType.volume / 2);
		if(volumeMod < MAX_FALLOFF)
			volumeMod = MAX_FALLOFF;
		float anxiety = noise.noiseType.anxiety * volumeMod;

//		Debug.Log("Noise triggered. Actor:" + gameObject.name + " Distance:" + dist + " BaseVolume:" + noise.volume + " BaseAnxiety: " + noise.anxiety +
//			" VolumeMod:" + volumeMod + " FinalAnxiety:" + anxiety);

//		Debug.Log(eb.gameObject.name + " is experiencing " + anxiety + " anxiety facing " + pathToNoise.x);

		// Suffer the anxiety
		eb.addAnxiety(anxiety, pathToNoise.x);
	}
}
