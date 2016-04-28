/*
 *  REFACTOR GOALS:
 *   - Zoom bouncing
 *   - Perspective camera tricks: tilt/pan/dolly-counter-zoom/aim-at
 *   - handheld camera, handheld zoom, rotate shake, edge bounce
 *   - Support storing of waypoints? Support queuing of camera moves? Static, reusable, named queues in inspector?
 *   - Support screw zoom
 *   - Support only tracking targets that are off-screen or within certain distance of screen edge.
 *   - Tooltips
 */

using UnityEngine;
using System.Collections;
using Spewnity;

public class Cameraman : MonoBehaviour
{
	public float trackingSpeed = 5.0f;
	public Transform trackingTarget;
	public Vector2 leading = Vector2.zero;
	public bool isTracking = false;
	public Transform cameraHarness;
	public Vector3 shakeAmount = Vector3.zero;
	public float shakeTimer;
	public bool doNotTrackX = false;
	public bool doNotTrackY = false;
	[HideInInspector]
	public Vector3 velocity = Vector3.zero;

	void Awake()
	{
		checkHarness();
	}

	public void trackTarget(Transform target, float? speed = null, Vector2? leading = null)
	{
		this.trackingTarget = target;
		this.leading = (leading == null ? Vector2.zero : (Vector2) leading);
		if(speed != null) trackingSpeed = (float) speed;
		enableTracking();
	}

	public void enableTracking()
	{
		this.isTracking = true;
	}

	public void disableTracking()
	{
		this.isTracking = false;
	}

	public void cutTo(float? x = null, float? y = null, float? z = null)
	{
		disableTracking();
		Vector3 pos = transform.position;
		if(x != null) pos.x = (float) x;
		if(y != null) pos.y = (float) y;
		if(z != null) pos.z = (float) z;
		transform.position = pos;
	}

	// TODO this will conflict with zooming
	public void dollyTo(float duration, float? x = null, float?y = null, AnimationCurve curve = null, System.Action<Transform> onComplete = null)
	{
		disableTracking();
		Vector3 target = new Vector3((x == null ? transform.position.x : (float) x), 
			                 (y == null ? transform.position.y : (float) y), 
			                 transform.position.z);
		StartCoroutine(transform.LerpPosition(target, duration, curve, onComplete));
		Debug.Log("Lerping from " + transform.position + " to " + target + " over " + duration + " sec");
	}

	// TODO this will conflict with dollying
	public void zoomTo(float duration, float z, AnimationCurve curve = null, System.Action<Transform> onComplete = null)
	{
		disableTracking();
		Vector3 target = new Vector3(transform.position.x, transform.position.y, z);
		StartCoroutine(transform.LerpPosition(target, duration, curve, onComplete));
	}

	public void shake(float duration, Vector3? amount = null)
	{
		// The Camera should have a parent game object at 0,0,0 to act as a harness.
		// If one is not supplied, the camera itself will shake, which can conflict
		// with other camera operations.
		cameraHarness = checkHarness();
		shakeTimer = duration;
		if(amount != null) shakeAmount = (Vector3) amount;
	}

	void LateUpdate()
	{		
		if(isTracking)
		{
			if(trackingTarget == null) return;

			Vector3 pos = transform.position;		
			if(!doNotTrackX) pos.x = trackingTarget.transform.position.x + leading.x;
			if(!doNotTrackY) pos.y = trackingTarget.transform.position.y + leading.y;
			transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, trackingSpeed);
		}

		if(shakeTimer > 0)
		{
			shakeTimer -= Time.deltaTime;
			Vector3 pos;
			if(shakeTimer <= 0)
			{
				pos = Vector3.zero;
				shakeTimer = 0;
			}
			else pos = new Vector3(Random.Range(-shakeAmount.x, shakeAmount.x), 
					Random.Range(-shakeAmount.y, shakeAmount.y), 
					Random.Range(-shakeAmount.z, shakeAmount.z));
			cameraHarness.position = pos;
		}
	}

	private Transform checkHarness()
	{
		if(cameraHarness == null)
			return transform;

		if(cameraHarness.position.magnitude != 0)
			Debug.Log("Warning: Camera harness should have a 0,0,0 position.");

		return cameraHarness;
	}
}