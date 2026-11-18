/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
const unityNamespace = {
  bundleHashLength: $BUNDLE_HASH_LENGTH, // 自定义bundle中的hash长度
  uintyColorSpace: '$UNITY_COLORSPACE', //Color Space: Gamma、Linear、Uninitialized(未初始化的颜色空间)
  convertPluginVersion: '$PLUGIN_VERSION',
  texturesHashLength: $TEXTURE_HASH_LENGTH, // 纹理中的hash长度
  texturesPath: '$TEXTURES_PATH', // 纹理存放路径
  needCacheTextures: $NEED_CACHE_TEXTURES, // 是否需要缓存纹理,
};

window.GameGlobal.unityNamespace = unityNamespace;
