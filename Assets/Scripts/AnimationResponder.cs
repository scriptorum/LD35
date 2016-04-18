using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AnimationResponder : MonoBehaviour
{
	public Callback animationEvent;

	public void trigger(AnimationEvent evt)
	{
		animationEvent.Invoke(evt);
	}
}

// Inspector won't list classes with generic parameters. It's a jerk.
[System.Serializable]
public class Callback: UnityEvent<AnimationEvent>
{
}
