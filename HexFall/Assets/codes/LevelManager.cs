using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	public int starRate = 10;
	[SerializeField]
	GameObject end;
	[SerializeField]
	Text pointView;
	[SerializeField]
	Text recordView;
	public float fallSpeed = 1;
	public float turnSpeed = 1;
	public AnimationCurve turnAnim;
	public Vector2 iconSize = new Vector2(1.154701f, 2f);
	public float iconXSlide = 1.73f;
	public Vector2Int mapSize = new Vector2Int(10, 10);
	public static bool lockGame = false;
	bool myPause = false;
	Camera cam;
	Vector3 activePos;
	AIcon[] active = new AIcon[3];
	[SerializeField]
	int bombPoint = 20;
	[SerializeField]
	Transform activeIcon;
	float oldActiveAngle;
	bool tuned = false;
	public float valueForTurn = 20;
	public AIcon[][] map;

	[SerializeField]
	private GameObject[] icons;
	public Color[] colors;
	[HideInInspector]
	public float iconSizeRate;
	[HideInInspector]
	public int addPoint = 0;
	[HideInInspector]
	public int multiplyPoint = 1;
	[HideInInspector]
	public int point = 0;
	int pointTarget;

	[SerializeField]
	Particles particle;
	bool taked = false;

	private void Awake()
	{
		if (!PlayerPrefs.HasKey("record"))
			PlayerPrefs.SetInt("record", 0);
		recordView.text = "record:\n" + PlayerPrefs.GetInt("record");
		if (colors.Length < 3)
			Debug.LogError("No less than 3 colors");
		cam = Camera.main;
		pointTarget = bombPoint;
		CreteMap();
	}

	public void End()
	{
		myPause = true;
		lockGame = true;
		end.SetActive(true);
	}

	int getIcon()
	{
		if (point < pointTarget)
			return Random.Range(0, starRate) == 0 ? 1 : 0;
		pointTarget += bombPoint;
		return 2;
	}

	int IconNumberInColon(int x)
	{
		int result = 0;
		for (int y = 0; y < mapSize.y; y++)
			if (map[x][y] != null)
				result++;
		return result;
	}

	AIcon getFinalAIcon(int posX, int posY)
	{
		if (posX < 0 || posX >= mapSize.x || posY < 0 || posY >= mapSize.y)
			return null;
		int index = 0;
		for (int y = 0; y < mapSize.y; y++)
		{
			if (map[posX][y] != null)
			{
				if (index == posY)
					return map[posX][y];
				index++;
			}
		}
		return null;
	}

	bool ColorControl(int x, int colorIndex)
	{
		int add = x % 2;
		int y = IconNumberInColon(x) - 1;

		AIcon[] aronuds = new AIcon[5];
		aronuds[0] = getFinalAIcon(x + 1, y + add);
		aronuds[1] = getFinalAIcon(x + 1, y + add - 1);
		aronuds[2] = getFinalAIcon(x, y - 1);
		aronuds[3] = getFinalAIcon(x - 1, y + add - 1);
		aronuds[4] = getFinalAIcon(x - 1, y + add);

		for (int i = 0; i < 4; i++)
		{
			if (aronuds[i] == null || aronuds[i + 1] == null)
				continue;
			int colorA = aronuds[i].colorIndex;
			int colorB = aronuds[i + 1].colorIndex;
			if (colorA == colorB && colorB == colorIndex)
				return false;
		}
		return true;
	}

	public int getColor(int x)
	{
		int result = Random.Range(0, colors.Length);
		while (!ColorControl(x, result))
		{
			result = (result + 1) % colors.Length;
		}
		return result;
	}

	float VectorToAngle(Vector2 vector)
	{
		return Mathf.Rad2Deg * -(float)System.Math.Atan2(vector.x, vector.y);
	}

	public void BeginDrag(Vector2 pos)
	{
		if (active[0] == null)
			return;
		oldActiveAngle = VectorToAngle(pos - (Vector2)activePos);
		tuned = false;
	}

	bool isColorsSame(AIcon a, AIcon b, AIcon c)
	{
		int aV = a.colorIndex;
		int bV = b.colorIndex;
		int cV = c.colorIndex;
		return aV == bV && bV == cV;
	}

	void LookCorrectA(int x, int y, HashSet<AIcon> selected)
	{
		int add = x % 2;
		if (x + 1 != mapSize.x && y + 1 != mapSize.y)
		{
			AIcon a = map[x][y];
			AIcon b = map[x + 1][y + add];
			AIcon c = map[x][y + 1];
			if (isColorsSame(a, b, c))
			{
				selected.Add(a);
				selected.Add(b);
				selected.Add(c);
			}
		}
		if (x != 0 && y + 1 != mapSize.y)
		{
			AIcon a = map[x][y];
			AIcon b = map[x - 1][y + add];
			AIcon c = map[x][y + 1];
			if (isColorsSame(a, b, c))
			{
				selected.Add(a);
				selected.Add(b);
				selected.Add(c);
			}
		}
	}

	void TakeIcons(HashSet<AIcon> list)
	{
		if (list.Count > 0 && active[0] != null)
		{
			activeIcon.position = activeIcon.position + Vector3.forward;
			for (int i = 0; i < active.Length; i++)
				active[i].transform.SetParent(transform);
			activeIcon.position = Vector3.one * 9999;
			active[0] = null;
		}
		foreach (AIcon icon in list)
		{
			taked = true;
			particle.Play(icon.transform.position, colors[icon.colorIndex]);
			icon.Take();
			Destroy(icon.gameObject);
			map[icon.pos.x][icon.pos.y] = null;
		}
	}

	bool ControlMap()
	{
		HashSet<AIcon> selected = new HashSet<AIcon>();
		for (int x = 0; x < mapSize.x; x++)
		{
			for (int y = 0; y < mapSize.y; y++)
			{
				LookCorrectA(x, y, selected);
			}
		}
		TakeIcons(selected);
		point += addPoint * multiplyPoint;
		pointView.text = point.ToString();
		if (point > PlayerPrefs.GetInt("record"))
		{
			PlayerPrefs.SetInt("record", point);
			recordView.text = "record:\n" + point;
		}
		addPoint = 0;
		multiplyPoint = 1;
		return selected.Count > 0;
	}

	void SendActions()
	{
		for (int x = 0; x < mapSize.x; x++)
		{
			for (int y = 0; y < mapSize.y; y++)
			{
				if (map[x][y] != null)
					map[x][y].Action();
			}
		}
	}

	IEnumerator searchIcons(int dir)
	{
		for (int i = 0; i < 3; i++)
		{
			Vector2Int a = active[1 + dir].pos;
			Vector2Int b = active[1].pos;
			Vector2Int c = active[1 - dir].pos;

			AIcon tmp = map[a.x][a.y];
			map[a.x][a.y] = map[b.x][b.y];
			map[b.x][b.y] = map[c.x][c.y];
			map[c.x][c.y] = tmp;

			map[a.x][a.y].pos = a;
			map[b.x][b.y].pos = b;
			map[c.x][c.y].pos = c;

			float timer = 0;
			Vector3 old = activeIcon.eulerAngles;
			Vector3 target = activeIcon.eulerAngles + Vector3.forward * -120 * dir;
			while (timer < 1)
			{
				activeIcon.eulerAngles = Vector3.Lerp(old, target, turnAnim.Evaluate(timer));
				yield return new WaitForSeconds(Time.deltaTime);
				timer += Time.deltaTime * turnSpeed * 6;
			}
			activeIcon.eulerAngles = target;
			if (ControlMap())
				break;
		}
		myPause = false;
	}

	public void Draging(Vector2 pos)
	{
		if (tuned || active[0] == null)
			return;
		float angle = Mathf.DeltaAngle(VectorToAngle(pos - (Vector2)activePos), oldActiveAngle);
		if (Mathf.Abs(angle) > valueForTurn)
		{
			tuned = true;
			myPause = true;
			lockGame = true;
			StartCoroutine(searchIcons(angle < 0 ? -1 : 1));
		}
	}

	void addIcon(int x)
	{
		int y = mapSize.y - 1;
		Transform tmp = Instantiate(icons[getIcon()], transform).transform;
		map[x][y] = tmp.GetComponent<AIcon>();
		map[x][y].setIndexPos(new Vector2Int(x, y));
		tmp.localScale = Vector3.one * iconSizeRate;
		float xPos = x * iconSizeRate * iconXSlide + iconSizeRate * iconSize.x;
		tmp.localPosition = new Vector3(xPos, 2f / cam.aspect, 0);
	}

	void CreteMap()
	{
		transform.position = new Vector3(-1, (float)-mapSize.y / mapSize.x, 0);
		float addX = 2 - iconXSlide;
		iconSizeRate = 1f / ((mapSize.x * iconXSlide * 0.5f) + addX);
		activeIcon.localScale = Vector3.one * iconSizeRate;
		map = new AIcon[mapSize.x][];
		for (int x = 0; x < mapSize.x; x++)
			map[x] = new AIcon[mapSize.y];
		lockGame = true;
	}

	void setIcon(Vector3 pos, float angle)
	{
		activeIcon.position = pos;
		activeIcon.eulerAngles = Vector3.forward * angle;
		activePos = cam.WorldToScreenPoint(pos);
	}

	void LookNear(int x, int y, Vector3 screenPos, ref float minDistance)
	{
		int add = x % 2;
		if (x + 1 != mapSize.x && y + 1 != mapSize.y)
		{
			Vector3 pos = (map[x][y].transform.position + map[x + 1][y + add].transform.position + map[x][y + 1].transform.position) * 0.333333f;
			float distance = Vector3.SqrMagnitude(cam.WorldToScreenPoint(pos) - screenPos);
			if (minDistance > distance)
			{
				minDistance = distance;
				active[0] = map[x][y];
				active[1] = map[x][y + 1];
				active[2] = map[x + 1][y + add];
				setIcon(pos, 180);
			}
		}
		if (x != 0 && y + 1 != mapSize.y)
		{
			Vector3 pos = (map[x][y].transform.position + map[x - 1][y + add].transform.position + map[x][y + 1].transform.position) * 0.333333f;
			float distance = Vector3.SqrMagnitude(cam.WorldToScreenPoint(pos) - screenPos);
			if (minDistance > distance)
			{
				minDistance = distance;
				active[0] = map[x][y];
				active[1] = map[x - 1][y + add];
				active[2] = map[x][y + 1];
				setIcon(pos, 0);
			}
		}
	}

	public void SelectIcon(Vector3 screenPos)
	{
		if (active[0] != null)
		{
			activeIcon.position = activeIcon.position + Vector3.forward;
			for (int i = 0; i < active.Length; i++)
				active[i].transform.SetParent(transform);
		}
		float minDistance = float.MaxValue;
		for (int x = 0; x < mapSize.x; x++)
		{
			for (int y = 0; y < mapSize.y; y++)
			{
				LookNear(x, y, screenPos, ref minDistance);
			}
		}
		for (int i = 0; i < active.Length; i++)
			active[i].transform.SetParent(activeIcon);
		activeIcon.position = activeIcon.position + Vector3.back;
	}

	bool allInPlace()
	{
		for (int x = 0; x < mapSize.x; x++)
			for (int y = 0; y < mapSize.y; y++)
				if (map[x][y] == null)
					return false;
		return true;
	}

	void setupMap()
	{
		for (int x = 0; x < mapSize.x; x++)
		{
			if (map[x][mapSize.y - 1] == null)
				addIcon(x);
		}
	}

	private void Update()
	{
		if (lockGame)
			setupMap();

		if (lockGame && !myPause && allInPlace() && !ControlMap())
		{
			lockGame = false;
			if (taked)
			{
				SendActions();
				taked = false;
			}
		}
	}

}
