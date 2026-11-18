window.wasmsdk = window.wasmsdk || {};
const LoadSubWasmStep = {
  START: '$load_subwasm_start$',
  END: '$load_subwasm_end$',
};
window.wasmsdk.getWasmFuncs = function () {
  return $wasmFuncs$;
};
window.wasmsdk.isOpenWasmSplit = function () {
  return '$isOpenWasmSplit$';
};
function updateSubWasmLoading(step, res) {
  if (qg.jsCallback) {
    qg.jsCallback({ eventId: step, data: { res } });
  } else if (step === LoadSubWasmStep.START) {
    qg.showToast({ message: 'wasm loading...' });
  } else if (step === LoadSubWasmStep.END) {
    qg.showToast({ message: res ? 'end~' : 'failed~' });
  }
}
function getAssistWasm() {
  try {
    const fs = qg.getFileSystemManager();
    if (UnityLoader.UnityInfo.isOpenVBrotli === '1') {
      return fs.readCompressedFileSync({
        filePath: '/unitySubWasmPkg/subwebgl.wasm.code.unityweb.br',
        compressionAlgorithm: 'br',
      });
    } else {
      return fs.readFileSync(
        '/unitySubWasmPkg/subwebgl.wasm.code.unityweb',
        'binary'
      );
    }
  } catch (err) {
    return;
  }
}
function loadSubpackagePromise(name) {
  return new Promise((resolve, reject) => {
    qg.loadSubpackage({
      name,
      success: resolve,
      fail: reject,
    });
  });
}
/// ---------  loading sub wasm start ----------------
window.wasmsdk.loadSubWasm = async function () {
  if (window.wasmsdk.isOpenWasmSplit() !== 'true') {
    console.log('[unitysub] loadSubWasm not open');
    return;
  }
  if (!window.wasmsdk.loadSubWasm._promise) {
    console.log('[unitysub] loadSubWasm assist wasm instantiate prepare');
    window.wasmsdk.loadSubWasm._promise = (async () => {
      try {
        console.log('[unitysub] loadSubWasm assist wasm instantiate start');
        updateSubWasmLoading(LoadSubWasmStep.START, '');
        await loadSubpackagePromise('unitySubWasmPkg');
        const assistWasm = getAssistWasm();
        if (!assistWasm || !assistWasm.data) {
          throw new Error('Read AssistWasm File Failed');
        }
        const bytes = assistWasm.data;
        const importObject = {
          primary: window['unityInstance'].Module['asm'],
        };
        await WebAssembly.instantiate(bytes, importObject);
        console.log('[unitysub] loadSubWasm assist wasm instantiate success!');
        updateSubWasmLoading(LoadSubWasmStep.END, true);
        return true;
      } catch (error) {
        console.warn('[unitysub] loadSubWasm assist wasm error', error);
        updateSubWasmLoading(LoadSubWasmStep.END, false);
        window.wasmsdk.loadSubWasm._promise = null;
        throw error; // 重新抛出错误
      }
    })();
  } else {
    console.log('[unitysub] loadSubWasm already loading, reusing promise...');
  }
  // 3. 处理回调兼容性
  return window.wasmsdk.loadSubWasm._promise;
};
window.wasmsdk.loadSubWasm._promise = null;
/// ---------  loading sub wasm end ----------------
window.wasmsdk.appId='$appId$';
window.wasmsdk.wasmHash='$wasmHash$';
window.wasmsdk.url='https://quickgamecp.vivo.com.cn';
window.wasmsdk.collectAPI='$collectAPI$'
window.wasmsdk._alreadyIndexs;
window.wasmsdk._ccIndexs = [];
window.wasmsdk._wasmUpdateId;
function updateAlreadyIndexs() {
  if (window.wasmsdk._alreadyIndexs) {
    return;
  }
  window.wasmsdk._alreadyIndexs = [];
  const url =
    window.wasmsdk.url +
    '/wasm/getfuncs?appId=' +
    window.wasmsdk.appId +
    '&wasmHash=' +
    window.wasmsdk.wasmHash;
  const xhr = new XMLHttpRequest();
  xhr.open('GET', url, true);
  xhr.responseType = 'json';
  xhr.onload = function () {
    if (xhr.status === 200) {
      const res = xhr.response;
      const message = res.message;
      const code = res.code;
      if (code !== '200') {
        console.error('server error code:', code, message);
        return;
      }
      const data = res.data;
      if (!data || !data.funcs) {
        console.error('server error data invalid:', data);
        return;
      }
      if (!(data.funcs instanceof Array)) {
        console.error('server error data.funcs not Array:', data.funcs);
        return;
      }
      window.wasmsdk._alreadyIndexs = data.funcs;
    } else {
      console.error('http error:', xhr.status);
    }
  };
  xhr.send(null);
}
let lastnewfc = 0;
let lastcurfc = 0;
function updateCollectFuncs() {
  let curfc = window.wasmsdk._ccIndexs.length;
  let newfc = 0;
  if (
    window.wasmsdk._ccIndexs &&
    window.wasmsdk._alreadyIndexs &&
    window.wasmsdk._alreadyIndexs.length > 0
  ) {
    const baseSet = new Set(window.wasmsdk._alreadyIndexs);
    let count = 0;
    for (const x of window.wasmsdk._ccIndexs) {
      if (!baseSet.has(x)) {
        count++;
      }
    }
    newfc = count;
  }
  if (newfc > 0 && newfc > lastnewfc) {
    lastnewfc = newfc;
    syncPanel();
  } else if (curfc > 0 && curfc > lastcurfc) {
    lastcurfc = curfc;
    syncPanel();
  }
  qg.jsCallback({
    eventId: '$wasm_collect_funcs$',
    data: {
      funcs: curfc,
      newf: newfc,
      total: window.wasmsdk.getWasmFuncs(),
    },
  });
}
window.wasmsdk.QGCollectIndex = function (index) {
  if (index < 0) {
    return;
  }
  if (!window.wasmsdk._wasmUpdateId) {
    updateAlreadyIndexs();
    window.wasmsdk._wasmUpdateId = setInterval(updateCollectFuncs, 2000);
    console.log('WasmSplit start updateCollectFuncs');
  }
  if (!window.wasmsdk._ccIndexs.includes(index)) {
    window.wasmsdk._ccIndexs.push(index);
  }
};
window.wsUpkeep = function (cb) {
  const url = window.wasmsdk.url + '/wasm/uploadkeep';
  const onload = function (xhr) {
    if (xhr.status >= 200 && xhr.status < 300) {
      if (typeof cb === 'function') {
        clearInterval(window.wasmsdk._wasmUpdateId);
        window.wasmsdk._wasmUpdateId = null;
        window.wasmsdk._alreadyIndexs = null;
        cb(true, '上传成功');
      } else {
        console.log('Data sent successfully');
      }
    } else {
      if (typeof cb === 'function') {
        cb(false, 'Failed to send data with status:' + xhr.status);
      } else {
        console.error('Failed to send data with status:', xhr.status);
      }
    }
  };
  syncSplitKeepFunc(url, lastcurfc, lastnewfc, onload);
};
function syncPanel() {
  const url = window.wasmsdk.collectAPI;
  const onload = function (xhr) {
    console.log('syncPanel onload', xhr.status);
  };
  syncSplitKeepFunc(url, lastcurfc, lastnewfc, onload);
}
function syncSplitKeepFunc(url, curfc, newfc, onload) {
  var sendMsg = {
    appid: window.wasmsdk.appId,
    data: window.wasmsdk._ccIndexs,
    wasmHash: window.wasmsdk.wasmHash,
    newfc: newfc,
    curfc: curfc,
  };
  console.log('js wsUpkeep called', sendMsg);
  var xhr = new XMLHttpRequest();
  xhr.open('POST', url, true);
  xhr.setRequestHeader('Content-Type', 'application/json');
  xhr.onload = function () {
    onload(xhr);
  };
  xhr.onerror = function () {
    console.error('Error occurred during the transaction');
  };
  xhr.send(JSON.stringify(sendMsg));
}