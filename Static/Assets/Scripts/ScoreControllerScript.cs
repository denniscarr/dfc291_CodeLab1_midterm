using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreControllerScript : MonoBehaviour {

    // Score display
	public int score = 0;
	public TextMesh scoreDisplay;
	public TextMesh multNumber;

    // Multiplier bar variables.
	public GameObject multiplierBar;
	public float multBarStartVal = 0.4f;
	public float multBarBaseDecay = 0.01f;
	public float multBarSizeMin = 0.03f;
	public float multBarSizeMax = 7.15f;
	float multBarValCurr;
	float multBarDecayCurr;
	float multBarStartValCurr;

	float multiplier = 1f;

	public int enemyScoreValue = 1000;
	public float enemyMultValue = 0.5f;
	public float bulletHitValue = 0.01f;
	public float getHurtPenalty = 0.1f;

    // Audio
	public AudioSource levelWinAudio;
	public AudioSource playerHurtAudio;

	// Used for level generation
	int levelNumber = 0;
	int numberOfEnemies = 4;
	int numberOfObstaclesMin = 10;
	int numberOfObstaclesMax = 50;
	int currentEnemyAmt;
    LevelGenScript levelGenerator;

    // Used for high score list.
    List<ScoreEntry> highScores;
    public Transform highScoreList;
    public Transform highScoreScreen;

    // Misc
    Transform floor;    // The floor of the game environment.

	void Awake()
    {
//		GameObject.Find ("Scripts").GetComponent<LevelGenScript> ().numberOfEnemies = numberOfEnemies;
//		GameObject.Find ("Scripts").GetComponent<LevelGenScript> ().numberOfObstacles = Random.Range(numberOfObstaclesMin, numberOfObstaclesMax);
//		GameObject.Find ("Scripts").GetComponent<LevelGenScript> ().SendMessage ("Generate");
	}

	void Start()
	{
		currentEnemyAmt = numberOfEnemies;
		multBarValCurr = multBarStartVal;
		multBarDecayCurr = multBarBaseDecay;
		multBarStartValCurr = multBarStartVal;
		multiplierBar.transform.localScale = new Vector3 (
			multiplierBar.transform.localScale.x,
			MyMath.Map (multBarValCurr, 0f, 1f, multBarSizeMin, multBarSizeMax),
			multiplierBar.transform.localScale.z
		);

		scoreDisplay.text = score.ToString();
		multNumber.text = multiplier.ToString () + "X";

        floor = GameObject.Find("Floor").transform;
        levelGenerator = GameObject.Find("Game Manager").GetComponent<LevelGenScript>();

        // Load high scores.
        highScores = new List<ScoreEntry>();
        highScoreList = GameObject.Find("High Score List").transform;
        highScoreScreen = GameObject.Find("High Score Screen").transform;
        //SaveHighScores();
        ShowHighScores();
	}

	void Update() {

		// Apply decay to multiplier bar
		multBarValCurr -= multBarDecayCurr*Time.deltaTime;
		multBarValCurr = Mathf.Clamp (multBarValCurr, 0f, 1f);

		// See if we need to lower the multiplier level
		if (multBarValCurr <= 0f && multiplier > 1f) {
			multiplier -= 0.1f;
			multBarStartValCurr = multBarStartVal/multiplier;
			multBarValCurr = multBarStartValCurr;
			multBarDecayCurr = multBarBaseDecay * multiplier;
			multNumber.text = multiplier.ToString () + "X";
		}

		// Update multiplier bar
		multiplierBar.transform.localScale = new Vector3 (
			multiplierBar.transform.localScale.x,
			MyMath.Map (multBarValCurr, 0f, 1f, multBarSizeMin, multBarSizeMax),
			multiplierBar.transform.localScale.z
		);
	}

	public void KilledEnemy()
    {
        // Increase the multiplier.
		float multBarIncreaseAmt = enemyMultValue / multiplier;
		multBarValCurr += multBarIncreaseAmt;

		// See if we need to raise the multiplier level
		if (multBarValCurr >= 1f)
        {
			multiplier += 0.1f;
			multBarStartValCurr = multBarStartVal/multiplier;
			multBarValCurr = multBarStartValCurr;
			multBarDecayCurr = multBarBaseDecay * multiplier;
		}

		multNumber.text = multiplier.ToString () + "X";

		score = Mathf.RoundToInt(score+enemyScoreValue * multiplier);
		scoreDisplay.text = score.ToString ();

		// See if it's time to end level
		currentEnemyAmt -= 1;
		if (currentEnemyAmt <= 0)
        {
			EndLevel ();
		}
	}

	void EndLevel()
	{
		levelWinAudio.Play ();

        // Disable the floor's collider so the player falls through it.
		floor.GetComponent<MeshCollider> ().enabled = false;

        levelNumber += 1;
		numberOfEnemies = levelNumber*7;
//		numberOfObstaclesMin *= levelNumber;
//		numberOfObstaclesMax *= levelNumber;
		score = Mathf.RoundToInt (score+1000*multiplier*levelNumber);
		GameObject.Find ("Game Manager").GetComponent<LevelGenScript> ().numberOfEnemies = numberOfEnemies;
		GameObject.Find ("Game Manager").GetComponent<LevelGenScript> ().numberOfObstacles = Random.Range(numberOfObstaclesMin, numberOfObstaclesMax);
		GameObject.Find ("Game Manager").GetComponent<LevelGenScript> ().Invoke ("Generate", 1.4f);
		currentEnemyAmt = numberOfEnemies;
	}

	public void BulletHit() {
		score = Mathf.RoundToInt (score+multiplier);
		scoreDisplay.text = score.ToString ();
		multBarValCurr += bulletHitValue;
	}

	void GetHurt() {
		playerHurtAudio.Play ();
		multBarValCurr -= getHurtPenalty;
	}


    // Loads high scores from player preferences into a List.
    void LoadHighScores()
    {
        // Initialize 10 empty score entries in the highScore List.
        for (int i = 0; i < 10; i++)
        {
            highScores.Add(new ScoreEntry("AAA", 0));
        }

        for (int i = 0; i < 10; i++)
        {
            // Check to see if this score entry has previously been saved.
            if (PlayerPrefs.GetString("HighScoreName" + i) != "")
            {
                highScores[i] = new ScoreEntry(PlayerPrefs.GetString("HighScoreName" + i), PlayerPrefs.GetInt("HighScoreNumber" + i));
            }
        }
    }


    void SaveHighScores()
    {
        for (int i = 0; i < 10; i++)
        { 
            PlayerPrefs.SetString("HighScoreName" + i, highScores[i].initials);
            PlayerPrefs.SetInt("HighScoreNumber" + i, highScores[i].score);
        }
    }


    void ShowHighScores()
    {
        LoadHighScores();

        highScoreScreen.gameObject.SetActive(true);

        string scoreList = "";

        for (int i = 0; i < highScores.Count; i++)
        {
            scoreList += highScores[i].initials + ": " + highScores[i].score + "\n";
        }

        print(scoreList);
        highScoreList.GetComponent<TextMesh>().text = scoreList;

        SaveHighScores();
    }


    class ScoreEntry
    {
        public string initials;
        public int score;

        public ScoreEntry(string _initials, int _score)
        {
            initials = _initials;
            score = _score;
        }
    }
}
