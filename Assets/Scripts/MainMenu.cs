using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	private Asteroid m_AsteroidPrefab;

	private List<Asteroid> m_Asteroids;

	void Start()
	{
		m_Asteroids = new List<Asteroid>();

		Asteroid temp;
		float rotation;
		Vector3 pos;
		Transform trans;

		for (int i = 0; i < 10; i++)
		{
			temp = Instantiate(m_AsteroidPrefab) as Asteroid;
			rotation = Random.Range(0.0f, 360.0f);
			trans = this.gameObject.transform;
			trans.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);
			pos = trans.up * 5.0f;
			temp.Spawn(Random.Range(-1.0f, 1.0f), pos, new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(1.0f, -1.0f)), Random.Range(0, 3));
			m_Asteroids.Add(temp);
		}
	}

	public void PlayClicked()
	{
		Application.LoadLevel("Game");
	}
}
