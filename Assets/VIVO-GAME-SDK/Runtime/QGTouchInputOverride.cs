using QGMiniGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.Touch;
class TouchData
{
	public Touch touch;
	public long timeStamp;
}

/**
 * 由于Unity WebGL发布的多点触控存在问题, 导致在vivo中多点触控存在粘连的情况
 * 所以需要使用qg的触控接口重新覆盖Unity的BaseInput关于触控方面的接口
 * 通过设置StandaloneInputModule.inputOverride的方式来实现
*/
[RequireComponent(typeof(StandaloneInputModule))]
public class QGTouchInputOverride : BaseInput
{
	private readonly List<TouchData> _touches = new List<TouchData>();
	private StandaloneInputModule _standaloneInputModule = null;

	protected override void Awake()
	{
		base.Awake();
		_standaloneInputModule = GetComponent<StandaloneInputModule>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		InitWechatTouchEvents();
		if (_standaloneInputModule)
		{
			_standaloneInputModule.inputOverride = this;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UnregisterWechatTouchEvents();
		if (_standaloneInputModule)
		{
			_standaloneInputModule.inputOverride = null;
		}
	}

	private void InitWechatTouchEvents()
	{
        RegisterWechatTouchEvents();
    }
	private void RegisterWechatTouchEvents()
	{
		QG.OnTouchStart(OnQGTouchStart);
        QG.OnTouchMove(OnQGTouchMove);
        QG.OnTouchEnd(OnQGTouchEnd);
        QG.OnTouchCancel(OnQGTouchCancel);
	}

	private void UnregisterWechatTouchEvents()
	{
        QG.OffTouchStart(OnQGTouchStart);
		QG.OffTouchMove(OnQGTouchMove);
		QG.OffTouchEnd(OnQGTouchEnd);
		QG.OffTouchCancel(OnQGTouchCancel);
	}

	private void OnQGTouchStart(OnTouchStartCallbackResult touchEvent)
	{
		foreach (var qgTouch in touchEvent.changedTouches)
		{
			var data = FindOrCreateTouchData(qgTouch.identifier);
			data.touch.phase = TouchPhase.Began;
			data.touch.position = new Vector2(qgTouch.clientX, qgTouch.clientY);
			data.touch.rawPosition = data.touch.position;
			data.timeStamp = touchEvent.timeStamp;
			// Debug.Log($"OnQGTouchStart:{qgTouch.identifier}, {data.touch.phase}");
		}
	}

	private void OnQGTouchMove(OnTouchStartCallbackResult touchEvent)
	{
		foreach (var qgTouch in touchEvent.changedTouches)
		{
			var data = FindOrCreateTouchData(qgTouch.identifier);
			UpdateTouchData(data, new Vector2(qgTouch.clientX, qgTouch.clientY), touchEvent.timeStamp, TouchPhase.Moved);
		}
	}

	private void OnQGTouchEnd(OnTouchStartCallbackResult touchEvent)
	{
		foreach (var qgTouch in touchEvent.changedTouches)
		{
			TouchData data = FindTouchData(qgTouch.identifier);
			if (data == null)
            {
				Debug.LogError($"OnQGTouchEnd, error identifier:{qgTouch.identifier}");
				return;
			}
			if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
            {
				Debug.LogWarning($"OnQGTouchEnd, error phase:{qgTouch.identifier}, phase:{data.touch.phase}");
			}
			// Debug.Log($"OnQGTouchEnd:{qgTouch.identifier}");
			UpdateTouchData(data, new Vector2(qgTouch.clientX, qgTouch.clientY), touchEvent.timeStamp, TouchPhase.Ended);
		}
	}

	private void OnQGTouchCancel(OnTouchStartCallbackResult touchEvent)
	{
		foreach (var qgTouch in touchEvent.changedTouches)
		{
			TouchData data = FindTouchData(qgTouch.identifier);
			if (data == null)
			{
				Debug.LogError($"OnQGTouchCancel, error identifier:{qgTouch.identifier}");
				return;
			}
			if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
			{
				Debug.LogWarning($"OnQGTouchCancel, error phase:{qgTouch.identifier}, phase:{data.touch.phase}");
			}
			// Debug.Log($"OnQGTouchCancel:{qgTouch.identifier}");
			UpdateTouchData(data, new Vector2(qgTouch.clientX, qgTouch.clientY), touchEvent.timeStamp, TouchPhase.Canceled);
		}
	}

	private void LateUpdate()
	{
		foreach (var t in _touches) 
		{
            if (t.touch.phase == TouchPhase.Began)
            {
				t.touch.phase = TouchPhase.Stationary;
            }
		}
		RemoveEndedTouches();
	}
	private void RemoveEndedTouches()
	{

		if (_touches.Count > 0)
		{
			_touches.RemoveAll(touchData =>
			{
				var touch = touchData.touch;
				return (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
			});
		}
	}

	private TouchData FindTouchData(int identifier)
	{
		foreach (var touchData in _touches)
		{
			var touch = touchData.touch;
			if (touch.fingerId == identifier)
			{
				return touchData;
			}
		}
		return null;
	}

	private TouchData FindOrCreateTouchData(int identifier)
	{
		var touchData = FindTouchData(identifier);
		if (touchData != null) return touchData;
		 
		var data = new TouchData();
		data.touch.pressure = 1.0f;
		data.touch.maximumPossiblePressure = 1.0f;
		data.touch.type = TouchType.Direct;
		data.touch.tapCount = 1;
		data.touch.fingerId = identifier;
		data.touch.radius = 0;
		data.touch.radiusVariance = 0;
		data.touch.altitudeAngle = 0;
		data.touch.azimuthAngle = 0;
		data.touch.deltaTime = 0;
		_touches.Add(data);
		return data;
	}

	private static void UpdateTouchData(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
	{
		data.touch.phase = phase;
		data.touch.deltaPosition = pos - data.touch.position;
		data.touch.position = pos;
		data.touch.deltaTime = (timeStamp - data.timeStamp) / 1000000.0f;
		
	}

#if !UNITY_EDITOR
	public override bool touchSupported
	{
		get
		{
			return true;
		}
	}
	public override bool mousePresent
	{
		get
		{
			return false;
		}
	}
	public override int touchCount
	{
		get { return _touches.Count; }
	}

	public override Touch GetTouch(int index)
	{
	    // Debug.LogError($"GetTouch touchCount:{touchCount}, index:{index}, touch:{_touches[index].touch.fingerId}, {_touches[index].touch.phase}");
		return _touches[index].touch;
	}
	
#endif
}
