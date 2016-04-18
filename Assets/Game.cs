using UnityEngine;
using System.Collections;
using Spewnity;

public class Game : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public EnemyStat[] enemyStats;
	public float enemyPlacementOffset;

	void Start()
	{
		Transform enemyPlaceholder = transform.Find("EnemyPlaceholder");

		enemyStats.Shuffle();
		foreach (EnemyStat stat in enemyStats)
		{
			GameObject go = (GameObject) Instantiate(enemyPrefab, enemyPlaceholder.position, Quaternion.identity);
			Debug.Assert(go != null);
			go.name = stat.name;

			EnemyView view = go.GetComponent<EnemyView>();
			Debug.Assert(view != null);
			view.setGarment(stat.garment);

			EnemyBehavior behavior = go.GetComponent<EnemyBehavior>();
			Debug.Assert(behavior != null);
			behavior.armed = stat.armed;
			behavior.speedModifier = stat.speed;

			// Move placeholder to next position
			Vector3 pos = enemyPlaceholder.position;
			pos.x += enemyPlacementOffset;
			enemyPlaceholder.position = pos;
		}
	}
}

[System.Serializable]
public struct EnemyStat
{
	public string name;
	public bool armed;
	public GarmentType garment;
	public float speed;
}