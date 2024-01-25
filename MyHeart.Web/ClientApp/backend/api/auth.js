import api from "./api";
import authStore from "../store/authStore.ts";

export default {
  urlurl: "login/",
  lostPassword(obj) {
    return api.post("users/recover", obj);
  },
  recoverPassword(obj) {
    return api.put("users/recover", obj);
  },
  login(username, password) {
    return api
      .post("login/", {
        userName: username,
        password: password
      })
      .then(r => {
        return r;
      })
      .catch(e => {
        throw e;
      });
  },
  logout() {
    authStore.clear();
  },
  isLogged() {
    if (authStore.isLoggedIn() == false) {
      return false;
    }

    if (authStore.isTokenExp()) {
      authStore.clear();
      return false;
    }

    return true;
  },
  externalLogin(issuer, accessToken) {
    return api
      .post("/login/external", {
        loginProvider: issuer,
        authorization: accessToken
      })
      .then(response => {
        return response;
      })
      .catch(e => {
        return e;
      });
  },
  externalLogin(data) {
    return api
      .post("/login/external", data)
      .then(response => {
        return response;
      })
      .catch(e => {
        return e;
      });
  },
  authorize() {
    return api
      .getNoCache("/users/current")
      .then(user => {
        console.log("LOGIN USER:", user);
        return user;
      })
      .catch(e => {
        return e;
      });
  }
};
