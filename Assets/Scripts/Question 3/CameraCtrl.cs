using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private Vector3 Offset;
    private Transform m_CurrTrayer;

    private static CameraCtrl instance;
    public static CameraCtrl Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if(m_CurrTrayer != null)
        {
            transform.position = Vector3.Lerp(transform.position, m_CurrTrayer.position + Offset, 0.1f);
        }
    }

    public void SetTrayer(Transform trayer)
    {
        if (trayer != null)
        {
            Offset = transform.position - trayer.position;
            transform.LookAt(trayer);

            transform.position = trayer.position + Offset;
        }
        m_CurrTrayer = trayer;
    }
}
