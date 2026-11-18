mergeInto(LibraryManager.library, {
  glGenTextures: function (n, textures) {
    for (var i = 0; i < n; i++) {
      var texture = GLctx.createTexture();
      if (!texture) {
        GL.recordError(1282);
        while (i < n) HEAP32[(textures + i++ * 4) >> 2] = 0;
        return;
      }
      var id = GL.getNewId(GL.textures);
      texture.name = id;
      GL.textures[id] = texture;
      window._lastTextureId = id;
      HEAP32[(textures + i * 4) >> 2] = id;
    }
  },
  glBindTexture: function (target, texture) {
    window._lastBoundTexture = texture;
    GLctx.bindTexture(target, texture ? GL.textures[texture] : null);
  },
  QG_OnTouchCancel: function () {
    window.wasmsdk.QG_OnTouchCancel();
  },
  QG_OffTouchCancel: function () {
    window.wasmsdk.QG_OffTouchCancel();
  },
  QG_OnTouchEnd: function () {
    window.wasmsdk.QG_OnTouchEnd();
  },
  QG_OffTouchEnd: function () {
    window.wasmsdk.QG_OffTouchEnd();
  },
  QG_OnTouchMove: function () {
    window.wasmsdk.QG_OnTouchMove();
  },
  QG_OffTouchMove: function () {
    window.wasmsdk.QG_OffTouchMove();
  },
  QG_OnTouchStart: function () {
    window.wasmsdk.QG_OnTouchStart();
  },
  QG_OffTouchStart: function () {
    window.wasmsdk.QG_OffTouchStart();
  },
});
