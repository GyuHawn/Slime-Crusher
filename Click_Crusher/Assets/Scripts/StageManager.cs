using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    private MonsterSpwan monsterSpawn;
    private SelectItem selectItem;

    private bool gameStart = false; // ���ӽ��� ����

    public int mainStage; // ���ν������� (1, 2, 3..)
    public int subStage; // ���꽺������ (1-1, 1-2...)
    public TMP_Text stageText;

    public int baseMonster; // �Ϲݸ��� ��
    public int strongMonster; // ��ȭ���� ��
    public int bossMonster; // �������� ��

    public int monsterCount = 0; // ��ȯ�� ���� ��

    private void Awake()
    {
        monsterSpawn = GameObject.Find("Manager").GetComponent<MonsterSpwan>();
        selectItem = GameObject.Find("Manager").GetComponent<SelectItem>();
    }

    void Start()
    {
        // 1-1 ���� ���� �� ���� ����
        if (!gameStart)
        {
            mainStage = 1;
            subStage = 1;
            StageMonsterSetting();
            SpawnMonsters();
            gameStart = true;
        }
    }

    void Update()
    {
        stageText.text = "Stage " + mainStage + "-" + subStage;
    }

    public void StageMonsterSetting()
    {
        if (subStage == 1)
        {
            //baseMonster = 5;
            baseMonster = 1;
        }
        else if (subStage == 2)
        {
            //baseMonster = 4;
            //strongMonster = 2;
            baseMonster = 1;
            strongMonster = 1;
        }
        else if (subStage == 3)
        {
            //baseMonster = 3;
            //strongMonster = 3;
            baseMonster = 1;
            strongMonster = 1;
        }
        else if (subStage == 4)
        {
            //baseMonster = 3;
            //strongMonster = 4;
            baseMonster = 1;
            strongMonster = 1;
        }
        else if (subStage == 5)
        {
            //baseMonster = 4;
            //strongMonster = 3;
            baseMonster = 1;
            strongMonster = 1;
            bossMonster = 1;
        }
    }

    void NextStageSetting()
    {
        baseMonster = 1;
        strongMonster = 0;
        bossMonster = 0;
    }
  
    void SpawnMonsters()
    {
        monsterCount = baseMonster + strongMonster + bossMonster; // ���� �� ����
        monsterSpawn.MonsterInstantiate(baseMonster, strongMonster, bossMonster);
    }

    public void NextStage()
    {
        if (monsterCount > 0) return; // ���Ͱ� �����ִٸ� ������� ����

        subStage++;
        if (subStage > 5)
        {
            subStage = 1;
            mainStage++;

            selectItem.selectItemMenu.SetActive(true);
            StartCoroutine(DelayStage());
        }

        NextStageSetting(); // �������� �̵��� ���ͼ� �ʱ�ȭ
        StageMonsterSetting();
        SpawnMonsters();
    }

    IEnumerator DelayStage()
    {
        yield return new WaitForSeconds(3f);
    }
}