using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidController : MonoBehaviour {
	
	[SerializeField]
	private Asteroid m_AsteroidPrefab;

	[SerializeField]
	private int m_MaxAsteroids = 4;

	[SerializeField]
	private PlayerController m_PC;

	private List<Asteroid> m_Asteroids;
	private int m_MaxRocks = 30;

	// Use this for initialization
	void Start () {
		m_Asteroids = new List<Asteroid>();
	}
	
	// Update is called once per frame
	void Update () {
		int bigRocks = 0;

		foreach (Asteroid ast in m_Asteroids)
		{
			if (ast.m_Splits == 0)
			{
				bigRocks++;
			}
		}

		if (bigRocks < m_MaxAsteroids && m_Asteroids.Count < m_MaxRocks)
		{
			SpawnAsteroid();
		}
	}

	private void SpawnAsteroid()
	{
		int size = Random.Range(0, 3);
		Asteroid temp = Instantiate(m_AsteroidPrefab) as Asteroid;
		float rotation = Random.Range(0.0f, 360.0f);
		Transform trans = this.gameObject.transform;
		trans.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);
		Vector3 pos = trans.up * 8.0f;
		temp.Spawn(Random.Range(-1.0f, 1.0f), pos, new Vector3(Random.Range(-1.0f, 1.0f) * size,Random.Range(-1.0f, 1.0f)), size, this);
		m_Asteroids.Add(temp);
	}

	public void AsteroidHit(Asteroid _hit)
	{		
		List<Asteroid> temp = new List<Asteroid>();
		foreach (Asteroid asteroid in m_Asteroids)
		{
			if (asteroid.GetInstanceID() == _hit.GetInstanceID())
			{
				if (asteroid.m_Splits < 2)
				{
					Asteroid ast = Instantiate(m_AsteroidPrefab) as Asteroid;
					float rotation = Random.Range(0.0f, 360.0f);
					Transform trans = this.gameObject.transform;
					trans.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);
					Vector3 pos = asteroid.transform.position;
					ast.Spawn(Random.Range(-1.0f, 1.0f), pos, Quaternion.AngleAxis(45, Vector3.Cross(asteroid.Velocity, Vector3.up)) * asteroid.Velocity, asteroid.m_Splits + 1, this);
					temp.Add(ast);

					ast = Instantiate(m_AsteroidPrefab) as Asteroid;
					rotation = Random.Range(0.0f, 360.0f);
					trans = this.gameObject.transform;
					trans.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);
					pos = asteroid.transform.position;
					ast.Spawn(Random.Range(-1.0f, 1.0f), pos, Quaternion.AngleAxis(45, Vector3.Cross(asteroid.Velocity, -Vector3.up)) * asteroid.Velocity, asteroid.m_Splits + 1, this);
					temp.Add(ast);
				}
				if (asteroid.m_Splits == 0)
				{
					m_PC.AddScore(20);
				}
				else if (asteroid.m_Splits == 1)
				{
					m_PC.AddScore(50);
				}
				else
				{
					m_PC.AddScore(100);
				}

				Destroy(asteroid.gameObject);
			}
			else
			{
				temp.Add(asteroid);
			}
		}
		m_Asteroids = temp;
	}

	public void FadeRocks()
	{
		foreach (Asteroid ast in m_Asteroids)
		{
			Color fade = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			(ast.GetComponent<SpriteRenderer>() as SpriteRenderer).color = fade;
		}
	}
}
