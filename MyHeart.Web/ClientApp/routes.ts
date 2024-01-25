//authorization
import LoginPage from "@components/User/login-page.vue";
import RegisterPage from "@components/User/register-page.vue";
import RegisterVerificationPage from "@components/User/register-verification-page.vue";
import RegistrationCompletePage from "@components/User/registration-complete-page.vue";
import ManagerPage from "@components/Manager/manager-page.vue";
import UserDetailPage from "@components/Manager/Users/user-detail-page.vue";
import UsersQuestionCommentsPage from "@components/Manager/Users/user-questions-comments-page.vue";
import DoctorDetailPage from "@components/Manager/Doctors/doctor-detail-page.vue";
import DoctorDetailSelfPage from "@components/Manager/Doctors/doctor-self-detail-page.vue";

import ClientDashbordPage from "@components/Client/client-dashboard-page.vue";
import ClientDiseases from "@components/Client/client-diseases.vue";
import ClientQuestionPage from "@components/Questions/client-questions.vue";
import ClientNewsDetail from "@components/Client/client-therapy-news-detail.vue";
import ClientTherapyNews from "@components/Client/client-therapy-news.vue"
import ClientTherapy from "@components/Client/client-therapy.vue"
import ClientNonpharmaticTherapy from "@components/Client/client-nonpharmatic-therapy.vue"
import ClientReports from "@components/Client/client-reports.vue"
import ClientQuestionaire from "@components/Client/client-questionaire.vue"

import AdminUsersList from "@components/Manager/Users/users-list-page.vue";
import AdminDoctors from "@components/Manager/Doctors/doctors-page.vue";
import AdminProfessionInfo from "@components/Manager/ProfessionInformation/professionInfo-page.vue";
import AdminTicketList from "@components/Manager/Tickets/ticket-list-page.vue";
import QuestionaireList from "@components/Manager/Questionaire/questionaire-list-page.vue";
import AdminReports from "@components/Manager/Reports/admin-reports.vue";
import AdminReportDetail from "@components/Manager/Reports/report-detail.vue";

import PasswordRecoveryPage from "./components/User/password-recovery-page.vue";
import PasswordRecoveryRequestPage from "./components/User/password-recovery-request.vue";

import QuestionChat from "./components/Questions/question-chat.vue";
import QuestionVideo from "./components/Questions/question-video.vue";

import CustomDataTable from "@components/Shared/customDataTable.vue";

import PrivacyPage from "./components/privacy.vue"
import SubscriptionPayment from "./components/Manager/Subscription/subscription-payment.vue"

import StatisticsPage from "./components/Manager/Statistics/statisctics-page.vue"

export const routes = [
    {
        path: "/",
        redirect: "/login"
    },
    {
        path: "/customDataTable",
        component: CustomDataTable,
        meta: {
            title: "test",
        },
    },
  {
    path: "/login",
    component: LoginPage,
    meta: {
        title: "My Heart - Přihlášení",
    },
  },
  {
     path: "/register",
     component: RegisterPage,
     meta: {
         title: "My Heart - Registrace",
     },
    },
    {
        path: "/register/verify/:id",
        component: RegisterVerificationPage,
        meta: {
            title: "My Heart - Registrace",
        },
    },
    {
        path: "/register/complete",
        component: RegistrationCompletePage,
        meta: {
            title: "My Heart - Registrace Dokončení",
        },
    },
    {
        path: "/password-recovery",
        component: PasswordRecoveryPage,
        meta: {
            title: "My Heart - Obnova hesla",
        },
    },
    {
        path: "/recover",
        component: PasswordRecoveryRequestPage,
        meta: {
            title: "My Heart - Obnova hesla",
        },
    },
    {
        path: "/admin",
        component: ManagerPage,
        meta: {
            title: "My Heart - Administrátor",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/users",
        component: AdminUsersList,
        meta: {
            title: "My Heart - Administrátor",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/doctors",
        component: AdminDoctors,
        meta: {
            title: "My Heart - Administrátor",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/profinfo",
        component: AdminProfessionInfo,
        meta: {
            title: "My Heart - Administrátor",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/questions",
        component: AdminTicketList,
        meta: {
            title: "My Heart - Administrátor",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/questionaire",
        component: QuestionaireList,
        meta: {
            title: "My Heart - Auto rozhovor",
            requiresAuth: true,
        },
    },
    {
        path: "/user/detail/:id",
        component: UserDetailPage,
        meta: {
            title: "My Heart - Uživatel",
            requiresAuth: true,
        },
    },
    {
        path: "/user/detail/:id/payment/:subscriptionId",
        component: SubscriptionPayment,
        meta:{
            title: "My Heart - Platba",
            requiresAuth: true,
        },
    },
    {
        path: "/question/:id",
        component: UsersQuestionCommentsPage,
        meta: {
            title: "My Heart - Dotazy",
            requiresAuth: true,
        },
    },
    {
        path: "/client/questions",
        component: ClientQuestionPage,
        meta: {
            title: "My Heart - Dotazy",
            requiresAuth: true,
        },
    },
    {
        path: "/client/question/:id",
        component: QuestionChat,
        meta: {
            title: "My Heart - Dotazy",
            requiresAuth: true,
        },
    },
    {
        path: "/client/video/:id",
        component: QuestionVideo,
        meta: {
            title: "My Heart - Dotazy",
            requiresAuth: true,
        },
    },
    {
        path: "/client/diseases",
        component: ClientDiseases,
        meta: {
            title: "My Heart - Moje onemocnění",
            requiresAuth: true,
        },
    },
    {
        path: "/client/therapy",
        component: ClientTherapy,
        meta: {
            title: "My Heart - Moje léčby",
            requiresAuth: true,
        },
    },
    {
        path: "/client/nonpharmatic-therapy",
        component: ClientNonpharmaticTherapy,
        meta: {
            title: "My Heart - Provedené výkony",
            requiresAuth: true,
        },
    },
    {
        path: "/client/reports",
        component: ClientReports,
        meta: {
            title: "My Heart - Moje zdravotní zprávy",
            requiresAuth: true,
        },
    },
    {
        path: "/client/questionaire",
        component: ClientQuestionaire,
        meta: {
            title: "Auto rozhovor",
            requiresAuth: true,
        },
    },
    {
        path: "/doctor/detail/:id",
        component: DoctorDetailSelfPage,
        meta: {
            title: "My Heart - Doktor",
            requiresAuth: true,
        },
    },
    {
        path: "/doctor/detail",
        component: DoctorDetailSelfPage,
        meta: {
            title: "My Heart - Doktor",
            requiresAuth: true,
        },
    },
    {
        path: "/client/dash",
        component: ClientDashbordPage,
        meta: {
            title: "My Heart",
            requiresAuth: true,
        },
    },
    {
        path: "/client/news",
        component: ClientTherapyNews,
        meta: {
            title: "My Heart - Novinky v léčbě",
            requiresAuth: true,
        },
    },
    {
        path: "/client/news/detail/:id",
        component: ClientNewsDetail,
        meta:{
            title: "My Heart - detail novinky",
            requiresAuth: true,
        },
    },
    {
        path: "/privacy",
        component: PrivacyPage,
        meta:{
            title: "My Heart - soukromí",
            requiresAuth: false,
        },
    },
    {
        path: "/admin/statistics",
        component: StatisticsPage,
        meta:{
            title: "My Heart - statistiky",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/reports",
        component: AdminReports,
        meta:{
            title: "My Heart - report",
            requiresAuth: true,
        },
    },
    {
        path: "/admin/report/detail/:id",
        component: AdminReportDetail,
        meta:{
            title: "My Heart - detail report",
            requiresAuth: true,
        },
    }
];
