using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoiseManager : MonoBehaviour
{
	private Dictionary<string, NoiseType> nameToNoise = new Dictionary<string, NoiseType>();
	public static NoiseManager instance = null;
	public GameObject noisePrefab;
	public NoiseType[] noiseTypes;

	void Awake()
	{
		if(instance == null) instance = this;
		else if(instance != this) Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		Debug.Assert(noisePrefab != null);

		foreach(NoiseType nt in noiseTypes)
			nameToNoise.Add(nt.name, nt);
	}

	public Noise addNoise(string noiseName, GameObject owner)
	{
		Debug.Assert(nameToNoise.ContainsKey(noiseName));

		GameObject go = (GameObject) Instantiate(noisePrefab, owner.transform.position, Quaternion.identity);
		Noise noise = go.GetComponent<Noise>();
		NoiseType nt = nameToNoise[noiseName];
		noise.create(nt, owner.name);
		return noise;
	}

}

[System.Serializable]
public struct NoiseType
{
	public string name;
	public float volume;
	public float anxiety;
}
