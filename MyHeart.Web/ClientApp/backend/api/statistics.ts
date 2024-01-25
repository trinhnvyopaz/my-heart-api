import Api from "./api";

export default {
    async downloadPacients() {
        var fileName = "Pacienti_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/pacients`, fileName);
    },
    async downloadQuestions() {
        var fileName = "Dotazy_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/questions`, fileName);
    },
    async downloadDoctors(){
        var fileName = "Lékaři_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/doctors`, fileName);
    },
    async downloadDiseases(){
        var fileName = "Onemocnění_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/diseases`, fileName);
    },
    async downloadPharmacy(){
        var fileName = "Léčba_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/pharmacy`, fileName);
    },
    async downloadNonpharmacy(){
        var fileName = "ProvedenýVýkon_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/Statistics/nonpharmacy`, fileName);
    }
};
