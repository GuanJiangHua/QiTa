using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAdaption : MonoBehaviour
{
    //控制所有带image组件的子物体
    //将其对齐底部
    //让它们横向间距相同
    //1，（当屏幕变宽时，5个方形宽度不变，间距变大，但依然保持间距相同）/2，（当屏幕变宽时，5个方形宽度变大，间距不变）
    public SelfAdaptionType selfAdaptionType = SelfAdaptionType.spacing;

    private List<Image> m_ImageList;


    private Dictionary<string,float> m_WidthRatioDic;   //image原始宽度占比（key：Image对象名称）
    private float constantSpacing;                      //恒定间距

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //1,获取所有带有image的子物体:
        Image[] subobject = GetComponentsInChildren<Image>();
        m_ImageList = new List<Image>(subobject);

        if (m_ImageList != null && m_ImageList.Count > 0)
        {
            //2,对齐排列至底部,并使横向间距相等:
            AlignAndArrange();

            //3,开始自适应计算:
            StartCoroutine(AdaptiveComputation());
        }
    }

    /// <summary>
    /// 对齐和排列
    /// </summary>
    private void AlignAndArrange()
    {
        RectTransform canvasTf = transform as RectTransform;

        //获取画布高度:
        float height = canvasTf.sizeDelta.y;
        //获取画布宽度:
        float width = canvasTf.sizeDelta.x;
        //计算间距:
        float remainingWidth = width;

        for (int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            remainingWidth -= temp.sizeDelta.x;
        }
        float spacing = remainingWidth / (m_ImageList.Count + 1);

        float startPosition = -width / 2;
        for (int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            //计算y坐标值:
            float YCoordinateValue = (-height + temp.sizeDelta.y)/2;
            //计算x坐标值:
            float XCoordinateValue = startPosition + spacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, YCoordinateValue);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + spacing + temp.sizeDelta.x;
        }

        //获取恒定的间距，和每个image对象占的所有对象宽度之和的比
        if (selfAdaptionType == SelfAdaptionType.spacing)
        {
            constantSpacing = spacing;
            m_WidthRatioDic = new Dictionary<string, float>();
            for(int i = 0; i < m_ImageList.Count; i++)
            {
                RectTransform temp = m_ImageList[i].transform as RectTransform;
                float ratio = temp.sizeDelta.x / (width-remainingWidth);

                m_WidthRatioDic.Add(temp.gameObject.name, ratio);
            }
        }
    }

    /// <summary>
    /// 恒定宽度的自适应方式
    /// </summary>
    private void AdaptationConstantWidth()
    {
        RectTransform canvasTf = transform as RectTransform;

        //获取画布宽度:
        float width = canvasTf.sizeDelta.x;
        //计算间距:
        float remainingWidth = width;
        for (int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            remainingWidth -= temp.sizeDelta.x;
        }
        float spacing = remainingWidth / (m_ImageList.Count + 1);

        float startPosition = -width / 2;
        for (int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            //计算x坐标值:
            float XCoordinateValue = startPosition + spacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, temp.anchoredPosition.y);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + spacing + temp.sizeDelta.x;
        }
    }

    /// <summary>
    /// 恒定间距的自适应方式
    /// </summary>
    private void AdaptiveConstantSpacing()
    {
        RectTransform canvasTf = transform as RectTransform;

        //获取当前画布宽度:
        float currWidth = canvasTf.sizeDelta.x;

        //计算除去间距后剩余宽度:
        float remainingWidth = currWidth - (m_ImageList.Count + 1) * constantSpacing;
        if (remainingWidth < 10) remainingWidth = 10f;

        for(int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            Vector2 newSize = new Vector2(0, temp.sizeDelta.y);
            newSize.x = m_WidthRatioDic[temp.gameObject.name] * remainingWidth;

            temp.sizeDelta = newSize;
        }

        float startPosition = -currWidth / 2;
        for (int i = 0; i < m_ImageList.Count; i++)
        {
            RectTransform temp = m_ImageList[i].transform as RectTransform;
            //计算y坐标值:
            float YCoordinateValue = temp.anchoredPosition.y;
            //计算x坐标值:
            float XCoordinateValue = startPosition + constantSpacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, YCoordinateValue);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + constantSpacing + temp.sizeDelta.x;
        }
    }

    /// <summary>
    /// 自适应计算的协程:
    /// </summary>
    IEnumerator AdaptiveComputation()
    {
        while (true)
        {
            switch (selfAdaptionType)
            {
                case SelfAdaptionType.spacing:
                    AdaptiveConstantSpacing();
                    break;
                case SelfAdaptionType.width:
                    AdaptationConstantWidth();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 自适应类型:
    /// 1,间距；2,宽度
    /// </summary>
    public enum SelfAdaptionType
    {
        spacing,
        width
    }
}
