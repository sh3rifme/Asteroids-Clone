using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

	[SerializeField]
	private Text[] m_Scores;

	[SerializeField]
	private Text[] m_Names;

	private ScoreManager m_ScoreManager;

	void Start()
	{
		m_ScoreManager = ScoreManager._Instance;

		for (int i = 0; i < m_ScoreManager.Scores.Count; i++) 
		{
			m_Scores[i].text = m_ScoreManager.Scores[i].ScoreVal.ToString();
			m_Names[i].text = m_ScoreManager.Scores[i].Name;
		}
	}
}
