using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	
	[Range(5.0f, 10.0f)]
	[SerializeField]
	private float m_AccelerationMultiplier = 10.0f;

	[Range(1.0f, 5.0f)]
	[SerializeField]
	private float m_RotationSpeed = 4.0f;

	[Range(3.0f, 10.0f)]
	[SerializeField]
	private float m_BulletSpeed = 10.0f;

	[Range(1.0f, 4.0f)]
	[SerializeField]
	private float m_RateOfFire = 1.0f;

	[SerializeField]
	private Booleet m_BulletPrefab;

	[SerializeField]
	private GameObject m_GGText;

	[SerializeField]
	private GameObject m_HighScoreForm;

	[SerializeField]
	private Text m_ScoreText;

	[SerializeField]
	private GameObject[] m_LifeIcons;

	[SerializeField]
	private GameObject m_ScoreBoard;

	[SerializeField]
	private Text m_NameInput;

	[SerializeField]
	private SpriteRenderer m_EngineEffect;

	private List<Booleet> m_Bullets;
    private Vector3 m_Velocity;
    private Vector3 m_Acceleration;
	private SpriteRenderer m_Ship;
	private float m_ShotTime = 0.0f;
	private float m_InvulnTime = 4.0f;
	private int m_Lives = 3;
	private int m_Score = 0;
	private bool m_CanShoot = true;
	private bool m_isWrappingX, m_isWrappingY;
	private bool m_isInvuln = true;
	private ScoreManager m_ScoreManager;
	private AsteroidController m_AC;

	// Use this for initialization
	void Start () {
		m_AC = FindObjectOfType<AsteroidController>() as AsteroidController;
		m_Ship = GetComponentInChildren<SpriteRenderer>() as SpriteRenderer;
		m_Bullets = new List<Booleet>();

		var cam = Camera.main;

		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));

		StartCoroutine(EngineFlicker());
		StartCoroutine(Flicker());
		m_ScoreManager = ScoreManager._Instance;
	}
	
	// Update is called once per frame
	void Update () {
		ScreenWrap();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
			Rotate(m_RotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
			Rotate(-m_RotationSpeed);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
			m_Acceleration = this.gameObject.transform.up * m_AccelerationMultiplier;
			m_EngineEffect.gameObject.SetActive(true);
        }
		else
		{
			m_Acceleration = Vector3.zero;
			m_EngineEffect.gameObject.SetActive(false);
		}

		if (Input.GetKey(KeyCode.Space) && m_CanShoot)
		{
			Shoot();
		}

		if (!m_CanShoot && m_ShotTime < 1.0f / m_RateOfFire)
		{
			m_ShotTime += Time.deltaTime;
		}
		else if (!m_CanShoot && m_ShotTime >= 1.0f / m_RateOfFire)
		{
			m_CanShoot = true;
			m_ShotTime = 0.0f;
		}
		
        IntegrateVelocity();
		KillBullets();

		if (m_isInvuln && m_InvulnTime > 0.0f)
		{
			m_InvulnTime -= Time.deltaTime;
		}
		else if (m_isInvuln && m_InvulnTime < 0.0f)
		{
			m_InvulnTime = 4.0f;
			m_isInvuln = false;
			m_Ship.color = Color.white;
			StopCoroutine("Flicker");
		}
		m_ScoreText.text = m_Score.ToString();

	}

	private void LoseLife()
	{
		m_LifeIcons[m_Lives].SetActive(false);
	}

	private void Shoot()
	{
		Booleet temp = Instantiate( m_BulletPrefab ) as Booleet;
		temp.gameObject.transform.position = this.gameObject.transform.position + (this.gameObject.transform.up * 0.7f);
		temp.Shoot(this.gameObject.transform.up * m_BulletSpeed);
		m_Bullets.Add(temp);
		m_CanShoot = false;
	}

	public void Die()
	{	
		if (m_isInvuln)
			return;

		Debug.Log(m_Lives);
		m_Lives--;
		LoseLife();
		if (m_Lives >= 1) {
			this.gameObject.transform.position = Vector3.zero;
			m_Velocity = Vector3.zero;
			m_isInvuln = true;
			StartCoroutine("Flicker");
		}
		else
		{	
			StartCoroutine(GameEnd());
		}
	}

	public void AddScore(int _arg)
	{
		m_Score += _arg;
	}

	private void ScreenWrap()
	{
		var cam = Camera.main;
		var viewportPosition = cam.WorldToViewportPoint(this.gameObject.transform.position);
		var newPosition = this.gameObject.transform.position;

		//Is the asteroid inside the camera's viewport?
		if ((viewportPosition.x < 1 && viewportPosition.x > 0.0f) && (viewportPosition.y < 1 && viewportPosition.y > 0.0f))
		{
			m_isWrappingX = false;
			m_isWrappingY = false;

			return;
		}

		if (m_isWrappingX && m_isWrappingY)
		{
			return;
		}

		if (!m_isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
		{
			newPosition.x = -newPosition.x;
			m_isWrappingX = true;
		}

		if (!m_isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0))
		{
			newPosition.y = -newPosition.y;
			m_isWrappingY = true;
		}

		this.transform.position = newPosition;
	}

    private void IntegrateVelocity()
    {
        m_Velocity = (m_Velocity + m_Acceleration * Time.deltaTime) * 0.99f;
		
        this.gameObject.transform.position = this.gameObject.transform.position + m_Velocity * Time.deltaTime;
    }
	
    private void Rotate(float _dir)
    {
        this.gameObject.transform.Rotate(0.0f, 0.0f, _dir);
    }

	private void KillBullets()
	{
		List<Booleet> temp = new List<Booleet>();
		foreach (Booleet bullet in m_Bullets)
		{
			if (bullet.Distance > 20.0f)
			{
				Destroy(bullet.gameObject);
			}
			else {
				temp.Add(bullet);
			}
		}
		m_Bullets = temp;
	}

	public void RegisterScore()
	{
		m_ScoreManager.AddScore(m_Score, m_NameInput.text);
		m_HighScoreForm.SetActive(false);
		m_ScoreBoard.SetActive(true);
	}

	public void KillBullet(Booleet _bullet)
	{
		List<Booleet> temp = new List<Booleet>();
		foreach (Booleet bullet in m_Bullets)
		{
			if (_bullet.GetInstanceID() == bullet.GetInstanceID())
			{
				Destroy(bullet.gameObject);
			}
			else
			{
				temp.Add(bullet);
			}
		}
		m_Bullets = temp;
	}

	private IEnumerator Flicker()
	{
		Color shipCol = m_Ship.color;

		while (m_isInvuln)
		{
			if (m_Ship.color.a == 0.0f)
			{
				shipCol.a = 1.0f;
			}
			else
			{
				shipCol.a = 0.0f;
			}

			m_Ship.color = shipCol;

			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator GameEnd()
	{
		m_AC.FadeRocks();
	
		m_Ship.gameObject.SetActive(false);
		m_GGText.gameObject.SetActive(true);

		yield return new WaitForSeconds(2.0f);

		m_GGText.gameObject.SetActive(false);
		m_HighScoreForm.gameObject.SetActive(true);
	}

	private IEnumerator EngineFlicker()
	{
		Color[] engineColors = {Color.white, Color.black};
		float fl = 0.0f;
		int col = 0;
		while (true)
		{
			fl += Time.deltaTime;
			if (fl > 0.2f)
			{
				col = col == 0 ? 1 : 0;
				fl = 0.0f;
			}
			if (m_EngineEffect.gameObject.activeSelf)
				m_EngineEffect.color = engineColors[col];

			yield return null;
		}
	}

	public void NewGame()
	{
		Application.LoadLevel("MainMenu");
	}
}
