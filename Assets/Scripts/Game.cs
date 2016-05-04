using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spewnity;
using GameExtensions.Internal;

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
	private ActionQueue script;
	private Vector3 trackTarget;

	public void Awake()
	{
		enemyPlaceholder = transform.Find("EnemyPlaceholder").position;
		cameraman = Camera.main.GetComponent<Cameraman>();
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		reset = transform.Find("/Game/Reset");
		script = gameObject.AddComponent<ActionQueue>();
		trackTarget = GameObject.Find("/Game/TrackTarget").transform.position;
	}

	public void skipIntro()
	{
		GameObject titling = GameObject.Find("/Titling");
		if(titling != null) titling.SetActive(false);

		script.Clear();
		CancelInvoke();
		SoundManager.instance.Stop("theme");
		StopAllCoroutines();
		cameraman.cutTo(null, 1.25f);

		removeEnemies();
		addEnemies();
		initGame();
	}

	public void Start()
	{
		EnemyBehavior.setRunMode(EnemyRunMode.RunScript);
		CanvasRenderer text1 = GameObject.Find("/Titling/Text1").GetComponent<CanvasRenderer>();
		CanvasRenderer text2 = GameObject.Find("/Titling/Text2").GetComponent<CanvasRenderer>();
		CanvasRenderer text3 = GameObject.Find("/Titling/Text3").GetComponent<CanvasRenderer>();
		Image fadeIn = GameObject.Find("/Titling/FadeIn").GetComponent<Image>();
		Button button = GameObject.Find("/Titling/Button").GetComponent<Button>();
		CanvasGroup buttonCanvas = GameObject.Find("/Titling/Button").GetComponent<CanvasGroup>();

		text1.SetAlpha(0f);
		text2.SetAlpha(0f);
		text3.SetAlpha(0f);
		button.interactable = false;
		buttonCanvas.alpha = 0f;

		cameraman.disableTracking();
		playerController.enabled = false;
		SoundManager.instance.Play("theme");

		script
			.Delay(1.8f)
			.Add(() => StartCoroutine(fadeIn.color.LerpColor(Color.clear, 2.0f, (c) => fadeIn.color = c, null, null)))
			.Add(() => cameraman.dollyTo(8f, null, 1.25f))
			.Delay(2f)
			.FadeAlpha(this, text1)
			.Delay(2f)
			.FadeAlpha(this, text2)
			.Delay(2f)
			.FadeAlpha(this, text3)
			.Delay(2f)
			.FadeAlpha(this, buttonCanvas)
			.Add(() => GameObject.Destroy(fadeIn.gameObject))
			.Add(() => button.interactable = true)
			.Run();
	}

	// Zips through all steps of the Start script
	public void skipTitling()
	{
		if(GameObject.Find("/Titling") == null)
			return;

		StopAllCoroutines();
		script.Cancel();
		script.Clear();
		cameraman.cutTo(null, 1.25f);
		GameObject.Find("/Titling/Text1").GetComponent<CanvasRenderer>().SetAlpha(1f);
		GameObject.Find("/Titling/Text2").GetComponent<CanvasRenderer>().SetAlpha(1f);
		GameObject.Find("/Titling/Text3").GetComponent<CanvasRenderer>().SetAlpha(1f);
		GameObject buttonGO = GameObject.Find("/Titling/Button");
		buttonGO.GetComponent<CanvasGroup>().alpha = 1f;
		buttonGO.GetComponent<Button>().interactable = true;
		GameObject.Destroy(GameObject.Find("/Titling/FadeIn"));
	}

	public void startGame()
	{	
		Button button = GameObject.Find("/Titling/Button").GetComponent<Button>();
		if(!button.interactable) return;
		
		button.interactable = false;
		CanvasGroup titling = GameObject.Find("/Titling").GetComponent<CanvasGroup>();
		StartCoroutine(titling.alpha.LerpFloat(0f, 2f, (float a) => titling.alpha = a, null, startIntro));
	}

	public void startIntro()
	{
		GameObject.Destroy(GameObject.Find("/Titling"));

		addEnemies();

		GameObject macReadyGO = (GameObject) GameObject.Find("MacReady");
		EnemyView macReadyView = macReadyGO.GetComponent<EnemyView>();
		macReadyView.setFacing(true);
		EnemyBehavior macReadyBehavior = macReadyGO.GetComponent<EnemyBehavior>();

		GameObject benningsGO = (GameObject) GameObject.Find("Bennings");
		HunterView benningsView = (HunterView) benningsGO.GetComponent<HunterView>();
		benningsView.setDisguise(false);
		benningsView.accelerate(-4.5f);

		HunterView playerView = (HunterView) playerController.gameObject.GetComponent<HunterView>();

		script
			.Add(() => macReadyView.fireGun())
			.Add(() => cameraman.dollyTo(6.5f, trackTarget.x, null, null, null))
			.Delay(2.0f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.5f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.15f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.10f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.75f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.25f)
			.Add(() => macReadyView.fireGun())
			.Delay(0.75f)
			.Add(() => macReadyView.fireGun())
			.Add(() => macReadyView.setMovement(-1.5f))
			.Delay(1f)
			.Add(() => macReadyView.fireGun())
			.Delay(.2f)
			.Add(() => macReadyView.fireGun())
			.Delay(.3f)
			.Add(() => macReadyView.fireGun(true))
			.Delay(.2f)
			.Add(() => benningsView.die())
			.Add(() => benningsView.halt())
			.Delay(2f)
			.Add(() => macReadyView.setMovement(0))
			.Delay(3f)
			.EnemySpeak("macready1", macReadyBehavior)
			.Add(() => cameraman.trackTarget(macReadyGO.transform, null, Vector2.zero))
			.Add(() => macReadyView.setMovement(1.5f))
			.Delay(4f)
			.EnemySpeak("macready2", macReadyBehavior)
			.Delay(4.2f)
			.Add(() => macReadyView.halt())
			.Add(() => macReadyView.setFacing(true))
			.EnemySpeak("macready3", macReadyBehavior)
			.Add(() => playerView.setMouth(MouthType.Smile))
			.Delay(3f)
			.Add(() => macReadyView.setFacing(false))
			.EnemySpeak("macready4", macReadyBehavior)
			.Add(() => playerView.setMouth(MouthType.Closed))
			.Delay(2f)
			.Add(initGame)
			.Run();
	}

	public void initGame()
	{
		playerController.enabled = true;
		cameraman.trackTarget(playerController.gameObject.transform, null, new Vector2(1.5f, 0));
		EnemyBehavior.setRunMode(EnemyRunMode.StopScript);
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
		EnemyStat[] shuffleStats = enemyStats.ToArray().Shuffle();
		enemyStats.Clear();
		foreach(EnemyStat enemy in shuffleStats) enemyStats.Add(enemy);
		enemyStats.Add((EnemyStat) macReady);
			
		// Place all enemies
		Vector3 pos = enemyPlaceholder;
		foreach(EnemyStat stat in enemyStats)
		{
			pos.x += enemyPlacementOffset;

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
		}

		// Move MacReady to start of queue
		Transform macT = ((GameObject) GameObject.Find("MacReady")).transform;
		pos = macT.position;
		pos.x = enemyPlaceholder.x;
		macT.position = pos;
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
	

namespace GameExtensions.Internal
{
	static class GameExtensions
	{
		internal static ActionQueue FadeAlpha(this ActionQueue aq, MonoBehaviour context, CanvasRenderer cr)
		{
			aq.Add(() => context.StartCoroutine(0f.LerpFloat(1f, 1f, (v) => cr.SetAlpha(v))));
			return aq;
		}

		internal static ActionQueue FadeAlpha(this ActionQueue aq, MonoBehaviour context, CanvasGroup grp)
		{
			aq.Add(() => context.StartCoroutine(0f.LerpFloat(1f, 1f, (v) => grp.alpha = v)));
			return aq;
		}

		internal static ActionQueue EnemySpeak(this ActionQueue aq, string soundName, EnemyBehavior eb)
		{
			AudioSource src = SoundManager.instance.GetSource("theme");
			float initVol = src.volume;
			aq.Add(() => src.volume *= 0.4f)
				.Add(() => eb.setState(eb.talk))
				.PlaySound(soundName)
				.Add(() => eb.setState(eb.quiet))
				.Add(() => src.volume = initVol);
			return aq;
		}
	}
}