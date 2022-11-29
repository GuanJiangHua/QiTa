using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int plantCount;          //星球数量
    //public float flyDuration;       //飞行时间
    public float flySpeed;          //飞行速度
    public float stayDuration;      //停留时间
    public float r;

    private Vector3[] m_PlantPosArray;                    //星球点

    private int m_PassedPlantCount;                       //通过的星球的数量
    private int m_PassByPosArrayIndex;                    //当前路过的星球点数组的下标
    private Vector3[] m_PassByPosArray = new Vector3[3];  //路过的星球点

    private GameObject m_PlayerObj;
    private Vector3 m_CurrPlayerTrayer;                   //玩家当前的目标

    private List<GameObject> m_WakeLiat;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        //1,随机plantCount个星球点坐标
        m_PlantPosArray = new Vector3[plantCount];
        GameObject plantPosPrefab = Resources.Load<GameObject>("Prefab/Question 3/Plant Pos");
        for (int i = 0; i < plantCount; i++)
        {
            m_PlantPosArray[i] = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

            GameObject plantPosObj = Instantiate(plantPosPrefab);

            plantPosObj.transform.position = m_PlantPosArray[i];
        }

        //2,实例化玩家，并设置摄像机跟随
        GameObject playerPrefab = Resources.Load<GameObject>("Prefab/Question 3/Player Obj");
        m_PlayerObj = Instantiate(playerPrefab);
        m_PlayerObj.transform.position = Vector3.zero;
        CameraCtrl.Instance.SetTrayer(m_PlayerObj.transform);

        //设置玩家出生位置:
        m_CurrPlayerTrayer = m_PlantPosArray[Random.Range(0, plantCount)];
        m_PlayerObj.transform.position = m_CurrPlayerTrayer;

        m_PassByPosArrayIndex = 0;
        m_PassedPlantCount = 0;

        m_WakeLiat = new List<GameObject>();
        GameObject wakeObj = Instantiate(plantPosPrefab);
        wakeObj.GetComponent<MeshRenderer>().material = new Material(wakeObj.GetComponent<MeshRenderer>().material);
        wakeObj.GetComponent<MeshRenderer>().material.color = Color.blue;
        wakeObj.SetActive(false);
        for (int i = 0; i < 20; i++)
        {
            GameObject plantPosObj = Instantiate(wakeObj);
            m_WakeLiat.Add(plantPosObj);
        }

        StartCoroutine(Wake());

        SelectNextPlanet();
    }

    /// <summary>
    /// 选择下一个星球
    /// </summary>
    private void SelectNextPlanet()
    {
        //将上一个星球点添加入
        m_PassByPosArray[m_PassByPosArrayIndex] = m_CurrPlayerTrayer;
        m_PassedPlantCount++;
        m_PassByPosArrayIndex = (m_PassByPosArrayIndex + 1) % 3;

        //剔除经过的星球点
        int eliminateCount = m_PassedPlantCount >= 3 ? 3 : m_PassedPlantCount;
        List<Vector3> eligiblePosList = new List<Vector3>(m_PlantPosArray);
        for (int i = 0; i < eliminateCount; i++)
        {
            for (int j = 0; j < eligiblePosList.Count; j++)
            {
                if (eligiblePosList[j] == m_PassByPosArray[i])
                {
                    eligiblePosList.RemoveAt(j);
                    break;
                }
            }
        }

        //随机选择新星球点
        int newPosIndex = Random.Range(0, eligiblePosList.Count);

        m_CurrPlayerTrayer = eligiblePosList[newPosIndex];
        //前往下一个星球:
        GoToCurrentPlanetPos();
    }

    /// <summary>
    /// 前往当前星球点
    /// </summary>
    private void GoToCurrentPlanetPos()
    {
        StartCoroutine(GoToPlanetPos());
    }
    /// <summary>
    /// 前往星球点:
    /// </summary>
    IEnumerator GoToPlanetPos()
    {
        if (flySpeed <= 0) yield break;
        Vector3 startPos = m_PlayerObj.transform.position;
        Vector3 endPos = m_CurrPlayerTrayer;
        Vector3 midpoint = new Vector3(startPos.x + Random.Range(-r, r), startPos.y + Random.Range(-r, r), startPos.z + Random.Range(-r, r));

        float totalTime = SecondOrderBezierCurveTool.CurveLength(startPos, endPos, midpoint)/flySpeed;
        float currTime = 0;

        while (true)
        {
            m_PlayerObj.transform.position = SecondOrderBezierCurveTool.GetBezierCurveUniformSpeedPos(startPos, endPos, midpoint, currTime/ totalTime);
            if(currTime > totalTime)
            {
                m_PlayerObj.transform.position = endPos;
                break;
            }
            currTime += Time.deltaTime;
            //Vector3.Lerp(startPos, m_CurrPlayerTrayer, flightTime / flyDuration);
            yield return null;
        }

        yield return new WaitForSeconds(stayDuration);
        SelectNextPlanet();
    }

    IEnumerator Wake()
    {
        int index = 0;
        while (true)
        {
            m_WakeLiat[index].SetActive(true);
            m_WakeLiat[index].transform.position = m_PlayerObj.transform.position;
            yield return new WaitForSeconds(0.1f);
            index = (index + 1) % m_WakeLiat.Count;
        }
    }

    /*
    IEnumerator Text()
    {
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = new Vector3(10,10,10);
        Vector3 midpoint = new Vector3(startPos.x + Random.Range(-r, r), startPos.y + Random.Range(-r, r), startPos.z + Random.Range(-r, r));

        float totalLength = 10;
        float currTime = 0;

        float timer = 0.1f;
        GameObject plantPosPrefab = Resources.Load<GameObject>("Prefab/Question 3/Plant Pos");

        while (true)
        {
            //m_PlayerObj.transform.position = SecondOrderBezierCurveTool.GetBezierCurvePos(startPos, endPos, midpoint, flySpeed * currTime / totalLength);

            if (currTime > 5)
            {
                //m_PlayerObj.transform.position = endPos;
                break;
            }
            currTime += Time.fixedDeltaTime;
            GameObject plantPosObj = Instantiate(plantPosPrefab);
            plantPosObj.transform.position = SecondOrderBezierCurveTool.GetBezierCurveUniformSpeedPos(startPos, endPos, midpoint, flySpeed * currTime / 5);
            yield return new WaitForSeconds(0.02f);
        }
    }
    */
}
