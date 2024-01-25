import AppSetting from './../../../appsettings.json'

const settingsStore = {
    getApiUrl(url) {        
        return AppSetting.BaseUrl + "/" + url;
    }


};
export default settingsStore;
