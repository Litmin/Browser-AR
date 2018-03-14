using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebPlaneBehavior : MonoBehaviour
{
    //ImageTarget GameObject
    public GameObject NormalTargetGameObject;
    public GameObject UDTGameObject;

    //Target是否在追踪
    public bool NormalTracking;
    public bool UDTIsTracking;

    //根据Target的缩放系数
    public float NormalScaleFromTarget;
    public float UDTScaleFromTarget;

    //网页材质
    public Material WebMaterial;

    //是否面向Camera
    public bool FaceCamera;

    //解码的参数
    //与target的宽度缩放系数   0-100  default:5
    public float m_ar_w;
    //宽度分辨率         0-10000 default:1080
    public int m_ar_px;
    //宽高比           0-100    default:1
    public float m_ar_whratio;
    //页面中心距target中心的高度 0-100  default:0 浮于target的高度
    public float m_ar_H;
    //页面与AR码所在平面，竖直方向的角度 -90~90 default:0
    public float m_ar_pitching;
    //页面与AR码所在平面，水平方向的角度 -90~90 default:0
    public float m_ar_rolling;
    //透明度 0到1 default :1
    public float m_ar_opacity;
    //是否朝向摄像机
    public int m_ar_toward;
    //抠像背景色
    public Color m_ar_bgcolor;

    //加载动画
    public GameObject webLoadGameobject;

    private void Awake()
    {
        NormalScaleFromTarget = 0.1f;
        UDTScaleFromTarget = 0.05f;

        NormalTracking = false;
        UDTIsTracking = false;

        m_ar_w = 2;
        m_ar_px = 1080;
        m_ar_H = 0.0f;
        m_ar_whratio = 1.0f;
        m_ar_pitching = 0.0f;
        m_ar_rolling = 0.0f;
        m_ar_opacity = 1.0f;
        m_ar_toward = 0;
        m_ar_bgcolor = new Color(-1, -1, -1);
    }
    void Start ()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
	
	void Update ()
    {
        NormalTracking = false;
        UDTIsTracking = false;
        if (NormalTargetGameObject.GetComponent<NormalTrackableEventHandler>().IsTracking)
        {
            NormalTracking = true;
            UDTIsTracking = false;
        }
        else if(UDTGameObject != null && UDTGameObject.GetComponent<UserTrackableEventHandler>().isTracking)
        {
            NormalTracking = false;
            UDTIsTracking = true;
        }
        //正常定位
		if(NormalTargetGameObject != null  && NormalTracking/* && !MainManager.GetInstance.UseUserDefinedTarget*/)
        {
            //如果有target 并且能追踪到 并且允许更新位置 则根据target更新网页的位置旋转缩放
            //平移
            this.transform.position = NormalTargetGameObject.transform.position;
            this.transform.Translate(0, m_ar_H / 20, 0);
            //缩放
            this.transform.localScale = new Vector3(NormalTargetGameObject.transform.localScale.x * NormalScaleFromTarget * m_ar_w, NormalTargetGameObject.transform.localScale.y * NormalScaleFromTarget, NormalTargetGameObject.transform.localScale.x * NormalScaleFromTarget * m_ar_w / m_ar_whratio);
            //旋转
            if (FaceCamera)
            {
                this.transform.LookAt(Camera.main.transform.position);
                this.transform.Rotate(90, 0, 0);
                this.transform.Rotate(0, 180, 0);
            }
            else
            {
                this.transform.rotation = NormalTargetGameObject.transform.rotation;
                this.transform.Rotate(m_ar_rolling, 0, m_ar_pitching);
            }
            return;
        }

        //UDT定位
        if (UDTGameObject != null  && UDTIsTracking /*&& MainManager.GetInstance.UseUserDefinedTarget*/)
        {
            //如果有target 并且能追踪到 并且允许更新位置 则根据target更新网页的位置旋转缩放
            this.transform.position = UDTGameObject.transform.position;
            this.transform.Translate(0, m_ar_H / 20, 0);
            //缩放
            this.transform.localScale = new Vector3(UDTGameObject.transform.localScale.x * UDTScaleFromTarget * m_ar_w, UDTGameObject.transform.localScale.y * UDTScaleFromTarget, UDTGameObject.transform.localScale.x * UDTScaleFromTarget * m_ar_w / m_ar_whratio);
            if (FaceCamera)
            {
                this.transform.LookAt(Camera.main.transform.position);
                this.transform.Rotate(90, 0, 0);
                this.transform.Rotate(0, 180, 0);
            }
            else
            {
                this.transform.rotation = UDTGameObject.transform.rotation;
                this.transform.Rotate(0, -90, 0);
                this.transform.Rotate(m_ar_pitching, 0, m_ar_rolling);
            }
        }
    }
    //初始化二维码参数
    public void Init(float _ar_w,int _ar_px,float _ar_whratio,float _ar_H,float _ar_pitching,float _ar_rolling,float _ar_opacity,int _ar_toward,Color _ar_bgcolor)
    {
        m_ar_w = _ar_w > 0.0f ? _ar_w : 2.0f;
        m_ar_px = _ar_px;
        m_ar_whratio = _ar_whratio;
        m_ar_H = _ar_H;
        m_ar_pitching = _ar_pitching;
        m_ar_rolling = _ar_rolling;
        m_ar_opacity = _ar_opacity;
        m_ar_toward = _ar_toward;
        m_ar_bgcolor = _ar_bgcolor;
        
        //设置normal game object
       // NormalTargetGameObject = _normalTarget;
        //更新网页透明度
        GetComponent<MeshRenderer>().material.SetFloat("_Opacity", _ar_opacity);
        //设置抠像背景色
        if (m_ar_bgcolor.Equals(new Color(-1,-1,-1)))
        {
            //不扣像
            GetComponent<MeshRenderer>().material.SetFloat("_BKey", -1);

        }
        else
        {
            //抠像
            GetComponent<MeshRenderer>().material.SetFloat("_BKey", 1);
            GetComponent<MeshRenderer>().material.SetColor("_KeyBg", m_ar_bgcolor);
        }
        //是否朝向摄像机
        if (m_ar_toward == 0)
        {
            FaceCamera = false;
            //更新UI
            if(UIManager.GetInstance.bFace == true)
            {
                UIManager.GetInstance.BillBoardButton();
            }
        }
        else if (m_ar_toward == 1)
        {
            FaceCamera = true;
            //更新UI
            if (UIManager.GetInstance.bFace == false)
            {
                UIManager.GetInstance.BillBoardButton();
            }
        }
        //更新是否显示的UI
        if(UIManager.GetInstance.ARRender == false)
        {
            UIManager.GetInstance.AROrRealButton();
        }
    }
}
