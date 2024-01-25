const userStore = {
    CULTURE_KEY: "culture_key",
    USER_KEY: "user_key",

    setCulture: function (culture) {
        localStorage.setItem(this.CULTURE_KEY, culture);
    },
    getCulture: function () {
        return localStorage.getItem(this.CULTURE_KEY);
    },
    setUser: function (user) {
        let u = {
            id: user.id,
            username: user.userName
        };

        localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    },
    getUser: function () {
        let u = localStorage.getItem(this.USER_KEY);
        try {
            return JSON.parse(u);
        } catch (error) {
            return null;
        }
    }
};
export default userStore;
