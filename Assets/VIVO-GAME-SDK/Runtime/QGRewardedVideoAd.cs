using UnityEngine;
using System;



namespace QGMiniGame
{
    public class QGRewardedVideoAd : QGBaseAd
    {

        public Action<QGRewardedVideoResponse> onCloseRewardedVideoAction;
        public Action<QGRewardedVideoResponse> onRewardedVideoAction;

        public QGRewardedVideoAd(string adId) : base(adId)
        {

        }

        public override void Hide(Action<QGBaseResponse> success = null, Action<QGBaseResponse> failed = null)
        {
            QGLog.LogWarning("QGRewardedVideoAd no Hide Function");
        }

        public void Load(Action<QGBaseResponse> success = null, Action<QGBaseResponse> failed = null)
        {
            QGMiniGameManager.Instance.LoadAd(adId, success, failed);
        }


         public void OnReward(Action<QGRewardedVideoResponse> onReward)
        {
            onRewardedVideoAction += onReward;
        }
        public void OffReward(Action<QGRewardedVideoResponse> offReward)
        {
            onRewardedVideoAction -= offReward;
        }


        public void OnClose(Action<QGRewardedVideoResponse> onClose)
        {
            onCloseRewardedVideoAction += onClose;
        }


        public void OffClose(Action<QGRewardedVideoResponse> offClose)
        {
            onCloseRewardedVideoAction -= offClose;
        }
    }
}
