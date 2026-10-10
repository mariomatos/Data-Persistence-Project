mergeInto(LibraryManager.library, {
  SyncPersistentData: function () {
    FS.syncfs(false, function (error) {
      if (error) {
        console.error("Could not synchronize persistent data to IndexedDB.", error);
      }
    });
  }
});
