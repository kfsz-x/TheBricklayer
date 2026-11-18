const sdk = {
  formatIdentifier(identifier) {
    if (identifier > 0 && Math.abs(identifier) < 2147483648) {
      return Math.round(identifier);
    }
    const { identifierCache } = GameGlobal;
    // eslint-disable-next-line no-restricted-syntax
    for (const key in identifierCache) {
      if (identifierCache[key] && identifierCache[key].key === identifier) {
        return identifierCache[key].value;
      }
    }
    let value = parseInt(Math.random() * 2147483648, 10);
    // eslint-disable-next-line no-loop-func
    while (identifierCache.some((v) => v.value === value)) {
      value += 1;
    }
    identifierCache.push({
      key: identifier,
      value,
    });
    if (identifierCache.length > 30) {
      identifierCache.shift();
    }
    return value;
  },
  formatTouchEvent(v) {
    // 这里将坐标转换为Unity的坐标
    return {
      identifier: this.formatIdentifier(v.identifier),
      clientX: v.clientX * devicePixelRatio,
      clientY: (window.innerHeight - v.clientY) * devicePixelRatio,
      pageX: v.pageX * devicePixelRatio,
      pageY: (window.innerHeight - v.pageY) * devicePixelRatio,
    };
  },
  QG_OnTouchCancel() {
    this.OnTouchCancelList = this.OnTouchCancelList || [];
    const callback = (res) => {
      const touches = res.touches.map((v) => this.formatTouchEvent(v));
      const resStr = JSON.stringify({
        touches,
        timeStamp: parseInt(res.timeStamp, 10),
        changedTouches: res.changedTouches.map((v) => this.formatTouchEvent(v)),
      });
      unityInstance.SendMessage(
        'QGMiniGameManager',
        '_OnTouchCancelCallback',
        resStr
      );
    };
    this.OnTouchCancelList.push(callback);
    qg.onTouchCancel(callback);
  },
  QG_OffTouchCancel() {
    (this.OnTouchCancelList || []).forEach((v) => {
      qg.offTouchCancel(v);
    });
  },
  QG_OnTouchEnd() {
    this.OnTouchEndList = this.OnTouchEndList || [];
    const callback = (res) => {
      const touches = res.touches.map((v) => this.formatTouchEvent(v));
      const resStr = JSON.stringify({
        touches,
        timeStamp: parseInt(res.timeStamp, 10),
        changedTouches: res.changedTouches.map((v) => this.formatTouchEvent(v)),
      });
      unityInstance.SendMessage(
        'QGMiniGameManager',
        '_OnTouchEndCallback',
        resStr
      );
    };
    this.OnTouchEndList.push(callback);
    qg.onTouchEnd(callback);
  },
  QG_OffTouchEnd() {
    (this.OnTouchEndList || []).forEach((v) => {
      qg.offTouchEnd(v);
    });
  },
  QG_OnTouchMove() {
    this.OnTouchMoveList = this.OnTouchMoveList || [];
    const callback = (res) => {
      const touches = res.touches.map((v) => this.formatTouchEvent(v));
      const resStr = JSON.stringify({
        touches,
        timeStamp: parseInt(res.timeStamp, 10),
        changedTouches: res.changedTouches.map((v) => this.formatTouchEvent(v)),
      });
      unityInstance.SendMessage(
        'QGMiniGameManager',
        '_OnTouchMoveCallback',
        resStr
      );
    };
    this.OnTouchMoveList.push(callback);
    qg.onTouchMove(callback);
  },
  QG_OffTouchMove() {
    (this.OnTouchMoveList || []).forEach((v) => {
      qg.offTouchMove(v);
    });
  },
  QG_OnTouchStart() {
    this.OnTouchStartList = this.OnTouchStartList || [];
    const callback = (res) => {
      const touches = res.touches.map((v) => this.formatTouchEvent(v));
      const resStr = JSON.stringify({
        touches,
        timeStamp: parseInt(res.timeStamp, 10),
        changedTouches: res.changedTouches.map((v) => this.formatTouchEvent(v)),
      });
      unityInstance.SendMessage(
        'QGMiniGameManager',
        '_OnTouchStartCallback',
        resStr
      );
    };
    this.OnTouchStartList.push(callback);
    qg.onTouchStart(callback);
  },
  QG_OffTouchStart() {
    (this.OnTouchStartList || []).forEach((v) => {
      qg.offTouchStart(v);
    });
  },
};

for (const [key, fn] of Object.entries(sdk)) {
  window.wasmsdk[key] = fn;
}
