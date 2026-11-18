const assert = require('assert');
const fs = require('fs');
const path = require('path');
const vm = require('vm');

const gameScriptPath = path.resolve(__dirname, '../Default/src/game.js');
const gameScript = fs.readFileSync(gameScriptPath, 'utf8');

const qg = {
  env: { USER_DATA_PATH: '/tmp' },
  getProvider: () => 'vivo',
  getPerformance: () => ({ now: () => 1 }),
  reportGameInfo: () => {},
};

const UnityLoader = {
  EnvConfig: { getConfig: () => undefined },
  UnityInfo: {
    unityVersion: { string: 'test' },
    unityPluginVersion: 'test',
    unityCodeSize: 0,
    unityDataSize: 0,
  },
  printLog: () => {},
  printError: () => {},
  instantiate: (_path, options) => {
    options.onProgress(null, 1);
    return { Module: {} };
  },
};

const windowObject = { qg, UnityLoader, wasmsdk: {} };
const context = {
  console,
  qg,
  UnityLoader,
  window: windowObject,
  GameGlobal: {},
  $WEBGL_VERSION: 2,
  require: request => request === './md5.js' ? { hex_md5: value => value } : {},
  setInterval: () => 1,
  clearInterval: () => {},
  setTimeout: callback => callback(),
  XMLHttpRequest: function XMLHttpRequest() {},
};

assert.doesNotThrow(
  () => vm.runInNewContext(gameScript, context, { filename: gameScriptPath }),
  '未启用 WASM 分包时，启动流程不应调用不存在的 isOpenWasmSplit'
);

console.log('game-load-sub-wasm compatibility test passed');
