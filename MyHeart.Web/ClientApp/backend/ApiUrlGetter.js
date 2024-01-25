const Config = require("../../appsettings.json");

function getApiUrl(mode) {
  console.log(mode, Config)

  if (!mode) return Config.DevBaseUrl;

  switch (mode) {
    case "dev":
      return Config.DevBaseUrl;
    case "test":
      return Config.TestBaseUrl;
    case "azure":
      return Config.AzureBaseUrl;
  }
}

module.exports = {
  getApiUrl,
};
