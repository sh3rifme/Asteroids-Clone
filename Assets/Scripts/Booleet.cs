using UnityEngine;
using System.Collections;

public class Booleet : MonoBehaviour {
	
	private Vector3 m_Velocity;
	private Vector3 m_StartPos;
	private float m_LifeTime = 3.0f;
	public float Distance
	{
		get { return Vector3.Distance(m_StartPos, this.gameObject.transform.position); }
	}

	public void Shoot(Vector3 _arg)
	{	
		m_StartPos = this.gameObject.transform.position;
		m_Velocity = _arg;
	}

	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position += m_Velocity * Time.deltaTime;
	}
}
