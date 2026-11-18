using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace QGMiniGame
{
    public class QGMiniGameManager : MonoBehaviour
    {
        #region Instance

        private static QGMiniGameManager instance = null;


        public static QGMiniGameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(QGMiniGameManager).Name).AddComponent<QGMiniGameManager>();
                    instance.CollectIndex(-1);
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        #endregion

        #region
        public void CollectIndex(int index)
        {
            QGCollectIndex(index);
        }
        #endregion
        
        // 平台能力
        #region 统一SDK登录

        public void Login(Action<QGCommonResponse<QGLoginBean>> successCallback = null, Action<QGCommonResponse<QGLoginBean>> failCallback = null)
        {
            QGLogin(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }
        #endregion

        #region 用户信息

        public void GetUserInfo(Action<QGCommonResponse<QGUserInfoBean>> successCallback = null, Action<QGCommonResponse<QGUserInfoBean>> failCallback = null)
        {
            QGGetUserInfo(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        #endregion

        #region 统一SDK支付
        public void Pay(QGPayParam param, Action<QGCommonResponse<QGPayBean>> successCallback = null, Action<QGCommonResponse<QGPayBean>> failCallback = null, Action<QGCommonResponse<QGPayBean>> cancelCallback = null, Action<QGCommonResponse<QGPayBean>> completeCallback = null)
        {
            QGPay(JsonUtility.ToJson(param), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(cancelCallback), QGCallBackManager.Add(completeCallback));
        }
        #endregion

        #region vivo支付
        public void Pay(PayParam param, Action<QGCommonResponse<QGPayBean>> successCallback = null, Action<QGCommonResponse<QGPayBean>> failCallback = null, Action<QGCommonResponse<QGPayBean>> cancelCallback = null, Action<QGCommonResponse<QGPayBean>> completeCallback = null)
        {
            QGPay(JsonUtility.ToJson(param), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(cancelCallback), QGCallBackManager.Add(completeCallback));
        }
        #endregion

        #region 统一SDK广告对象获取
        public JsonData GetAdInfo()
        {
            string adInfo = QGGetAdInfo();
            QGLog.Log("adInfo:" + adInfo);
            return JsonMapper.ToObject(adInfo);
        }

        #endregion

        #region 创建Banner广告

        public QGBannerAd CreateBannerAd(QGCreateBannerAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGBannerAd ad = new QGBannerAd(adId);
            QGCreateBannerAd(adId, param.GetAdId(), JsonUtility.ToJson(param.style), param.adIntervals);
            return ad;
        }

        #endregion

        #region 创建插屏广告

        public QGInterstitialAd CreateInterstitialAd(QGCommonAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGInterstitialAd ad = new QGInterstitialAd(adId);
            QGCreateInterstitialAd(adId, param.GetAdId());
            return ad;
        }

        #endregion

        #region 创建激励视频广告

        public QGRewardedVideoAd CreateRewardedVideoAd(QGCommonAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGRewardedVideoAd ad = new QGRewardedVideoAd(adId);
            QGCreateRewardedVideoAd(adId, param.GetAdId());
            return ad;
        }

        #endregion

        #region 创建原生广告

        public QGNativeAd CreateNativeAd(QGCommonAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGNativeAd ad = new QGNativeAd(adId);
            QGCreateNativeAd(adId, param.GetAdId());
            return ad;
        }

        #endregion

        #region 创建模板广告

        public QGCustomAd CreateCustomAd(QGCreateCustomAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGCustomAd ad = new QGCustomAd(adId);
            QGCreateCustomAd(adId, param.GetAdId(), JsonUtility.ToJson(param.style));
            return ad;
        }

        public bool IsShow(string adId)
        {
            return QGIsShow(adId);
        }

        #endregion

        #region 创建横幅广告

        public QGBoxBannerAd CreateBoxBannerAd(QGCommonAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGBoxBannerAd ad = new QGBoxBannerAd(adId);
            QGCreateBoxBannerAd(adId, param.GetAdId());
            return ad;
        }

        #endregion

        #region 创建九宫格广告

        public QGBoxPortalAd CreateBoxPortalAd(QGCreateBoxPortalAdParam param)
        {
            var adId = QGCallBackManager.getKey();
            QGBoxPortalAd ad = new QGBoxPortalAd(adId);
            QGCreateBoxPortalAd(adId, param.GetAdId(), param.image, param.marginTop);
            return ad;
        }

        #endregion

        #region 原生广告曝光

        public void ReportAdShow(string adId, QGNativeReportParam param)
        {
            QGReportAdShow(adId, param.adId);
        }

        public void ReportAdClick(string adId, QGNativeReportParam param)
        {
            QGReportAdClick(adId, param.adId);
        }

        #endregion

        #region 广告通用逻辑
        public void ShowAd(string adId, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGShowAd(adId, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void HideAd(string adId, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGHideAd(adId, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void LoadAd(string adId, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGLoadAd(adId, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void DestroyAd(string adId)
        {
            QGDestroyAd(adId);
        }

        #endregion

        #region V订阅
        public void Subscribe(SubscribeParam param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGSubscribe(param.templateIds, param.clientId, param.userId, param.scene, param.type, param.subDesc, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void GetStatus(Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGGetStatus(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void UnSubscribe(SubscribeParam param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGUnSubscribe(param.templateIds, param.clientId, param.userId, param.scene, param.type, param.subDesc, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void IsRelationExist(SubscribeParam param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGIsRelationExist(param.templateIds, param.clientId, param.userId, param.scene, param.type, param.subDesc, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        #endregion


        // 系统信息
        #region 异步获取系统信息

        public void GetSystemInfo(Action<QGCommonResponse<QGSystemInfo>> successCallback = null, Action<QGCommonResponse<QGSystemInfo>> failCallback = null)
        {
            QGGetSystemInfo(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void SystemInfoCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGSystemInfo>>(msg);
        }

        #endregion

        #region 同步获取系统信息

        public QGSystemInfo GetSystemInfoSync()
        {
            return JsonMapper.ToObject<QGSystemInfo>(QGGetSystemInfoSync());
        }

        #endregion

        #region getWindowInfo 获取窗口信息/  getMenuButtonBoundingClientRect 获取菜单按钮（右上角胶囊按钮）的布局位置信息。坐标信息以屏幕左上角为原点。
        public QGWindowInfo GetWindowInfo()
        {
            return JsonMapper.ToObject<QGWindowInfo>(QGGetWindowInfo());
        }

        public MenuButtonBoundingClientRect GetMenuButtonBoundingClientRect()
        {
            return JsonMapper.ToObject<MenuButtonBoundingClientRect>(QGGetMenuButtonBoundingClientRect());
        }

        #endregion

        #region 系统信息获取 GetManifestInfo、GetProvider
        public void GetManifestInfo(Action<QGCommonResponse<QGManifestInfoRponse>> successCallback = null, Action<QGCommonResponse<QGManifestInfoRponse>> failCallback = null)
        {
            QGGetManifestInfo(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void ManifestInfoCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGManifestInfoRponse>>(msg);
        }

        public string GetProvider()
        {
            return QGGetProvider();
        }

        #endregion

        #region 电量 

        public void GetBatteryInfo(Action<QGCommonResponse<BatteryInfoParam>> successCallback = null, Action<QGCommonResponse<BatteryInfoParam>> failCallback = null, Action<QGCommonResponse<BatteryInfoParam>> completeCallback = null)
        {
            QGGetBatteryInfo(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void GetBatteryInfoCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<BatteryInfoParam>>(msg);
        }

        public string GetBatteryInfoSync()
        {
            return QGGetBatteryInfoSync();
        }
        #endregion

        #region 设备信息 设备号 
        public void GetDeviceId(Action<QGCommonResponse<DeviceIdParam>> successCallback = null, Action<QGCommonResponse<DeviceIdParam>> failCallback = null, Action<QGCommonResponse<DeviceIdParam>> completeCallback = null)
        {
            QGGetDeviceId(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void GetDeviceIdCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<DeviceIdParam>>(msg);
        }
        #endregion

        #region 获取网络类型

        public void GetNetworkType(Action<QGCommonResponse<QGGetNetworkType>> successCallback = null, Action<QGCommonResponse<QGGetNetworkType>> failCallback = null)
        {
            QGGetNetworkType(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }
        public void GetNetworkTypeCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGGetNetworkType>>(msg);
        }

        #endregion

        #region 修改/读取剪贴板内容
        public void SetClipboardData(string data, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGSetClipboardData(data, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void GetClipboardData(Action<QGCommonResponse<string>> successCallback = null, Action<QGCommonResponse<string>> failCallback = null)
        {
            QGGetClipboardData(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }
        public void SetClipboardDataCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGBaseResponse>(msg);
        }
        public void GetClipboardDataCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<string>>(msg);
        }
        #endregion

        #region 帧率设置
        public void SetPreferredFramesPerSecond(int fps)
        {
            QGSetPreferredFramesPerSecond(fps);
        }
        #endregion

        #region SetTimeout
        public void SetTimeout(int times, Action<QGBaseResponse> action = null)
        {
            QGSetTimeout(times, QGCallBackManager.Add(action));
        }

        #endregion

        #region 判断桌面启动 
        public void IsStartupByShortcut(Action<QGIsStartupByShortcutParam> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGIsStartupByShortcut(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void IsStartupByShortcutCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGIsStartupByShortcutParam>(msg);
        }
        #endregion

        #region 生命周期
        public void OnShow(Action<QGOnshowResponse> onShowCallback = null)
        {
            QGOnShow(QGCallBackManager.Add(onShowCallback));
        }

        public void OffShow(Action<QGBaseResponse> onShowCallback = null)
        {
            QGOffShow(QGCallBackManager.Add(onShowCallback));
        }

        public void OnHide(Action<QGBaseResponse> onHideCallback = null)
        {
            QGOnHide(QGCallBackManager.Add(onHideCallback));
        }

        public void OffHide(Action<QGBaseResponse> onHideCallback = null)
        {
            QGOffHide(QGCallBackManager.Add(onHideCallback));
        }

        public void QGOnShowCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGOnshowResponse>(msg, false);
        }

        public void QGOnHideCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGBaseResponse>(msg, false);
        }

        #endregion


        // 系统监听

        #region 监听网络状态变化事件

        public void OnNetworkStatusChange(Action<QGOnNetworkStatus> successCallback = null)
        {
            QGOnNetworkStatusChange(QGCallBackManager.Add(successCallback));
        }

        public void OnNetworkStatusChangeResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGOnNetworkStatus>(msg, false);
        }
        #endregion

        #region 监听qg.onAudioInterruptionBegin

        public void OnAudioInterruptionBegin()
        {
            QGOnAudioInterruptionBegin();
        }

        #endregion

        #region 取消监听qg.onAudioInterruptionBegin

        public void OffAudioInterruptionBegin()
        {
            QGOffAudioInterruptionBegin();
        }

        #endregion

        #region 监听qg.onAudioInterruptionEnd

        public void OnAudioInterruptionEnd()
        {
            QGOnAudioInterruptionEnd();
        }

        #endregion

        #region 取消监听qg.onAudioInterruptionEnd

        public void OffAudioInterruptionEnd()
        {
            QGOffAudioInterruptionEnd();
        }

        #endregion

        #region 监听全局错误事件

        public void OnError()
        {
            QGOnError();
        }

        #endregion

        #region 取消监听全局错误事件

        public void OffError()
        {
            QGOffError();
        }

        #endregion

        #region 屏幕亮度  传感器
        public void GetScreenBrightness(Action<QGCommonResponse<ScreenBrightnessParam>> successCallback = null, Action<QGCommonResponse<ScreenBrightnessParam>> failCallback = null, Action<QGCommonResponse<ScreenBrightnessParam>> completeCallback = null)
        {
            QGGetScreenBrightness(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void GetScreenBrightnessCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<ScreenBrightnessParam>>(msg);
        }

        public void SetScreenBrightness(float param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGSetScreenBrightness(param, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void SetKeepScreenOn(bool param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGSetKeepScreenOn(param, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }
        #endregion

        #region 设备信息 地理位置 
        public void GetLocation(Action<QGCommonResponse<GetLocationParam>> successCallback = null, Action<QGCommonResponse<GetLocationParam>> failCallback = null, Action<QGCommonResponse<GetLocationParam>> completeCallback = null)
        {
            QGGetLocation(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void GetLocationCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<GetLocationParam>>(msg);
        }
        #endregion

        #region 设备信息 加速计 
        public void OnAccelerometerChange(Action<QGCommonResponse<onAccelerometerChangeParam>> successCallback = null)
        {
            QGOnAccelerometerChange(QGCallBackManager.Add(successCallback));
        }

        public void OnAccelerometerChangeCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<onAccelerometerChangeParam>>(msg);
        }

        public void StartAccelerometer(string param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGStartAccelerometer(param, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void StopAccelerometer(Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGStopAccelerometer(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }
        #endregion

        #region 设备信息 罗盘 
        public void OnCompassChange(Action<QGCommonResponse<float>> successCallback = null)
        {
            QGOnCompassChange(QGCallBackManager.Add(successCallback));
        }
        public void OnCompassChangeCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<float>>(msg);
        }

        public void StartCompass(Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGStartCompass(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void StopCompass(Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGStopCompass(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        private Action<string> startDeviceMotionListeningSuccess;
        private Action<string> startDeviceMotionListeningFail;
        private Action<string> stopDeviceMotionListeningSuccess;
        private Action<string> stopDeviceMotionListeningFail;
        public void AddStartDeviceMotionListeningCallBack(Action<string> successCallback = null, Action<string> failCallback = null)
        {
            startDeviceMotionListeningSuccess = successCallback;
            startDeviceMotionListeningFail = failCallback;
        }

        public void AddStopDeviceMotionListeningCallBack(Action<string> successCallback = null, Action<string> failCallback = null)
        {
            stopDeviceMotionListeningSuccess = successCallback;
            stopDeviceMotionListeningFail = failCallback;
        }

        public void StartDeviceMotionListeningSuccess(string msg)
        {
            startDeviceMotionListeningSuccess(msg);
        }

        public void StartDeviceMotionListeningFail(string msg)
        {
            startDeviceMotionListeningFail(msg);
        }

        public void StopDeviceMotionListeningSuccess(string msg)
        {
            stopDeviceMotionListeningSuccess(msg);
        }

        public void StopDeviceMotionListeningFail(string msg)
        {
            stopDeviceMotionListeningFail(msg);
        }

        #endregion


        //交互
        #region 退出游戏
        public void ExitApplication()
        {
            QGExitApplication();
        }
        #endregion

        #region 获取桌面图标是否创建

        public void HasShortcutInstalled(Action<QGCommonResponse<QGShortcutBean>> successCallback = null, Action<QGCommonResponse<QGShortcutBean>> failCallback = null)
        {
            QGHasShortcutInstalled(QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        #endregion

        #region 创建桌面图标

        public void InstallShortcut(string message, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> failCallback = null)
        {
            QGInstallShortcut(message, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        #endregion

        #region 短振动

        public void VibrateShort()
        {
            QGVibrateShort();
        }

        #endregion

        #region 长振动

        public void VibrateLong()
        {
            QGVibrateLong();
        }

        #endregion

        #region 显示对话框

        public void ShowModal(ShowModalParam showModalParam, Action<QGCommonResponse<ShowModalResponse>> successCallback = null, Action<QGCommonResponse<ShowModalResponse>> failCallback = null, Action<QGCommonResponse<ShowModalResponse>> completeCallback = null)
        {
            QGShowModal(JsonMapper.ToJson(showModalParam), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        #endregion

        #region 键盘
        public void ShowKeyboard(KeyboardParam param, Action<QGBaseResponse> successCallback = null, Action<QGBaseResponse> cancelCallback = null, Action<QGBaseResponse> completeCallback = null)
        {
            QGShowKeyboard(JsonUtility.ToJson(param), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(cancelCallback), QGCallBackManager.Add(completeCallback));
        }

        public string OnKeyboardInput(Action<QGKeyboardInputResponse> successCallback = null)
        {
            string inputId = QGCallBackManager.Add(successCallback);
            QGOnKeyboardInput(inputId);
            return inputId;
        }

        public string OnKeyboardConfirm(Action<QGKeyboardInputResponse> successCallback = null)
        {
            string confirmId = QGCallBackManager.Add(successCallback);
            QGOnKeyboardConfirm(confirmId);
            return confirmId;
        }

        public string OnKeyboardComplete(Action<QGKeyboardInputResponse> successCallback = null)
        {
            string completeId = QGCallBackManager.Add(successCallback);
            QGOnKeyboardComplete(completeId);
            return completeId;
        }

        public void OffKeyboardInput(string inputId, Action callback = null)
        {
            QGCallBackManager.responseCallBacks.Remove(inputId);
            callback?.Invoke();
        }

        public void OffKeyboardConfirm(string confirmId, Action callback)
        {
            QGCallBackManager.responseCallBacks.Remove(confirmId);
            callback?.Invoke();
        }

        public void OffKeyboardComplete(string completeId, Action callback)
        {
            QGCallBackManager.responseCallBacks.Remove(completeId);
            callback?.Invoke();
        }

        public void OffKeyboardInput()
        {
            QGOffKeyboardInput();
        }

        public void OffKeyboardConfirm()
        {
            QGOffKeyboardConfirm();
        }

        public void OffKeyboardComplete()
        {
            QGOffKeyboardComplete();
        }

        public void HideKeyboard()
        {
            QGHideKeyboard();
        }
        public void OnKeyboardInputResponseCallback(string msg)
        {
            var res = JsonUtility.FromJson<QGKeyboardInputResponse>(msg);
            var callBack = (Action<QGKeyboardInputResponse>)QGCallBackManager.responseCallBacks[res.callbackId];
            if (callBack != null)
            {
                callBack(res);
            }
        }

        #endregion

        #region sdk
        /// <summary>
        /// 停止纹理下载
        /// </summary>
        public static void StopDownloadTexture()
        {
            QGStopDownloadTexture();
        }
        /// <summary>
        /// 开始纹理下载
        /// </summary>
        public static void StarDownloadTexture()
        {
            QGStarDownloadTexture();
        }

        #endregion

        #region loading
        public void ShowToast(ShowToastParam param)
        {
            QGShowToast(JsonUtility.ToJson(param));
        }

        public void ShowLoading(string title, bool mask)
        {
            QGShowLoading(title, mask);
        }
        public void HideLoading(Action<QGBaseResponse> success = null)
        {
            QGHideLoading(QGCallBackManager.Add(success));
        }

        public void ShowModalCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<ShowModalResponse>>(msg);
        }

        #endregion


        // 多媒体

        #region 播放远程音频

        public QGAudioPlayer PlayAudio(AudioParam param)
        {
            var playerId = QGCallBackManager.getKey();
            Debug.Log("playerId: " + playerId);
            QGAudioPlayer ap = new QGAudioPlayer(playerId);
            QGPlayAudio(playerId, JsonUtility.ToJson(param));
            return ap;
        }

        #endregion

        #region 播放远程视频

        public QGVideoPlayer CreateVideo(VideoParam param)
        {
            var adId = QGCallBackManager.getKey();
            Debug.Log("adId: " + adId);
            QGVideoPlayer ad = new QGVideoPlayer(adId);
            QGCreateVideo(adId, JsonUtility.ToJson(param));
            return ad;
        }

        #endregion

        #region 播放器通用逻辑
        public void PlayMedia(string playerId)
        {
            QGPlayMedia(playerId);
        }

        public void PauseMedia(string playerId)
        {
            QGPauseMedia(playerId);
        }

        public void StopMedia(string playerId)
        {
            QGStopMedia(playerId);
        }

        public void DestroyMedia(string playerId)
        {
            QGDestroyMedia(playerId);
        }

        public void SeekMedia(string playerId, float time)
        {
            QGSeekMedia(playerId, time);
        }

        public void AudioPlayerVolume(string playerId, float volume)
        {
            QGAudioPlayerVolume(playerId, volume);
        }

        public void AudioPlayerLoop(string playerId, bool bl)
        {
            QGAudioPlayerLoop(playerId, bl);
        }
        public float AudioPlayerGetDuration(string playerId)
        {
            return QGAudioPlayerGetDuration(playerId);
        }

        public float AudioPlayerGetCurrentTime(string playerId)
        {
            return QGAudioPlayerGetCurrentTime(playerId);
        }

        public bool AudioPlayerGetPaused(string playerId)
        {
            return QGAudioPlayerGetPaused(playerId);
        }

        public float AudioPlayerGetBuffered(string playerId)
        {
            return QGAudioPlayerGetBuffered(playerId);
        }



        public void VideoPlayerRequestFullScreen(string playerId, int direction)
        {
            QGVideoPlayerRequestFullScreen(playerId, direction);
        }

        public void VideoPlayerExitFullScreen(string playerId)
        {
            QGVideoPlayerExitFullScreen(playerId);
        }

        #endregion

        #region 播放器通用回调
        public void pdOnPlayCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onPlayAction?.Invoke();
            }
        }

        public void pdOnCanPlayCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onCanPlayAction?.Invoke();
            }
        }

        public void pdOnPauseCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onPauseAction?.Invoke();
            }
        }

        public void pdOnStopCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onStopAction?.Invoke();
            }
        }

        public void pdOnEndedCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onEndedAction?.Invoke();
            }
        }

        public void pdOnTimeUpdateCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onTimeUpdateAction?.Invoke();
            }
        }

        public void pdOnErrorCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onErrorAction?.Invoke();
            }
        }

        public void pdOnWaitingCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onWaitingAction?.Invoke();
            }
        }

        public void pdOnSeekingCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onSeekingAction?.Invoke();
            }
        }

        public void pdOnSeekedCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onSeekedAction?.Invoke();
            }
        }
        #endregion

        #region 录音
        public QGRecordManager GetRecorderManager()
        {
            var recordId = QGCallBackManager.getKey();
            Debug.Log("recordId: " + recordId);
            QGRecordManager record = new QGRecordManager(recordId);
            QGGetRecorderManager(recordId);
            return record;
        }

        public void RecorderStart(string recordId, RecordParam recordParam)
        {
            QGRecorderStart(recordId, JsonUtility.ToJson(recordParam));
        }

        public void RecorderPause(string recordId)
        {
            QGRecorderPause(recordId);
        }

        public void RecorderResume(string recordId)
        {
            QGRecorderResume(recordId);
        }

        public void RecorderStop(string recordId)
        {
            QGRecorderStop(recordId);
        }

        public void rdOnStartCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onStartAction?.Invoke();
            }
        }

        public void rdOnResumeCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onResumeAction?.Invoke();
            }
        }

        public void rdOnPauseCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onPauseAction?.Invoke();
            }
        }

        public void rdOnStopCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onStopAction?.Invoke(res);
            }
        }

        public void rdOnFrameRecordedCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onFrameRecordedAction?.Invoke();
            }
        }

        public void rdOnErrorCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var record = QGRecordManager.QGRecords[res.callbackId];
            if (record != null)
            {
                record.onErrorAction?.Invoke();
            }
        }
        #endregion


        // 网络
        #region 下载

        public void DownLoadFile(DownLoadFileParam param, Action<QGCommonResponse<DownLoadFileResponse>> successCallback = null, Action<QGCommonResponse<DownLoadFileResponse>> failCallback = null)
        {
            QGDownLoadFile(JsonUtility.ToJson(param), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void DownloadFileCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<DownLoadFileResponse>>(msg);
        }

        #endregion

        #region 上传

        public void UploadFile(UpLoadFileParam param, Action<QGCommonResponse<UpLoadFileResponse>> successCallback = null, Action<QGCommonResponse<UpLoadFileResponse>> failCallback = null)
        {
            QGUploadFile(JsonUtility.ToJson(param), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }

        public void UploadFileCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<UpLoadFileResponse>>(msg);
        }
        #endregion

        #region 数据云储存

        public void SetUserCloudStorage(UserCloudStorageParam userCloudStorageParam, Action<QGCommonResponse<string>> successCallback = null, Action<QGCommonResponse<string>> failCallback = null, Action<QGCommonResponse<string>> completeCallback = null)
        {
            QGSetUserCloudStorage(JsonUtility.ToJson(userCloudStorageParam), QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void SetUserCloudStorageCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<string>>(msg);
        }

        public void GetUserCloudStorage(string storageKey, Action<QGCommonResponse<UserCloudStorageParam>> successCallback = null, Action<QGCommonResponse<UserCloudStorageParam>> failCallback = null, Action<QGCommonResponse<UserCloudStorageParam>> completeCallback = null)
        {
            QGGetUserCloudStorage(storageKey, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback), QGCallBackManager.Add(completeCallback));
        }

        public void GetUserCloudStorageCallBack(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<UserCloudStorageParam>>(msg);
        }

        public void RemoveUserCloudStorage(string storageKey)
        {
            QGRemoveUserCloudStorage(storageKey);
        }
        #endregion


        // 文件&数据缓存 
        #region 文件
        public string GetQGEnvUSER_DATA_PATH()
        {
            return QGGetEnvUSER_DATA_PATH();
        }
        #endregion
        #region 文件
        public QGFileSystemManager GetFileSystemManager()
        {
            return new QGFileSystemManager();
        }
        #endregion

        #region storage 
        // 数据存储  
        public static void StorageSetItem(string keyName, string keyValue)
        {
            QGStorageSetItem(keyName, keyValue);
        }
        // 数据读取  
        public string StorageGetItem(string keyName)
        {
            return QGStorageGetItem(keyName);
        }
        // 清除数据 
        public static void StorageRemoveItem(string keyName)
        {
            QGStorageRemoveItem(keyName);
        }

        // 调用该方法会清空存储中的所有键名 
        public static void StorageClear()
        {
            QGStorageClear();
        }

        #endregion

        // 旧文件系统
        #region 判断文件是否存在
        public string AccessFile(string uri)
        {
            return QGAccessFile(uri);
        }
        #endregion

        #region 读取文件
        public void ReadFile(QGFileParam param, Action<QGFileResponse> successCallback = null, Action<QGFileResponse> failCallback = null)
        {
            QGReadFile(param.uri, param.encoding, param.position, param.length, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }
        #endregion

        #region 同步读取文件
        public QGFileInfo ReadFileSync(QGFileParam param)
        {
            string msg = QGReadFileSync(param.uri, param.encoding, param.position, param.length);
            QGFileResponse res = JsonUtility.FromJson<QGFileResponse>(msg);
            QGFileInfo fileInfo = new QGFileInfo();
            fileInfo.textStr = res.textStr;
            if (res.encoding != "utf8")
            {
                var fileBuffer = new byte[res.byteLength];
                QGGetFileBuffer(fileBuffer, res.callbackId);
                fileInfo.textData = fileBuffer;
            }
            return fileInfo;
        }
        #endregion

        #region 写入文件
        public void WriteFile(QGFileParam param, Action<QGFileResponse> successCallback = null, Action<QGFileResponse> failCallback = null)
        {
            QGWriteFile(param.uri, param.encoding, param.position, param.textStr, param.textData, param.textData == null ? 0 : param.textData.Length, QGCallBackManager.Add(successCallback), QGCallBackManager.Add(failCallback));
        }
        #endregion

        #region 写入文件同步方法
        public string WriteFileSync(QGFileParam param)
        {
            return QGWriteFileSync(param.uri, param.encoding, param.position, param.textStr, param.textData, param.textData == null ? 0 : param.textData.Length);
        }
        #endregion

        #region unity的PlayerPrefs

        public void StorageSetIntSync(string key, int value)
        {
            QGStorageSetIntSync(key, value);
        }

        public int StorageGetIntSync(string key, int defaultValue)
        {
            return QGStorageGetIntSync(key, defaultValue);
        }

        public void StorageSetStringSync(string key, string value)
        {
            QGStorageSetStringSync(key, value);
        }

        public string StorageGetStringSync(string key, string defaultValue)
        {
            return QGStorageGetStringSync(key, defaultValue);
        }

        public void StorageSetFloatSync(string key, float value)
        {
            QGStorageSetFloatSync(key, value);
        }

        public float StorageGetFloatSync(string key, float defaultValue)
        {
            return QGStorageGetFloatSync(key, defaultValue);
        }

        public void StorageDeleteAllSync()
        {
            QGStorageDeleteAllSync();
        }

        public void StorageDeleteKeySync(string key)
        {
            QGStorageDeleteKeySync(key);
        }

        public bool StorageHasKeySync(string key)
        {
            return QGStorageHasKeySync(key);
        }

        #endregion

        #region 判断当前是否是vivo环境

        public bool IsVivoRuntime()
        {
            return QGIsVivoRuntime();
        }
        #endregion


        // js通用回调
        #region JS回调
        public void LoginResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGLoginBean>>(msg);
        }

        public void GetUserInfoResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGUserInfoBean>>(msg);
        }


        public void ShortcutResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGShortcutBean>>(msg);
        }

        public void DefaultResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGBaseResponse>(msg);
        }

        public void FileSystemResponseCallback(string msg)
        {
            Debug.Log("FileSystemResponseCallback = " + msg);
            QGFileSystemManager.HandleMethodCallback(msg);
        }

        public void FileSystemStatResponseCallback(string msg)
        {
            Debug.Log("FileSystemStatResponseCallback = " + msg);
            QGFileSystemManager.HandleStatCallback(msg);
        }

        public void PayResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGCommonResponse<QGPayBean>>(msg);
        }

        public void ReadFileResponseCallback(string msg)
        {
            if (msg.Contains("utf8"))
            {
                QGCallBackManager.InvokeResponseCallback<QGFileResponse>(msg);
            }
            else
            {
                QGFileResponse response = JsonUtility.FromJson<QGFileResponse>(msg);
                var fileBuffer = new byte[response.byteLength];
                QGGetFileBuffer(fileBuffer, response.callbackId);
                response.textData = fileBuffer;
                var callback = (Action<QGFileResponse>)QGCallBackManager.responseCallBacks[response.callbackId];
                callback(response);
                QGCallBackManager.responseCallBacks.Remove(response.callbackId);
            }

        }

        public void WriteFileResponseCallback(string msg)
        {
            QGCallBackManager.InvokeResponseCallback<QGFileResponse>(msg);
        }

        // 广告通用回调 
        public void AdOnErrorCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null)
            {
                ad.onErrorAction?.Invoke(res);
            }
        }

        public void AdOnLoadCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null)
            {
                ad.onLoadAction?.Invoke();
            }
        }

        public void AdOnCloseCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null)
            {
                ad.onCloseAction?.Invoke();
            }
        }

        public void AdOnHideCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null)
            {
                ad.onHideAction?.Invoke();
            }
        }

        public void AdOnShowCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null && ad is QGBoxPortalAd)
            {
                ((QGBoxPortalAd)ad).onShowAction?.Invoke();
            }
        }

        public void NativeAdOnLoadCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGNativeResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null && ad is QGNativeAd)
            {
                ((QGNativeAd)ad).onLoadNativeAction?.Invoke(res);
            }
        }


        public void RewardedVideoAdOnCloseCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGRewardedVideoResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null && ad is QGRewardedVideoAd)
            {
                ((QGRewardedVideoAd)ad).onCloseRewardedVideoAction?.Invoke(res);
            }
        }

        public void RewardedVideoAdOnRewardCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGRewardedVideoResponse>(msg);
            var ad = QGBaseAd.QGAds[res.callbackId];
            if (ad != null && ad is QGRewardedVideoAd)
            {
                ((QGRewardedVideoAd)ad).onRewardedVideoAction?.Invoke(res);
            }
        }

        #endregion

        #region C#OnTouch调用JS桥接方法
        public void _OnTouchCancelCallback(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var res = JsonMapper.ToObject<OnTouchStartCallbackResult>(msg);
                OnTouchCancelAction?.Invoke(res);
            }
        }
        [DllImport("__Internal")]
        private static extern void QG_OnTouchCancel();
        private Action<OnTouchStartCallbackResult> OnTouchCancelAction;
        public void OnTouchCancel(Action<OnTouchStartCallbackResult> result)
        {
            if (OnTouchCancelAction == null)
            {
                QG_OnTouchCancel();
            }
            OnTouchCancelAction += result;
        }
        [DllImport("__Internal")]
        private static extern void QG_OffTouchCancel();
        public void OffTouchCancel(Action<OnTouchStartCallbackResult> result)
        {
            if (result == null)
            {
                OnTouchCancelAction = null;
            }
            else
            {
                OnTouchCancelAction -= result;
            }
            if (OnTouchCancelAction == null)
            {
                QG_OffTouchCancel();
            }
        }
        public void _OnTouchEndCallback(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var res = JsonMapper.ToObject<OnTouchStartCallbackResult>(msg);
                OnTouchEndAction?.Invoke(res);
            }
        }
        [DllImport("__Internal")]
        private static extern void QG_OnTouchEnd();
        private Action<OnTouchStartCallbackResult> OnTouchEndAction;
        public void OnTouchEnd(Action<OnTouchStartCallbackResult> result)
        {
            if (OnTouchEndAction == null)
            {
                QG_OnTouchEnd();
            }
            OnTouchEndAction += result;
        }
        [DllImport("__Internal")]
        private static extern void QG_OffTouchEnd();
        public void OffTouchEnd(Action<OnTouchStartCallbackResult> result)
        {
            if (result == null)
            {
                OnTouchEndAction = null;
            }
            else
            {
                OnTouchEndAction -= result;
            }
            if (OnTouchEndAction == null)
            {
                QG_OffTouchEnd();
            }
        }
        public void _OnTouchMoveCallback(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var res = JsonMapper.ToObject<OnTouchStartCallbackResult>(msg);
                OnTouchMoveAction?.Invoke(res);
            }
        }
        [DllImport("__Internal")]
        private static extern void QG_OnTouchMove();
        private Action<OnTouchStartCallbackResult> OnTouchMoveAction;
        public void OnTouchMove(Action<OnTouchStartCallbackResult> result)
        {
            if (OnTouchMoveAction == null)
            {
                QG_OnTouchMove();
            }
            OnTouchMoveAction += result;
        }
        [DllImport("__Internal")]
        private static extern void QG_OffTouchMove();
        public void OffTouchMove(Action<OnTouchStartCallbackResult> result)
        {
            if (result == null)
            {
                OnTouchMoveAction = null;
            }
            else
            {
                OnTouchMoveAction -= result;
            }
            if (OnTouchMoveAction == null)
            {
                QG_OffTouchMove();
            }
        }
        public void _OnTouchStartCallback(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var res = JsonMapper.ToObject<OnTouchStartCallbackResult>(msg);
                OnTouchStartAction?.Invoke(res);
            }
        }
        [DllImport("__Internal")]
        private static extern void QG_OnTouchStart();
        private Action<OnTouchStartCallbackResult> OnTouchStartAction;
        public void OnTouchStart(Action<OnTouchStartCallbackResult> result)
        {
            if (OnTouchStartAction == null)
            {
                QG_OnTouchStart();
            }
            OnTouchStartAction += result;
        }
        [DllImport("__Internal")]
        private static extern void QG_OffTouchStart();
        public void OffTouchStart(Action<OnTouchStartCallbackResult> result)
        {
            if (result == null)
            {
                OnTouchStartAction = null;
            }
            else
            {
                OnTouchStartAction -= result;
            }
            if (OnTouchStartAction == null)
            {
                QG_OffTouchStart();
            }
        }
        #endregion


        [DllImport("__Internal")]
        public static extern void QGGetFileBuffer(byte[] d, string c);

        [DllImport("__Internal")]
        private static extern void QGCollectIndex(int index);

        [DllImport("__Internal")]
        private static extern void QGLogin(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGGetUserInfo(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGHasShortcutInstalled(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGInstallShortcut(string m, string s, string f);

        [DllImport("__Internal")]
        private static extern string QGGetAdInfo();

        [DllImport("__Internal")]
        private static extern void QGCreateBannerAd(string a, string p, string s, int i);

        [DllImport("__Internal")]
        private static extern void QGCreateInterstitialAd(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGCreateRewardedVideoAd(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGCreateCustomAd(string a, string p, string s);

        [DllImport("__Internal")]
        private static extern void QGCreateNativeAd(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGCreateBoxBannerAd(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGCreateBoxPortalAd(string a, string p, string i, int m);

        [DllImport("__Internal")]
        private static extern void QGShowAd(string a, string s, string f);

        [DllImport("__Internal")]
        private static extern void QGHideAd(string a, string s, string f);

        [DllImport("__Internal")]
        private static extern void QGLoadAd(string a, string s, string f);

        [DllImport("__Internal")]
        private static extern void QGDestroyAd(string a);

        [DllImport("__Internal")]
        private static extern bool QGIsShow(string a);

        [DllImport("__Internal")]
        private static extern void QGReportAdShow(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGReportAdClick(string a, string p);

        [DllImport("__Internal")]
        private static extern void QGStorageSetIntSync(string k, int v);

        [DllImport("__Internal")]
        private static extern int QGStorageGetIntSync(string k, int d);

        [DllImport("__Internal")]
        private static extern void QGStorageSetStringSync(string k, string v);

        [DllImport("__Internal")]
        private static extern string QGStorageGetStringSync(string k, string d);

        [DllImport("__Internal")]
        private static extern void QGStorageSetFloatSync(string k, float v);

        [DllImport("__Internal")]
        private static extern float QGStorageGetFloatSync(string k, float d);

        [DllImport("__Internal")]
        private static extern void QGStorageDeleteAllSync();

        [DllImport("__Internal")]
        private static extern void QGStorageDeleteKeySync(string k);

        [DllImport("__Internal")]
        private static extern bool QGStorageHasKeySync(string k);

        [DllImport("__Internal")]
        private static extern void QGPay(string p, string s, string f, string c, string o);

        [DllImport("__Internal")]
        private static extern string QGAccessFile(string u);

        [DllImport("__Internal")]
        private static extern void QGReadFile(string u, string e, int p, int l, string s, string f);

        [DllImport("__Internal")]
        private static extern string QGReadFileSync(string u, string e, int p, int l);

        [DllImport("__Internal")]
        private static extern void QGWriteFile(string u, string e, int p, string t, byte[] d, int l, string c, string f);

        [DllImport("__Internal")]
        private static extern string QGWriteFileSync(string u, string e, int p, string t, byte[] d, int l);


        [DllImport("__Internal")]
        private static extern void QGShowKeyboard(string p, string s, string c, string o);

        [DllImport("__Internal")]
        private static extern void QGOnKeyboardInput(string p);

        [DllImport("__Internal")]
        private static extern void QGOnKeyboardConfirm(string p);

        [DllImport("__Internal")]
        private static extern void QGOnKeyboardComplete(string p);

        [DllImport("__Internal")]
        private static extern void QGOffKeyboardInput();

        [DllImport("__Internal")]
        private static extern void QGOffKeyboardConfirm();

        [DllImport("__Internal")]
        private static extern void QGOffKeyboardComplete();

        [DllImport("__Internal")]
        private static extern void QGHideKeyboard();

        [DllImport("__Internal")]
        private static extern void QGExitApplication();

        [DllImport("__Internal")]
        private static extern void QGSubscribe(string t, string c, string u, string s, int h, string l, string success, string fail);

        [DllImport("__Internal")]
        private static extern void QGGetStatus(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGUnSubscribe(string t, string c, string u, string s, int h, string l, string success, string fail);

        [DllImport("__Internal")]
        private static extern void QGIsRelationExist(string t, string c, string u, string s, int h, string l, string success, string fail);

        [DllImport("__Internal")]
        private static extern bool QGIsVivoRuntime();


        [DllImport("__Internal")]
        private static extern void QGSetClipboardData(string p, string s, string f);

        [DllImport("__Internal")]
        private static extern void QGGetClipboardData(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGGetNetworkType(string s, string f);

        [DllImport("__Internal")]
        private static extern void QGOnNetworkStatusChange(string p);

        [DllImport("__Internal")]
        private static extern void QGVibrateShort();

        [DllImport("__Internal")]
        private static extern void QGVibrateLong();

        [DllImport("__Internal")]
        private static extern void QGGetSystemInfo(string s, string f);

        [DllImport("__Internal")]
        private static extern string QGGetSystemInfoSync();

        [DllImport("__Internal")]
        private static extern string QGGetWindowInfo();

        [DllImport("__Internal")]
        private static extern string QGGetMenuButtonBoundingClientRect();

        [DllImport("__Internal")]
        private static extern void QGShowModal(string a, string b, string c, string d);

        [DllImport("__Internal")]
        private static extern void QGDownLoadFile(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGUploadFile(string a, string b, string c);

        [DllImport("__Internal")]
        private static extern void QGCreateVideo(string a, string b);

        [DllImport("__Internal")]
        private static extern void QGPlayAudio(string a, string b);

        [DllImport("__Internal")]
        private static extern void QGPlayMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGPauseMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGStopMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGDestroyMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGSeekMedia(string a, float time);

        [DllImport("__Internal")]
        private static extern void QGAudioPlayerVolume(string a, float b);
        [DllImport("__Internal")]
        private static extern void QGAudioPlayerLoop(string a, bool b);
        [DllImport("__Internal")]
        private static extern float QGAudioPlayerGetDuration(string a);
        [DllImport("__Internal")]
        private static extern float QGAudioPlayerGetCurrentTime(string a);
        [DllImport("__Internal")]
        private static extern bool QGAudioPlayerGetPaused(string a);
        [DllImport("__Internal")]
        private static extern float QGAudioPlayerGetBuffered(string a);

        [DllImport("__Internal")]
        private static extern void QGVideoPlayerRequestFullScreen(string a, int b);
        [DllImport("__Internal")]
        private static extern void QGVideoPlayerExitFullScreen(string a);

        [DllImport("__Internal")]
        private static extern void QGOnAudioInterruptionBegin();

        [DllImport("__Internal")]
        private static extern void QGOffAudioInterruptionBegin();

        [DllImport("__Internal")]
        private static extern void QGOnAudioInterruptionEnd();

        [DllImport("__Internal")]
        private static extern void QGOffAudioInterruptionEnd();

        [DllImport("__Internal")]
        private static extern void QGOnError();

        [DllImport("__Internal")]
        private static extern void QGOffError();

        [DllImport("__Internal")]
        private static extern void QGShowToast(string a);

        [DllImport("__Internal")]
        private static extern void QGGetManifestInfo(string a, string b);

        [DllImport("__Internal")]
        private static extern string QGGetProvider();
        [DllImport("__Internal")]
        private static extern void QGSetPreferredFramesPerSecond(int a);
        [DllImport("__Internal")]
        private static extern void QGShowLoading(string a, bool mask);
        [DllImport("__Internal")]
        private static extern void QGHideLoading(string a);
        [DllImport("__Internal")]
        private static extern void QGSetTimeout(int a, string b);

        [DllImport("__Internal")]
        private static extern void QGSetUserCloudStorage(string a, string b, string c, string d);
        [DllImport("__Internal")]
        private static extern void QGGetUserCloudStorage(string k, string b, string c, string d);
        [DllImport("__Internal")]
        private static extern void QGRemoveUserCloudStorage(string k);

        [DllImport("__Internal")]
        private static extern void QGGetBatteryInfo(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern string QGGetBatteryInfoSync();
        [DllImport("__Internal")]
        private static extern void QGGetDeviceId(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGGetScreenBrightness(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGSetScreenBrightness(float a, string b, string c, string d);
        [DllImport("__Internal")]
        private static extern void QGSetKeepScreenOn(bool a, string b, string c, string d);
        [DllImport("__Internal")]
        private static extern void QGGetLocation(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGOnAccelerometerChange(string a);
        [DllImport("__Internal")]
        private static extern void QGStartAccelerometer(string a, string b, string c, string d);
        [DllImport("__Internal")]
        private static extern void QGStopAccelerometer(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGOnCompassChange(string a);
        [DllImport("__Internal")]
        private static extern void QGStartCompass(string a, string b, string c);
        [DllImport("__Internal")]
        private static extern void QGStopCompass(string a, string b, string c);

        [DllImport("__Internal")]
        private static extern void QGIsStartupByShortcut(string a, string b);

        [DllImport("__Internal")]
        private static extern void QGGetRecorderManager(string a);
        [DllImport("__Internal")]
        private static extern void QGRecorderStart(string a, string b);
        [DllImport("__Internal")]
        private static extern void QGRecorderPause(string a);
        [DllImport("__Internal")]
        private static extern void QGRecorderResume(string a);
        [DllImport("__Internal")]
        private static extern void QGRecorderStop(string a);
        [DllImport("__Internal")]
        private static extern void QGOnShow(string a);
        [DllImport("__Internal")]
        private static extern void QGOffShow(string a);
        [DllImport("__Internal")]
        private static extern void QGOnHide(string a);
        [DllImport("__Internal")]
        private static extern void QGOffHide(string a);

        // 数据存储
        [DllImport("__Internal")]
        private static extern void QGStorageSetItem(string k, string v);

        [DllImport("__Internal")]
        private static extern string QGStorageGetItem(string k);

        [DllImport("__Internal")]
        private static extern void QGStorageRemoveItem(string k);
        [DllImport("__Internal")]
        private static extern void QGStorageClear();

        [DllImport("__Internal")]
        private static extern string QGGetEnvUSER_DATA_PATH();

        [DllImport("__Internal")]
        private static extern void QGStopDownloadTexture();
        [DllImport("__Internal")]
        private static extern void QGStarDownloadTexture();
    }
}
