using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIcon : MonoBehaviour
{
	[HideInInspector]
	public Vector2Int pos = Vector2Int.zero;
	protected LevelManager lm;
	float targetPos;
	[HideInInspector]
	public int colorIndex = 0;
	Material selfMaterial;
	float speed = 0;
	bool goDown = true;
	public abstract void Action();

	public abstract void Take();

	float getTargetPos()
	{
		float add = (pos.x % 2) * (lm.iconSize.y * 0.5f * lm.iconSizeRate);
		return pos.y * lm.iconSizeRate * lm.iconSize.y + add;
	}
	public void setIndexPos(Vector2Int pos)
	{
		this.pos = pos;
		colorIndex = lm.getColor(pos.x);
		selfMaterial.color = lm.colors[colorIndex] * selfMaterial.color;
		targetPos = getTargetPos();
	}

	void Awake()
	{
		lm = transform.parent.GetComponent<LevelManager>();
		selfMaterial = new Material(GetComponent<MeshRenderer>().material);
		GetComponent<MeshRenderer>().material = selfMaterial;
	}

	bool placeControl()
	{
		bool result = pos.y != 0 && lm.map[pos.x][pos.y - 1] == null;
		if (result)
		{
			LevelManager.lockGame = true;
			pos.y--;
			targetPos = getTargetPos();
			goDown = true;
		}
		return result;
	}

	void goingDown()
	{
		transform.localPosition = transform.localPosition + Vector3.down * speed * Time.deltaTime;
		speed += Time.deltaTime * Mathf.Pow(lm.mapSize.y, 2) * lm.fallSpeed * 0.5f;
		;
		if (transform.localPosition.y <= targetPos)
		{
			goDown = false;
			lm.map[pos.x][pos.y] = this;
			if (pos.y < lm.mapSize.y - 1)
				lm.map[pos.x][pos.y + 1] = null;
			transform.localPosition = new Vector3(transform.localPosition.x, targetPos, transform.localPosition.z);
			if (!placeControl())
				speed = 0;
		}
	}

	void Update()
	{
		if (goDown)
			goingDown();
		else
			placeControl();
	}
}
