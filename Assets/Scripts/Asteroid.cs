using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {
	
	[HideInInspector]
	public int m_Splits;

	private AsteroidController m_AsteroidController;
	private PlayerController m_PlayerController;

	private Vector3 m_Velocity;
	private float m_SpinRate = 1.0f;
	private Transform[] m_Ghosts;

	private float m_ScreenWidth, m_ScreenHeight;
	private bool m_isWrappingX, m_isWrappingY;

	public Vector3 Velocity
	{
		get { return m_Velocity; }
	}

	void Start()
	{
		if (Application.loadedLevelName == "Game")
			m_PlayerController = GameObject.Find("Ship").GetComponent<PlayerController>() as PlayerController;

		var cam = Camera.main;

		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));

		m_ScreenWidth = screenTopRight.x - screenBottomLeft.x;
		m_ScreenHeight = screenTopRight.y - screenBottomLeft.y;
	}

	void Update()
	{
		ScreenWrap();
		this.gameObject.transform.position = this.gameObject.transform.position + m_Velocity * Time.deltaTime;
		this.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), m_SpinRate);
	}

	public void Spawn(float _spin, Vector3 _pos, Vector3 _velocity, int _splits, AsteroidController _cont = null)
	{
		m_SpinRate = _spin;
		this.gameObject.transform.position = _pos;
		m_Velocity = _velocity;
		m_Splits = _splits;
		if (m_Splits == 0)
		{
			this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
		}
		else if (m_Splits == 1)
		{
			this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
		}
		else {
			this.gameObject.transform.localScale = new Vector3(0.125f, 0.125f, 1.0f);
		}
		if (_cont != null)
		{
			m_AsteroidController = _cont;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.name.Contains("Booleet"))
		{
			m_PlayerController.KillBullet(col.gameObject.GetComponent<Booleet>() as Booleet);
			m_AsteroidController.AsteroidHit(this); 
		}
		else if (col.gameObject.name.Contains("Ship"))
		{
			m_PlayerController.Die();
		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.name.Contains("Ship"))
		{
			m_PlayerController.Die();
		}
	}

	void ScreenWrap()
	{		
		Camera cam = Camera.main;
		Vector3 viewportPosition = cam.WorldToViewportPoint(this.gameObject.transform.position);
		Vector3 newPosition = this.gameObject.transform.position;

		//Is the asteroid inside the camera's viewport?
		if ((viewportPosition.x < 1.0 && viewportPosition.x > 0.0f ) && (viewportPosition.y < 1.0f && viewportPosition.y > 0.0f))
		{
			m_isWrappingX = false;
			m_isWrappingY = false;

			return;
		}

		if (m_isWrappingX && m_isWrappingY)
		{
			return;
		}

		if (!m_isWrappingX && (viewportPosition.x > 1.1f || viewportPosition.x < -0.1f))
		{
			newPosition.x = -newPosition.x;
			m_isWrappingX = true;
		}

		if (!m_isWrappingY && (viewportPosition.y > 1.1f || viewportPosition.y < -0.1f))
		{
			newPosition.y = -newPosition.y;
			m_isWrappingY = true;
		}

		this.transform.position = newPosition;
	}
}
