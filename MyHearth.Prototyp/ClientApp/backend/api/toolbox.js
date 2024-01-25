import api from '@backend/api/api'

export default {

    // Products list
    getProductsList () {
        return api.get('/toolbox/GetProductsList')
            .then(function (r) {
                return r.data
            })
    },
    getProduct(id) {
        return api.get('/toolbox/' + id)
            .then(function (r) {
                return r.data
            })
    },
    CreateProductInToolbox(data) {
        return api.post('/toolbox/CreateProductInToolbox', data)
    },
    UpdateProductInToolbox(data) {
        return api.put('/toolbox/UpdateProductInToolbox', data)
    },
    RemoveProductFromToolbox(id) {
        return api.delete('/toolbox/' + id)
    }

}
