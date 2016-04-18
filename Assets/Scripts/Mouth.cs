using UnityEngine;
using System.Collections;

public class Mouth : MonoBehaviour
{
	public Sprite smile;
	public Sprite closed;
	public Sprite talk;
	public Sprite yell;
	private SpriteRenderer sr;

	void Awake()
	{	
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	public void setMouth(MouthType type)
	{
		Sprite sprite = null;
		switch(type)
		{
			case MouthType.None:
				sprite = null;
				break;
			case MouthType.Closed:
				sprite = closed;
				break;
			case MouthType.Talk:
				sprite = talk;
				break;
			case MouthType.Yell:
				sprite = yell;
				break;
			case MouthType.Smile:
				sprite = smile;
				break;
			default:
				throw new UnityException("WTF");
		}

		Debug.Assert(sprite != null);
		sr.sprite = sprite;
	}

	public void setFlipped(bool flipped)
	{
		sr.flipX = flipped;
	}

	public void setLayer(string sortingLayer, int sortingOrder)
	{
		sr.sortingLayerName = sortingLayer;
		sr.sortingOrder = sortingOrder;
	}
}

public enum MouthType
{
	None,
	Closed,
	Talk,
	Yell,
	Smile
}
