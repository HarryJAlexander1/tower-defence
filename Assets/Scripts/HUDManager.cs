using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TMP_Text Health;
    public TMP_Text TowerHealth;
    public TMP_Text RoundNo;
    public TMP_Text Fund;
    public TMP_Text Score;
    public GameObject GameManagerObject;
    private GameManager GameManager;

    // Start is called before the first frame update
    void Start()
    {
        GameManagerObject = GameObject.FindWithTag("GameManager");
        GameManager = GameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Health.text = "Health: " + GameManager.PlayerHealth.ToString();
        TowerHealth.text = "Tower Health: " + GameManager.TowerHealth.ToString();
        RoundNo.text = "Round: " + GameManager.LevelCount.ToString();
        Fund.text = "Fund: " + GameManager.Fund.ToString();
        Score.text = "Score: " + GameManager.PlayerScore.ToString();
    }
}
