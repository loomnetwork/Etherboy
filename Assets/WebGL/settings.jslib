var EtherboySettingsLib = {
  // NOTE: ESL will be undefined if the Use pre-built Engine option is enabled!
  $ESL: {
    allocateStringBuffer: function (str) {
      const bufferSize = lengthBytesUTF8(str) + 1;
      const buffer = _malloc(bufferSize);
      stringToUTF8(str, buffer, bufferSize);
      return buffer;
    }
  },

  GetDAppChainConfigFromHostPage: function () {
    const cfg = window.LOOM_SETTINGS.dappchain;
    const cfgStr = JSON.stringify({
      write_host: cfg.writeUrl,
      read_host: cfg.readUrl
    });
    return ESL.allocateStringBuffer(cfgStr);
  },
};
    
autoAddDeps(EtherboySettingsLib, '$ESL');
mergeInto(LibraryManager.library, EtherboySettingsLib);
