import api from '@backend/api/api'

export default {
    upload(file, entity, id) {

        let formData = new FormData();
        formData.append('File', file);
        formData.append('Entity', entity);
        formData.append('RefId', id);

        return api.upload("/asset", formData);
    }
}
