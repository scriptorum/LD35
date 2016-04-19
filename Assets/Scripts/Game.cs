using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Spewnity;

public class Game : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public EnemyStat[] enemyStats;
	public float enemyPlacementOffset;
	private Vector3 enemyPlaceholder;

	public void Awake()
	{
		enemyPlaceholder = transform.Find("EnemyPlaceholder").position;
	}

	public void Start()
	{
		Camera.main.GetComponent<Cameraman>().setGoal(CameramanTarget.None);
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
		SoundManager.instance.play("theme");
	}

	public void skipIntro()
	{
		GameObject.Find("/Titling").SetActive(false);
		startGame();
	}

	public void startGame(Button startButton)
	{	
		startButton.interactable = false;
		CanvasGroup titling = GameObject.Find("/Titling").GetComponent<CanvasGroup>();
		StartCoroutine(titling.alpha.LerpFloat(0f, 2f, (float a) => titling.alpha = a, null, startScript));
	}

	public void startScript()
	{
		addEnemies();
		startGame();
	}

	public void startGame()
	{
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
		Camera.main.GetComponent<Cameraman>().setGoal(CameramanTarget.Main);
	}

	public void addEnemies()
	{
		enemyStats.Shuffle();
		foreach (EnemyStat stat in enemyStats)
		{
			GameObject go = (GameObject) Instantiate(enemyPrefab, enemyPlaceholder, Quaternion.identity);
			Debug.Assert(go != null);
			go.name = stat.name;

			EnemyView view = go.GetComponent<EnemyView>();
			Debug.Assert(view != null);
			view.setGarment(stat.garment);

			EnemyBehavior behavior = go.GetComponent<EnemyBehavior>();
			Debug.Assert(behavior != null);
			behavior.armed = stat.armed;
			view.armed = stat.armed;
			behavior.speedModifier = stat.speed;

			// Move placeholder to next position
			Vector3 pos = enemyPlaceholder;
			pos.x += enemyPlacementOffset;
			enemyPlaceholder = pos;
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