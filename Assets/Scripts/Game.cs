using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spewnity;

public class Game : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public List<EnemyStat> enemyStats;
	public float enemyPlacementOffset;
	private Vector3 enemyPlaceholder;
	private Cameraman cameraman;
	private PlayerController playerController;
	private Transform reset;
	private int scriptStep = 0;

	public void Awake()
	{
		enemyPlaceholder = transform.Find("EnemyPlaceholder").position;
		cameraman = Camera.main.GetComponent<Cameraman>();
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		reset = transform.Find("/Game/Reset");
		scriptStep = 0;
	}

	public void Start()
	{
		cameraman.disableTracking();
		playerController.enabled = false;
		SoundManager.instance.play("theme");
	}

	public void skipIntro()
	{
		GameObject titling = GameObject.Find("/Titling");
		if(titling != null) titling.SetActive(false);
		removeEnemies();
		addEnemies();
		initGame();
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
		cameraman.dollyTo(7f, playerController.transform.position.x, null, null, null);
		Invoke("nextStep", 8f);
	}

	public void nextStep()
	{
		scriptStep++;
		Invoke("speakMacReady", 2f);
	}

	public void speakMacReady()
	{
		if(scriptStep > 4)
		{
			initGame();
			return;
		}

		SoundManager.instance.play("macready" + scriptStep, (snd) => nextStep());
	}

	public void initGame()
	{
		CancelInvoke();
		playerController.enabled = true;
		cameraman.enableTracking();
		EnemyBehavior.runMode = EnemyRunMode.StopScript;
	}

	public void removeEnemies()
	{
		foreach(Transform child in reset)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public void addEnemies()
	{
		// Shuffle enemies
		// Force MacReady to the front
		EnemyStat? macReady = null;
		foreach(EnemyStat enemy in enemyStats)
		{
			if(enemy.name == "MacReady")
			{
				macReady = (EnemyStat) enemy;
				enemyStats.Remove(enemy);
				break;
			}
		}
		Debug.Assert(macReady != null);
		EnemyStat[] shuffleStats = enemyStats.ToArray().Shuffle();;
		enemyStats.Clear();
		enemyStats.Add((EnemyStat) macReady);
		foreach(EnemyStat enemy in shuffleStats)
			enemyStats.Add(enemy);
			
		// Place all enemies
		Vector3 pos = enemyPlaceholder;
		foreach (EnemyStat stat in enemyStats)
		{
			GameObject go = (GameObject) Instantiate(enemyPrefab, pos, Quaternion.identity);
			Debug.Assert(go != null);
			go.name = stat.name;
			go.transform.parent = reset;

			EnemyView view = go.GetComponent<EnemyView>();
			Debug.Assert(view != null);
			view.setGarment(stat.garment);

			EnemyBehavior behavior = go.GetComponent<EnemyBehavior>();
			Debug.Assert(behavior != null);
			behavior.armed = stat.armed;
			view.armed = stat.armed;
			behavior.speedModifier = stat.speed;

			// Move placeholder to next position
			pos.x += enemyPlacementOffset;
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