import Vue from 'vue'
import VueRouter from 'vue-router'
import { routes } from './routes'

import authApi from '@backend/api/auth'

Vue.use(VueRouter);

let router = new VueRouter({
    mode: 'history',
    routes
})

router.beforeEach((to, from, next) =>
{
    //page title
    document.title = to.meta.title;

    if (to.meta.requiresAuth)
    {
        var token = localStorage.getItem("token_key");
        if (token == null)
        {
            authApi.logout();
            next('/login');
        }

        authApi.authorize().then(r => {
            if (r.status == 401) {
                authApi.logout();
                next('/login');
            }    
        });

        //var http = new XMLHttpRequest();

        //http.onreadystatechange = function ()
        //{
        //    if (http.readyState == XMLHttpRequest.DONE)
        //    {
        //        if (http.status == 401)
        //        {
        //            authApi.logOut();
        //            next('/login');
        //            //alert('YOU WERE LOGGED OUT');
        //        }
        //    }
        //}       

        //var key = 'Authorization';
        //var value = 'Bearer ' + token;
                

        //http.open('GET', currentUser, true);
        //http.setRequestHeader(key, value);
        //http.setRequestHeader('Cache-Control', 'no-cache');
        //http.send();
    }

    next();
})

export default router
