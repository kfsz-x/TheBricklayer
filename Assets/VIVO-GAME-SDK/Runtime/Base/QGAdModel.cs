using System;
using UnityEngine;


namespace QGMiniGame
{

    [Serializable]
    public class QGNativeResponse : QGBaseResponse
    {
        [SerializeField] public NativeItemBean[] adList; // 原生广告数据列表
    }

    [Serializable]
    public class QGRewardedVideoResponse : QGBaseResponse
    {
        [SerializeField] public bool isEnded; // 视频是否是在用户完整观看的情况下被关闭的，true 表示用户是在视频播放完以后关闭的视频，false 表示用户在视频播放过程中关闭了视频
    }

    [Serializable]
    public class NativeItemBean
    {
        public string adId; // 广告标识，用来上报曝光与点击
        public string title; // 广告标题
        public string desc; // 广告描述
        public string icon; // 推广应用的Icon图标
        public string[] imgUrlList; // 广告图片，建议使用该图片资源
        public string logoUrl; // 广告标签图片
        public string clickBtnTxt; // 点击按钮文本描述
        public int creativeType; // 获取广告类型，取值说明：0：混合
        public int interactionType; // 获取广告点击之后的交互类型，取值说明： 1：网址类 2：应用下载类 8：快应用生态应用
    }

    public class QGNativeReportParam
    {
        public string adId; //广告位id
    }

    public class QGCommonAdParam
    {
        public string posId; //广告位id，与下方的adUnitId填写一个即可
        public string adUnitId; //广告位id（必填 非常重要）

        public string GetAdId()
        {
            // 如果 posId 不为空就用它，否则用 adUnitId
            return !string.IsNullOrEmpty(posId) ? posId : adUnitId;
        }
    }

    public class QGCreateCustomAdParam : QGCommonAdParam
    {
        public Style style; // 广告位置
    }

    public class QGCreateBannerAdParam : QGCommonAdParam
    {
        public int adIntervals; // 刷新时间
        public Style style; // 广告位置
    }
	
	public class QGCreateBoxPortalAdParam : QGCommonAdParam
    {
        public string image; //替换悬浮icon的默认图标，支持本地和网络图片 注：图片宽度不能超过300px，否则会以宽度300进行缩放
        public int marginTop; //盒子九宫格广告悬浮Icon相对顶部的距离，单位：px，不同分辨率自行调整
    }

    public class Style
    {
        public int left = -1;
        public int top = -1;
        public int width = -1;
        public string gravity = "center";
    }
}
