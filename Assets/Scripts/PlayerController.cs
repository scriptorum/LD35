using UnityEngine;
using System.Collections;
using Spewnity;

public class PlayerController : MonoBehaviour
{
	private HunterView view;
	private Hunter hunter;

	public void Awake()
	{
		view = gameObject.GetComponent<HunterView>();
		hunter = gameObject.GetComponent<Hunter>();
	}

	public void onMovement(InputEvent evt)
	{
		if(!enabled)
			return;
		
		// No motion - slow down player gradually
		if(evt.axis.x == 0)
			hunter.isMoving = view.decelerate();

		// Speed up (or hard brake) - player controls velocity change
		else hunter.isMoving = view.accelerate(evt.axis.x);
	}
}
