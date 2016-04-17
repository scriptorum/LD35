using UnityEngine;
using System.Collections;

public class Mouth : MonoBehaviour
{
	public Sprite mouthSmile;
	public Sprite mouthClosed;
	public Sprite mouthTalk;
	public Sprite mouthYell;
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
			case MouthType.Closed:
				sprite = mouthClosed;
				break;
			case MouthType.Talk:
				sprite = mouthTalk;
				break;
			case MouthType.Yell:
				sprite = mouthYell;
				break;
			case MouthType.Smile:
				sprite = mouthSmile;
				break;
			default:
				throw new UnityException("WTF");
		}

		sr.sprite = sprite;
	}

	public void setFlipped(bool flipped)
	{
		sr.flipX = flipped;
	}

	public void setSortingOrder(int order)
	{
		sr.sortingOrder = order;
	}
}

public enum MouthType
{
	Closed,
	Talk,
	Yell,
	Smile
}
