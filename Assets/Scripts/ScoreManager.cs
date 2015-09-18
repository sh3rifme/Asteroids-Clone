using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {
	
	private List<Score> m_Scores;

	private static ScoreManager m_Inst;

	public struct Score
	{
		public int ScoreVal;
		public string Name;
	};

	public static ScoreManager _Instance
	{
		get
		{
			if (m_Inst == null)
			{
				m_Inst = new GameObject("ScoreManager").AddComponent<ScoreManager>();
			}
			return m_Inst;
		}
	}

	public List<Score> Scores
	{
		get { return m_Scores; }
	}

	// Use this for initialization
	void Awake () {
		m_Scores = new List<Score>();
		for (int i = 0; i < 10; i++)
		{
			Score temp = new Score();
			temp.Name = PlayerPrefs.GetString("Score" + i + "name");
			temp.ScoreVal = PlayerPrefs.GetInt("Score" + i + "score");

			if (temp.Name.Length == 0)
			{
				temp.Name = " ";
			}
			m_Scores.Add(temp);
			SortScores();
		}
	}

	private void SortScores()
	{
		List<Score> temp = new List<Score>();
		foreach (Score score in m_Scores)
		{
			if (!(score.Name == " " && score.ScoreVal == 0))
			{
				temp.Add(score);
			}
		}
		m_Scores = temp;

		m_Scores.Sort((x, y) => x.ScoreVal.CompareTo(y.ScoreVal));
		m_Scores.Reverse();
		while (m_Scores.Count > 10)
		{
			m_Scores.RemoveAt(m_Scores.Count - 1);
		}

		for (int i = 0; i < 10; i++)
		{
			if (i < m_Scores.Count)
			{
				PlayerPrefs.SetString("Score" + i + "name", m_Scores[i].Name);
				PlayerPrefs.SetInt("Score" + i + "score", m_Scores[i].ScoreVal);
			}
			else
			{
				PlayerPrefs.SetString("Score" + i + "name", " ");
				PlayerPrefs.SetInt("Score" + i + "score", 0);
			}
		}
	}

	public void AddScore(int _scoreVal, string _name)
	{
		Score temp = new Score();
		temp.ScoreVal = _scoreVal;
		temp.Name = _name;
		m_Scores.Add(temp);
		SortScores();
	}
}
