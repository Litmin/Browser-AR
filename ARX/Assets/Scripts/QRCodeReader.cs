using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using Vuforia;
using System.Net;
using System;

public class QRCodeReader : MonoBehaviour
{
    //二维码扫描结果  测试用
    public Text resultText;
    //控制是否扫描二维码
    public bool m_Scanning = false;
    //解析二维码对象
    private BarcodeReader m_CodeReader;
    //Vuforia格式
    private Vuforia.Image.PIXEL_FORMAT m_PixelFormat = Vuforia.Image.PIXEL_FORMAT.GRAYSCALE;

    private bool m_RegisteredFormat = false;
    //从Vuforia 相机获取的图片
    private Vuforia.Image VuforiaImageData;
    //图片宽 高
    private int ImageWidth, ImageHeight;
    //unity 图片格式
    private Color32[] color32ImageData;
    //解析二维码的频率
    public float DecodeRate;
    //控制解码的Timer
    private float DecodeTimer;
    //存储已经扫描到的二维码
    public ArrayList URLStoreList;
    public string LastURL;

    //自动对焦Timer
    private float AutoFocusTimer;

    //存储参数
    //与target的宽度缩放系数   0-100  default:5
    public float temp_ar_w;
    //宽度分辨率         0-10000 default:1080
    public int temp_ar_px;
    //宽高比           0-100    default:1
    public float temp_ar_whratio;
    //页面中心距target中心的高度 0-100  default:0 浮于target的高度
    public float temp_ar_H;
    //页面与AR码所在平面，竖直方向的角度 -90~90 default:0
    public float temp_ar_pitching;
    //页面与AR码所在平面，水平方向的角度 -90~90 default:0
    public float temp_ar_rolling;
    //透明度 0到1 default :1
    public float temp_ar_opacity;
    //是否朝向摄像机
    public int temp_ar_toward;
    //抠像背景色
    public Color temp_ar_bgcolor;

    //特效协程
    private Coroutine showWebCoroutine;

    private bool ExpandFinish;
    private void OnEnable()
    {
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }

    private void Start()
    {
        m_CodeReader = new BarcodeReader();
        
        m_Scanning = true;

        DecodeRate = 0.3f;
        DecodeTimer = 0.0f;

        URLStoreList = new ArrayList();
        LastURL = "";

        AutoFocusTimer = 0.0f;

        //初始化存储的参数
        temp_ar_w = 2.0f;
        temp_ar_px = 1080;
        temp_ar_H = 0.0f;
        temp_ar_whratio = 1.0f;
        temp_ar_pitching = 0.0f;
        temp_ar_rolling = 0.0f;
        temp_ar_opacity = 1.0f;
        temp_ar_toward = 0;
        temp_ar_bgcolor = new Color(-1, -1, -1);

        StartCoroutine(StartAutoFocus());

        ExpandFinish = false;
    }

    private void Update()
    {
        AutoFocusTimer += Time.deltaTime;
        if(AutoFocusTimer > 0.3f)
        {
            Vuforia.CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            AutoFocusTimer = 0.0f;
        }
    }

    //切换程序后重新设置格式
    private void OnApplicationFocus(bool focus)
    {
        //if (focus == false)
        //{
        //    StartCoroutine(StartAutoFocus());

        //}
        //else
        //{
        //    //m_RegisteredFormat = false;
        //    StartCoroutine(StartAutoFocus());
        //    resultText.text = "hahahahah";

        //}
    }
    

    public void OnTrackablesUpdated()
    {

        if (!m_RegisteredFormat)
        {
            //Vuforia.CameraDevice.Instance.SetFrameFormat(m_PixelFormat, false);
            Vuforia.CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);

            m_RegisteredFormat = true;
        }
        
        DecodeTimer += Time.deltaTime;
        if (m_Scanning && Vuforia.CameraDevice.Instance != null && DecodeTimer >= DecodeRate)
        {
            //协程方式解析二维码
            ////获取摄像机数据
            //VuforiaImageData = Vuforia.CameraDevice.Instance.GetCameraImage(m_PixelFormat);

            //ImageWidth = VuforiaImageData.BufferWidth;
            //ImageHeight = VuforiaImageData.BufferHeight;

            //color32ImageData = ImageToColor32(VuforiaImageData);

            ////解码二维码
            //StartCoroutine(ScanQRCode());

            DecodeTimer = .0f;
            //多线程方式解析二维码
            Loom.RunAsync
            (
                () =>
                {
                    try
                    {
                        VuforiaImageData = Vuforia.CameraDevice.Instance.GetCameraImage(m_PixelFormat);

                        ImageWidth = VuforiaImageData.BufferWidth;
                        ImageHeight = VuforiaImageData.BufferHeight;

                        color32ImageData = ImageToColor32(VuforiaImageData);

                        var result = m_CodeReader.Decode(color32ImageData, ImageWidth, ImageHeight);

                        if (result != null && LastURL.Equals(result.Text) != true && result.Text != null)
                        {
                            ExpandFinish = false;
                            //解析短连接
                            string FinalResult = null;
                            FinalResult = ExpandShortUrl(result.Text);
                            if (FinalResult == null)
                            {
                                FinalResult = result.Text;
                            }
                            else
                            {

                            }
                            if (ExpandFinish)
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    //resultText.text = result.Text;

                                    //如果扫描的的二维码之前没扫到过
                                    //if(URLStoreList.Contains(result.Text) == false)
                                    //{
                                    //    MainManager.GetInstance.m_AndroidWebView.LoadUrl(result.Text);

                                    //    MainManager.GetInstance.BuildUserDefinedTarget();
                                    //}
                                    //else
                                    //{

                                    //}

                                    //如果这次扫描的二维码跟上一次不一样
                                    if (LastURL.Equals(result.Text) != true && result.Text != null)
                                    {
                                        LastURL = result.Text;
                                        //播放特效
                                        MainManager.GetInstance.WebPlaneBehavior.gameObject.GetComponent<MeshRenderer>().enabled = false;
                                        MainManager.GetInstance.WebPlaneBehavior.webLoadGameobject.GetComponent<WebLoadAnim>().child.GetComponent<MeshRenderer>().enabled = true;
                                        //if(showWebCoroutine != null)
                                        {
                                            StartCoroutine(ShowWeb());
                                        }
                                        //解析并应用参数
                                        DeCodeAndChangeWebviewParm(FinalResult);
                                        //初始化webplane
                                        MainManager.GetInstance.InitWebObject(temp_ar_px, (int)(temp_ar_px / temp_ar_whratio), FinalResult, temp_ar_w, temp_ar_px, temp_ar_whratio, temp_ar_H, temp_ar_pitching, temp_ar_rolling, temp_ar_opacity, temp_ar_toward, temp_ar_bgcolor);

                                        //将是否使用UDT变量设为false
                                        // MainManager.GetInstance.UseUserDefinedTarget = false;
                                        //开始计时  时间到后使用UDT
                                        // MainManager.GetInstance.BeginUDTTimer();

                                        if (MainManager.GetInstance.NormalImageTarget.IsTracking == false)
                                        {
                                            MainManager.GetInstance.BuildUserDefinedTarget();
                                            MainManager.GetInstance.BeginUserDefinedTrack();
                                        }
                                    }
                                }
                           );
                            }
                           

                        }
                    }
                    finally
                    {

                    }
                }
            );
        }
    }

    //解析二维码协程
    IEnumerator ScanQRCode()
    {
        var result = m_CodeReader.Decode(color32ImageData, ImageWidth, ImageHeight);

        if (result != null)
        {
          //  resultText.text = result.Text;

            MainManager.GetInstance.m_AndroidWebView.LoadUrl(result.Text);

            MainManager.GetInstance.BuildUserDefinedTarget();

            m_Scanning = false;
        }

        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 设置摄像头为自动对焦
    /// </summary>
    /// <returns></returns>
    IEnumerator StartAutoFocus()
    {
        Vuforia.CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// Vuforia 图片格式转为Color32
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    Color32[] ImageToColor32(Vuforia.Image a)
    {
        if (!a.IsValid()) return null;
        Color32[] r = new Color32[a.BufferWidth * a.BufferHeight];
        for (int i = 0; i < r.Length; i++)
        {
            r[i].r = r[i].g = r[i].b = a.Pixels[i];
        }
        return r;
    }

    //解码 更改网页显示参数
    public void DeCodeAndChangeWebviewParm(string _qrcode)
    {
        //初始化存储的参数
        temp_ar_w = 2;
        temp_ar_px = 1080;
        temp_ar_H = 0.0f;
        temp_ar_whratio = 1.0f;
        temp_ar_pitching = 0.0f;
        temp_ar_rolling = 0.0f;
        temp_ar_opacity = 1.0f;
        temp_ar_toward = 0;
        temp_ar_bgcolor = new Color(-1,-1,-1);

        //解码参数
        int wenhaoindex = _qrcode.IndexOf("?");
        if (wenhaoindex == -1 || wenhaoindex == _qrcode.Length - 1)
            return;
        string parmString = _qrcode.Remove(0, wenhaoindex);
        //去掉空格
        parmString.Replace(" ", "");
        //转换为小写
        _qrcode = _qrcode.ToLower();
        //宽度
        if(!float.TryParse(LittleDecode(_qrcode, "ar-w", "2"), out temp_ar_w))
        {
            temp_ar_w = 2.0f;
        }
        //分辨率
        if(!int.TryParse(LittleDecode(_qrcode, "ar-px", "1080"),out temp_ar_px))
        {
            temp_ar_px = 1080;
        }
        //宽高比
        if(!float.TryParse(LittleDecode(_qrcode, "ar-whratio", "1.0"),out temp_ar_whratio))
        {
            temp_ar_whratio = 1.0f;
        }
        //页面距码的高度
        if(!float.TryParse(LittleDecode(_qrcode, "ar-h", "0.0"),out temp_ar_H))
        {
            temp_ar_H = 0.0f;
        }
        //上下角度
        if(!float.TryParse(LittleDecode(_qrcode, "ar-pitching", "0.0"),out temp_ar_pitching))
        {
            temp_ar_pitching = 0.0f;
        }
        //左右角度
        if(!float.TryParse(LittleDecode(_qrcode, "ar-rolling", "0.0"),out temp_ar_rolling))
        {
            temp_ar_rolling = 0.0f;
        }
        //透明度
        if(!float.TryParse(LittleDecode(_qrcode, "ar-t", "1.0"),out temp_ar_opacity))
        {
            temp_ar_opacity = 1.0f;
        }
        //是否朝向摄像机
        if(!int.TryParse(LittleDecode(_qrcode, "ar-toward", "0"),out temp_ar_toward))
        {
            temp_ar_toward = 0;
        }
        //抠像背景色
        string tempBgColorString = LittleDecode(_qrcode, "ar-bg", "-1-1-1");
        if(tempBgColorString.Equals("-1-1-1"))
        {
            temp_ar_bgcolor = new Color(-1, -1, -1);
        }
        else if (!ColorUtility.TryParseHtmlString(tempBgColorString, out temp_ar_bgcolor))
        {
            temp_ar_bgcolor = new Color(-1, -1, -1);
        }
    }
    //解析参数的小函数
    public string LittleDecode(string _parmWholeString,string _parmName,string _default)
    {
        if(!_parmWholeString.Contains(_parmName))
        {
            return _default;
        }
        string result;
        int startIndex = _parmWholeString.IndexOf(_parmName) + _parmName.Length + 1;
        string _parmString = _parmWholeString.Remove(0, startIndex);
        result = _parmWholeString.Remove(0, startIndex);
        int endIndex = _parmString.IndexOf("&") == -1 ? result.Length : _parmString.IndexOf("&") ;
        result = result.Remove(endIndex,result.Length - endIndex);
        return result;
    }

    /// <summary>
    /// 还原短地址
    /// </summary>
    public  string ExpandShortUrl(string shortUrl)
    {
        string nativeUrl = null;
        try
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(shortUrl);
            req.AllowAutoRedirect = false;  // 禁止自动跳转
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            if (response.StatusCode == HttpStatusCode.Found)
                nativeUrl = response.Headers["Location"];
        }
        catch (Exception ex)
        {
            nativeUrl = null;
        }
        ExpandFinish = true;
        return nativeUrl;

    }

    IEnumerator ShowWeb()
    {
        yield return new WaitForSeconds(3.0f);
        if(MainManager.GetInstance.WebPlaneBehavior.gameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            MainManager.GetInstance.WebPlaneBehavior.gameObject.GetComponent<MeshRenderer>().enabled = true;
            MainManager.GetInstance.WebPlaneBehavior.webLoadGameobject.GetComponent<WebLoadAnim>().child.GetComponent<MeshRenderer>().enabled = false;
        }
        showWebCoroutine = null;
    }
}