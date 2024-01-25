import Api from "./api";
import user from "./user";

export default {
    async downloadHealthStatusReport(id: number) {
        var fileName = "ZdravotníZpráva_" + new Date().toLocaleString() + ".pdf";
        await Api.download("PdfExport/HealtStatusReport/" + id, fileName);
    },
    async downloadPharmaciesReport(id: number) {
        var fileName = "UžívanéLéky_" + new Date().toLocaleString() + ".pdf";
        await Api.download("PdfExport/PharmaciesReport/" + id, fileName);
    },
    async downloadChatReport(userId: number, questionId: number, subject: string) {
        var fileName = "Chat_" + "subject_" + new Date().toLocaleString() + ".pdf";
        await Api.download("PdfExport/ChatReport/" + questionId + "/" + userId, fileName);
    },
};