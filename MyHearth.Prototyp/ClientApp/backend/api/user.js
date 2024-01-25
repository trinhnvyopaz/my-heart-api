import api from '@backend/api/api'
import userStore from '@backend/store/userStore'


export default {
    getCurrentUser() {
        return api.get('/users/current')
            .then(function (r) {
                return r.data
            })
    },
    update(model) {
        return api.put('/users/', model)
            .then(function (result) {
                console.log("REGISTER RESULT", result);
                return result;
            })
    },
    register(model) {
        return api.post('/users/', model)
            .then(function (result) {
                console.log("REGISTER RESULT", result);
                return result;
            })
    },
    removeUser(id) {
        return api.delete('/users/' + id)
            .then(function (result) {
                console.log("DELETE USER RESULT", result);
                return result;
            })
    },
    changePassword(model) {
        console.log("PSWD CHANGE", model);
        return api.put('/users', model)
            .then(function (result) {
                console.log("PASSWORD CHANGE RESULT", result);
                return result;
            })
    },
    resetPassword(email) {
        return api.post('users/recover', { email: email })
            .then(function (result) {
                console.log("PASSWORD RESET REQUEST RESULT", result);
                return result;
            })
    },
    recoverPassword(confirmString, password) {
        return api.put('users/recover', { confirmString: this.confirmString, password: this.password })
            .then(function (result) {
                console.log("PASSWORD RECOVERY RESULT", result);
                return result;
            })
    },
    switchLanguage(culture_code) {

        userStore.setCulture(culture_code);
        return api.put('user/SwitchLanguage', { Culture: culture_code })
            .then(function (result) {
                return result.data
            })
    }
}
