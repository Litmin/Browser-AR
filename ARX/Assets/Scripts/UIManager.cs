using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager GetInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UIManager>() as UIManager;
                if(instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        scanning = true;
        ARRender = true;
        bFace = false;
    }

    //扫描按钮
    private bool scanning;
    public Sprite ScanIcon;
    public Sprite NotScanIcon;
    public Image ScanButtonImage;
   // public Text ScanStateText;
    public void ScanButton()
    {
        MainManager.GetInstance.m_UDTEventHandler.BuildNewTarget();

        if (scanning == false)
        {
            //ScanStateText.text = "扫描中";
            MainManager.GetInstance.m_QRCodeReader.m_Scanning = true;
            ScanButtonImage.sprite = ScanIcon;
            scanning = true;
            ShowToast("开始扫描");
        }
        else
        {
           // ScanStateText.text = "开始扫描";
            MainManager.GetInstance.m_QRCodeReader.m_Scanning = false;
            ScanButtonImage.sprite = NotScanIcon;
            scanning = false;
            ShowToast("已停止扫描");
        }
    }

    //AR 实景按钮
    //按钮显示的图片
    public Sprite ARIcon;
    public Sprite RealIcon;
    public Image AROrRealButtonImage;
    public Text AROrRealButtonText;
    public bool ARRender;
    public void AROrRealButton()
    {
        if(ARRender)
        {
            ARRender = false;
            AROrRealButtonText.text = "实景";
            AROrRealButtonImage.sprite = RealIcon;
            if(MainManager.GetInstance.WebPlaneBehavior != null)
                MainManager.GetInstance.WebPlaneBehavior.gameObject.SetActive(false);
            ShowToast("已切换为实景模式");
        }
        else
        {
            ARRender = true;
            AROrRealButtonText.text = "AR";
            AROrRealButtonImage.sprite = ARIcon;
            if (MainManager.GetInstance.WebPlaneBehavior != null)
                MainManager.GetInstance.WebPlaneBehavior.gameObject.SetActive(true);
            ShowToast("已切换为AR模式");
        }
    }


    //面向按钮
    public Sprite FaceIcon;
    public Sprite NoFaceIcon;
    public Image FaceOrNoFaceButtonImage;
    public Text FaceOrNoFaceButtonText;
    public bool bFace;
    public void BillBoardButton()
    {
        if(bFace)
        {
            bFace = false;
            FaceOrNoFaceButtonText.text = "固定";
            FaceOrNoFaceButtonImage.sprite = NoFaceIcon;
            if (MainManager.GetInstance.WebPlaneBehavior != null)
                MainManager.GetInstance.WebPlaneBehavior.FaceCamera = false;
            ShowToast("模型朝向已固定");
        }
        else
        {
            bFace = true;
            FaceOrNoFaceButtonText.text = "跟随";
            FaceOrNoFaceButtonImage.sprite = FaceIcon;
            if (MainManager.GetInstance.WebPlaneBehavior != null)
                MainManager.GetInstance.WebPlaneBehavior.FaceCamera = true;
            ShowToast("模型正对手机");
        }
    }

    //Toast
    public Image ToastImage;
    public Text ToastText;
    private Coroutine FadeOutCoroutine;

    public void ShowToast(string toastString)
    {
        ToastText.text = toastString;
        ToastImage.color = new Color(1,1,1,0.5f);
        ToastText.color = new Color(0.74f, 0.74f, 0.74f, 1.0f);
        if (FadeOutCoroutine != null)
            StopCoroutine(FadeOutCoroutine);
        FadeOutCoroutine = StartCoroutine("FadeOutToast");
    }

    IEnumerator FadeOutToast()
    {
        float timer;
        for (timer = 0;timer < 2.0f;)
        {
            timer += Time.deltaTime;
            ToastImage.color = new Color(1, 1, 1, ((2.0f-timer)/2.0f) * 0.5f);
            ToastText.color = new Color(0.74f, 0.74f, 0.74f, ((2.0f - timer) / 2.0f) * 0.74f);
            yield return new WaitForEndOfFrame();
        }
    }

    //创建自己的应用 打开网页
    public void CreateSelfApp()
    {
        Application.OpenURL("http://browser-ar.com/");
    }
}
