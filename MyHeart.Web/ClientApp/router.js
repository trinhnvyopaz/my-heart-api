import Vue from "vue";
import VueRouter from "vue-router";
import { routes } from "./routes.ts";

import UserApi from "./backend/api/user";
import AuthStore from "./backend/store/authStore";
import EventBus from "./models/EventBus";
import Events from "./models/shared/Events";

Vue.use(VueRouter);

let router = new VueRouter({
  mode: "history",
  routes
});

router.beforeEach(async (to, from, next) => {
  //page title
  console.log(to);
  if (to.meta.requiresAuth) {
    document.title = to.meta.title;
    var currentUserResponse = await UserApi.getCurrentUser();

    if (!currentUserResponse.success) {
      AuthStore.clearCurrentUser();
      AuthStore.clearToken();
      EventBus.$emit(Events.UserLoggedOut);

      next("/login");
      return;
    }
  }

  next();
});

export default router;
