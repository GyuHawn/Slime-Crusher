using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Threading;

public class StageManager : MonoBehaviour
{
    private MonsterSpwan monsterSpawn;
    private SelectItem selectItem;
    private SelectPass selectPass;
    private StageTimeLimit stageTimeLimit;
    private PlayerController playerController;
    private CharacterSkill characterSkill;
    private Character character;
    private ItemSkill itemSkill;
    private StageStatus stageStatus;
    private StageTile stageTile;
    private Combo combo;

    public bool gameStart = false; // 게임시작 여부

    public GameObject[] map;
    public int mainStage; // 메인스테이지 (1, 2, 3...)
    public int subStage; // 서브스테이지 (1-1, 1-2...)
    public TMP_Text stageText;
    private bool isNextStageAvailable = true;

    public int base0Monster; // 스테이지 몬스터 [0]의 수
    public int base1Monster; // 스테이지 몬스터 [1]의 수
    public int base2Monster; // 스테이지 몬스터 [2]의 수
    public int base3Monster; // 스테이지 몬스터 [3]의 수
    public int bossMonster; // 보스몬스터 수
    public bool allMonstersSpawned = false; // 모든 몬스터 소환 확인

    // public int monsterCount = 0; // 소환된 몬스터 수

    public float timeLimit; // 스테이지당 제한시간

    public bool selectingItem;
    
    public float totalTime;
    public int rewardMoney;
    public TMP_Text totalTimeText;
    public TMP_Text rewardMoneyText;
    public TMP_Text finalWaveText;
    public TMP_Text maxComboText;
    public TMP_Text clearText;

    // 스테이지 타일
    public GameObject tileMap;
    public GameObject[] stageTiles;
    public TMP_Text tileMapStageText;

    public int tileNum;

    public bool passing;

    public Canvas canvas;

    private void Awake()
    {
        monsterSpawn = GameObject.Find("Manager").GetComponent<MonsterSpwan>();
        selectItem = GameObject.Find("Manager").GetComponent<SelectItem>();
        selectPass = GameObject.Find("Manager").GetComponent<SelectPass>();
        stageTimeLimit = GameObject.Find("Manager").GetComponent<StageTimeLimit>();
        playerController = GameObject.Find("Manager").GetComponent<PlayerController>();
        characterSkill = GameObject.Find("Manager").GetComponent<CharacterSkill>();
        character = GameObject.Find("Manager").GetComponent<Character>();
        itemSkill = GameObject.Find("Manager").GetComponent<ItemSkill>();
        stageStatus = GameObject.Find("Manager").GetComponent<StageStatus>();
        stageTile = GameObject.Find("Manager").GetComponent<StageTile>();
        combo = GameObject.Find("Manager").GetComponent<Combo>();
    }

    void Start()
    {
        // 1-1 시작 설정 후 게임 시작
        if (!gameStart)
        {
            mainStage = 1;
            subStage = 1;
            StageMonsterSetting();
            SpawnMonsters();
            selectingItem = false;
            gameStart = true;

            totalTime = 0;
            rewardMoney = 0;

            tileNum = 0;
        }
    }

    void Update()
    {
        if(mainStage > 20)
        {
            clearText.text = "게임 클리어!!";
            clearText.fontSize = 40;
            Time.timeScale = 0f;
            Reward();
            gameStart = false;
            playerController.gameover.SetActive(true);
        }

        if (mainStage <= 8)
        {
            for (int i = 0; i < mainStage - 1; ++i)
            {
                map[i].SetActive(false);
            }

            map[mainStage - 1].SetActive(true);
        }
        else
        {
            for (int i = 0; i < 7; ++i)
            {
                map[i].SetActive(false);
            }

            map[7].SetActive(true);
        }


        if (mainStage <= 7)
        {
            stageText.text = "Stage " + mainStage + "-" + subStage;
        }
        else if(mainStage >= 8)
        {
            stageText.text = "Stage " + mainStage;
        }

        if(stageTimeLimit.stageFail >= stageTimeLimit.stageTime)
        {
            playerController.gameover.SetActive(true);
            gameStart = false;
        }

        if(mainStage <= 9)
        {
            tileMapStageText.text = "(0" + mainStage + "/20)";
        }
        else
        {
            tileMapStageText.text = "(" + mainStage + "/20)";
        }

        totalTime = playerController.gameTime;
        totalTimeText.text = string.Format("{0:00}:{1:00}", Mathf.Floor(totalTime / 60), totalTime % 60);
        if(mainStage < 8)
        {
            finalWaveText.text = mainStage + " - " + subStage + " Wave";
        }
        else if(mainStage >= 8)
        {
            finalWaveText.text = mainStage + " Wave";
        }
        maxComboText.text = combo.maxComboNum.ToString();
        rewardMoneyText.text = rewardMoney.ToString();
    }

    public void Reward()
    {
        if (gameStart)
        {
            rewardMoney += (int)(totalTime * 1);
            rewardMoney += combo.maxComboNum;

            int playerMoney = PlayerPrefs.GetInt("GameMoney", 0);

            playerMoney += rewardMoney;

            PlayerPrefs.SetInt("GameMoney", playerMoney);
            PlayerPrefs.Save();
        }       
    }

    public void StageMonsterSetting()
    {
        if (mainStage <= 7)
        {
            // 1~7 스테이지 설정
            switch (subStage)
            {
                case 1:
                    base0Monster = 2;
                    break;
                case 2:
                    base0Monster = 2;
                    base1Monster = 2;
                    break;
                case 3:
                    base0Monster = 2;
                    base1Monster = 2;
                    base2Monster = 3;
                    break;
                case 4:
                    base0Monster = 2;
                    base1Monster = 2;
                    base2Monster = 3;
                    base3Monster = 3;
                    break;
                case 5:
                    base0Monster = 1;
                    base1Monster = 2;
                    base2Monster = 3;
                    base3Monster = 3;
                    bossMonster = 1;
                    break;
            }
        }
        else
        {
            // 8 스테이지 이후
            base0Monster = 2;
            base1Monster = 1;
            base2Monster = 1;
            base3Monster = mainStage - 7; // 8 스테이지 이후부터 몬스터 증가를 위한 값  
            bossMonster = 1;
        }
    }

    void NextStageSetting()
    {
        base0Monster = 1;
        base1Monster = 0;
        base2Monster = 0;
        base3Monster = 0;
        bossMonster = 0;

        stageTimeLimit.stageFail = 0f;
    }

    void SpawnMonsters()
    {
        //monsterCount = base0Monster + base1Monster + base2Monster + base3Monster + bossMonster; // 몬스터 수 설정
        monsterSpawn.MonsterInstantiate(base0Monster, base1Monster, base2Monster, base3Monster, bossMonster);

        allMonstersSpawned = true;
    }

    void SelectPass()
    {
        selectPass.passMenu.SetActive(true);
        playerController.isAttacking = true;
        Time.timeScale = 0f;
    }

    public void NextStage()
    {
        //if (monsterCount > 0) return; // 몬스터가 남아있다면 실행되지 않음
        if (isNextStageAvailable)
        {
            if (mainStage <= 20)
            {

                allMonstersSpawned = false;
                characterSkill.CharacterCoolTime();
                ResetStageState();


                if (character.currentCharacter == 2)
                {
                    characterSkill.useWaterSkill = false;
                }

                if (mainStage < 8)
                {
                    NextSubStage();

                    if (mainStage >= 2)
                    {
                        if (subStage == 2)
                        {
                            selectingItem = true;
                            SelectPass();
                        }
                    }

                    if (subStage == 3)
                    {
                        selectItem.ItemSelect();
                        StartCoroutine(DelayStage());
                    }
                    else if (subStage > 5)
                    {
                        Debug.Log("aa");
                        subStage = 1;
                        NextMainStage();

                        selectItem.ItemSelect();
                        StartCoroutine(DelayStage());
                    }
                }
                else
                {
                    NextMainStage();
                    if (mainStage == 12 || mainStage == 15 || mainStage == 18)
                    {
                        selectingItem = true;
                        SelectPass();
                    }
                    else if (mainStage == 10 || mainStage == 16)
                    {
                        selectItem.ItemSelect();
                        StartCoroutine(DelayStage());
                    }
                }

                if (mainStage >= 8)
                {
                    if (mainStage == 8 || mainStage == 10 || mainStage == 12 || mainStage == 15 || mainStage == 16 || mainStage == 18)
                    {
                    }
                    else
                    {
                        passing = true;
                        NextStage2();
                    }
                }
                else
                {
                    if ((mainStage >= 2 && (subStage == 1 || subStage == 2)) || subStage == 3)
                    {
                    }
                    else
                    {
                        passing = true;
                        NextStage2();
                    }
                }

            }
            StartCoroutine(DelayNextStage());
        }
    }

    IEnumerator DelayNextStage()
    {
        isNextStageAvailable = false;
        yield return new WaitForSeconds(1f); 

        isNextStageAvailable = true; 
    }

    public void NextStage2()
    {
        if(mainStage <= 20)
        {
            if (passing)
            {
                StartCoroutine(NextStageOrTile());
            }
        }
    }

    IEnumerator NextStageOrTile()
    {
        canvas.enabled = !canvas.enabled;
        tileMap.SetActive(true);
        stageTile.stageNext = true;
        stageTimeLimit.stageFail = 0f;
        tileNum++;
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= 2; i++)
        {
            stageTiles[tileNum].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            stageTiles[tileNum].SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

        stageTiles[tileNum].SetActive(true);
        yield return new WaitForSeconds(1f);
        passing = false;
        tileMap.SetActive(false);
        stageTile.stageNext = false;
        canvas.enabled = !canvas.enabled;

        yield return new WaitForSeconds(0.2f);

        stageStatus.ResetStatus();

        NextStageSetting(); // 스테이지 이동시 몬스터수 초기화
        StageMonsterSetting();
        SpawnMonsters();

        stageStatus.BuffStatus(allMonstersSpawned);
    }

    void ResetStageState()
    {
        GameObject[] skills = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject skill in skills)
        {
            if (skill.name == "BossSkill" || skill.name == "PlayerSkill" || skill.name == "MonsterAttack" || skill.name == "HealthUpItem")
            {
                Destroy(skill);
            }
        }

        itemSkill.holyWave = false;
        playerController.stage5Debuff = false;
    }

    void NextSubStage()
    {
        subStage++;
        rewardMoney += 10;
    }

    void NextMainStage()
    {
        mainStage++;
        rewardMoney += 100;
    }

    IEnumerator DelayStage()
    {
        yield return new WaitForSeconds(1f);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
