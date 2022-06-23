mergeInto(LibraryManager.library, {
  Connect: function (id) {
    window.connectAbly(UTF8ToString(id));
  },

  MakeChannel: function() {
    window.makeChannel();
  },

  Publish: function (name, data) {
    window.publishAbly(UTF8ToString(name), UTF8ToString(data));
  },

});