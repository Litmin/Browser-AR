using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class MainManager : MonoBehaviour
{
    private static MainManager instance;

    public static MainManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainManager>() as MainManager;
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<MainManager>();
                }
            }
            return instance;
        }
    }


    //prefabs
    public GameObject WebPlanePrefab;
    //TargetGameObject
    public NormalTrackableEventHandler NormalImageTarget;
    private UserTrackableEventHandler UserDefinedImageTarget;

    //打开网页
    public AndroidWebview m_AndroidWebView;
    //网页位置 旋转 缩放
    public WebPlaneBehavior WebPlaneBehavior;

    //解析二维码
    public QRCodeReader m_QRCodeReader;
    //UserDefinedTargetBuilder
    public UDTEventHandler m_UDTEventHandler;

    //是否使用UserDefinedTarget
    public bool UseUserDefinedTarget;

    //是否开启判断要不要使用UDT的计时器
    private bool UpdateUseUDTTrigger;
    //计时器UseUDTTimer
    private float UseUDTTimer;
    //多少秒后开始使用UDT
    public float WaitForUseUDTMaxTime;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UseUserDefinedTarget = false;

        UpdateUseUDTTrigger = false;

        UseUDTTimer = 0.0f;
        WaitForUseUDTMaxTime = 2.0f;
    }

    private void Update()
    {
       // Debug.Log(UpdateUseUDTTrigger);
        //if(UpdateUseUDTTrigger)
        //{
        //    if(NormalImageTarget.IsTracking)
        //    {
        //        UpdateUseUDTTrigger = false;
        //    }
        //    UseUDTTimer += Time.deltaTime;
        //    if(UseUDTTimer >= WaitForUseUDTMaxTime)
        //    {
        //        //超过时间  使用UDT
        //        UpdateUseUDTTrigger = false;
        //        UseUDTTimer = 0.0f;
        //        BuildUserDefinedTarget();
        //        BeginUserDefinedTrack();
        //    }
        //}
    }

    //开始扫描二维码
    public void BeginScanQRCode()
    {
        m_QRCodeReader.m_Scanning = true;
    }

    //停止扫描二维码
    public void StopScanQRCode()
    {
        m_QRCodeReader.m_Scanning = false;
    }

    //开始用户自定义Target定位
    public void BeginUserDefinedTrack()
    {
        //NormalImageTarget.TrackActive = false;
        //UserDefinedImageTarget.TrackActive = true;
        UseUserDefinedTarget = true;
        //开启UDT Extened Tracking 关闭Normal Target的Extened Tracking
        StateManager stateManager = TrackerManager.Instance.GetStateManager();

        // 1. Stop extended tracking on all the trackables
        foreach (var tb in stateManager.GetTrackableBehaviours())
        {
            var itb = tb as ImageTargetBehaviour;
            if (itb != null)
            {
                itb.ImageTarget.StopExtendedTracking();
            }
        }

        List<TrackableBehaviour> trackableList = stateManager.GetTrackableBehaviours().ToList();

        if(trackableList.Count >= 2)
        {
            ImageTargetBehaviour lastItb = trackableList[1] as ImageTargetBehaviour;

            if (lastItb != null)
            {
                if (lastItb.ImageTarget.StartExtendedTracking())
                    Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
            }
        }

    }
    //停止用户自定义Target定位
    public void StopUserDefinedTrack()
    {
        if(UserDefinedImageTarget != null)
        {
            UserDefinedImageTarget.TrackActive = false;
        }
        UseUserDefinedTarget = false;
        UpdateUseUDTTrigger = false;
        UseUDTTimer = 0.0f;
        //开启Normal的Extened Tracking 关闭
        StateManager stateManager = TrackerManager.Instance.GetStateManager();

        // 1. Stop extended tracking on all the trackables
        foreach (var tb in stateManager.GetTrackableBehaviours())
        {
            var itb = tb as ImageTargetBehaviour;
            if (itb != null)
            {
                itb.ImageTarget.StopExtendedTracking();
            }
        }

        List<TrackableBehaviour> trackableList = stateManager.GetTrackableBehaviours().ToList();

       // if(trackableList.Count >= 1)
        {
            ImageTargetBehaviour lastItb = trackableList[0] as ImageTargetBehaviour;

            if (lastItb != null)
            {
                if (lastItb.ImageTarget.StartExtendedTracking())
                    Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
            }
        }
    }

    //开始计时器  多少秒后使用UDT
    public void BeginUDTTimer()
    {
        UpdateUseUDTTrigger = true;
        UseUDTTimer = 0.0f;
    }

    public void BuildUserDefinedTarget()
    {
        //删除之前的UDT
        //if(GameObject.FindGameObjectWithTag("UDT") != null)
        //{
        //    GameObject obj = GameObject.FindGameObjectWithTag("UDT");
        //    GameObject.Destroy(obj);
        //}

        m_UDTEventHandler.BuildNewTarget();

       
    }

    public void BuildDefinedTargetCallback()
    {
        UserDefinedImageTarget = FindObjectOfType<UserTrackableEventHandler>();
        WebPlaneBehavior.UDTGameObject = UserDefinedImageTarget.gameObject;
        BeginUserDefinedTrack();
    }

    public void InitWebObject(int _width,int _height,string _url, float _ar_w, int _ar_px, float _ar_whratio, float _ar_H, float _ar_pitching, float _ar_rolling, float _ar_opacity, int _ar_toward,Color _ar_bgcolor)
    {
        WebPlaneBehavior.Init(_ar_w,_ar_px,_ar_whratio,_ar_H,_ar_pitching,_ar_rolling,_ar_opacity,_ar_toward, _ar_bgcolor);
        m_AndroidWebView.Init(_width, _height, _url);
    }

}
