using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAdaption : MonoBehaviour
{
    //�������д�image�����������
    //�������ײ�
    //�����Ǻ�������ͬ
    //1��������Ļ���ʱ��5�����ο�Ȳ��䣬����󣬵���Ȼ���ּ����ͬ��/2��������Ļ���ʱ��5�����ο�ȱ�󣬼�಻�䣩
    public SelfAdaptionType selfAdaptionType = SelfAdaptionType.spacing;

    private List<Image> m_ImageList;


    private Dictionary<string,float> m_WidthRatioDic;   //imageԭʼ���ռ�ȣ�key��Image�������ƣ�
    private float constantSpacing;                      //�㶨���

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    private void Init()
    {
        //1,��ȡ���д���image��������:
        Image[] subobject = GetComponentsInChildren<Image>();
        m_ImageList = new List<Image>(subobject);

        if (m_ImageList != null && m_ImageList.Count > 0)
        {
            //2,�����������ײ�,��ʹ���������:
            AlignAndArrange();

            //3,��ʼ����Ӧ����:
            StartCoroutine(AdaptiveComputation());
        }
    }

    /// <summary>
    /// ���������
    /// </summary>
    private void AlignAndArrange()
    {
        RectTransform canvasTf = transform as RectTransform;

        //��ȡ�����߶�:
        float height = canvasTf.sizeDelta.y;
        //��ȡ�������:
        float width = canvasTf.sizeDelta.x;
        //������:
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
            //����y����ֵ:
            float YCoordinateValue = (-height + temp.sizeDelta.y)/2;
            //����x����ֵ:
            float XCoordinateValue = startPosition + spacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, YCoordinateValue);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + spacing + temp.sizeDelta.x;
        }

        //��ȡ�㶨�ļ�࣬��ÿ��image����ռ�����ж�����֮�͵ı�
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
    /// �㶨��ȵ�����Ӧ��ʽ
    /// </summary>
    private void AdaptationConstantWidth()
    {
        RectTransform canvasTf = transform as RectTransform;

        //��ȡ�������:
        float width = canvasTf.sizeDelta.x;
        //������:
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
            //����x����ֵ:
            float XCoordinateValue = startPosition + spacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, temp.anchoredPosition.y);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + spacing + temp.sizeDelta.x;
        }
    }

    /// <summary>
    /// �㶨��������Ӧ��ʽ
    /// </summary>
    private void AdaptiveConstantSpacing()
    {
        RectTransform canvasTf = transform as RectTransform;

        //��ȡ��ǰ�������:
        float currWidth = canvasTf.sizeDelta.x;

        //�����ȥ����ʣ����:
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
            //����y����ֵ:
            float YCoordinateValue = temp.anchoredPosition.y;
            //����x����ֵ:
            float XCoordinateValue = startPosition + constantSpacing + temp.sizeDelta.x / 2;

            Vector2 newPos = new Vector2(XCoordinateValue, YCoordinateValue);
            temp.anchoredPosition = newPos;

            startPosition = startPosition + constantSpacing + temp.sizeDelta.x;
        }
    }

    /// <summary>
    /// ����Ӧ�����Э��:
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
    /// ����Ӧ����:
    /// 1,��ࣻ2,���
    /// </summary>
    public enum SelfAdaptionType
    {
        spacing,
        width
    }
}
