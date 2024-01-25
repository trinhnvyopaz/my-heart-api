//authorization
import Login from '@components/authorization/login'
import Register from '@components/authorization/register'
import RegistrationComplete from '@components/authorization/registration-complete'
import ResetPassword from '@components/authorization/reset-password'
import ResetPasswordComplete from '@components/authorization/reset-password-complete'
import AcceptTerms from '@components/authorization/accept-terms'
import PasswordRecovery from '@components/authorization/password-recovery'
import Terms from '@components/authorization/terms'
import SetNewPassword from '@components/authorization/set-new-password'

import NotFound from '@components/NotFound'

import MainPage from '@components/Home/MainPage';
import ManagerPage from '@components/Manager/ManagerPage';
import ClientPage from '@components/Client/ClientPage';
import DoctorPage from '@components/Doctor/DoctorPage';

import { translations } from './utils/translations';

import userStore from '@backend/store/userStore';

import InformationComponnent from '@components/Manager/componnents/informations/Main/informationComponnent';
import SymptomComponnentInfoDetail from '@components/Manager/componnents/informations/Symptom/symptomComponnentInfoDetail';

import NewQuestionComponnent from '@components/Client/Question/NewQuestionComponnent';
import QuestionListComponnent from '@components/Client/Question/QuestionListComponnent';
import ClientProfile from '@components/Client/ClientProfilePage';
import DiagnosisList from '@components/Client/DiagnosisListPage';
import UserDetail from '@components/Manager/componnents/users/UserDetailComponnent';
import PatientDetail from '@components/Doctor/components/patient/components/PatientComponent';
import DetailOfMedicalGroupComponnent from '@components/Manager/componnents/informations/MedicalGroup/DetailOfMedicalGroupComponnent';
import DetailOfFarmakComponnent from "@components/Manager/componnents/informations/Pharmas/DetailOfPharmaComponnent";
import DetailOfNonPharmaComponnent from "@components/Manager/componnents/informations/NonPharmas/DetailOfNonPharmaComponnent";
import DetailOfPreventiveMeasureComponnent from "@components/Manager/componnents/informations/PreventiveMeasures/DetailOfPreventiveMeasureComponnent";
import DetailOfMedicalPlaceComponnent from "@components/Manager/componnents/informations/MedicalPlaces/DetailOfMedicalPlaceComponnent";
import DetailOfNewsComponnent from "@components/Manager/componnents/informations/News/DetailOfNewsComponnent";

export const routes = [

    //authorization
    { path: '/login', component: Login, meta: { title: 'Login', breadcrumb: [] } },
    { path: '/register', component: Register, meta: { title: 'Registration', breadcrumb: [] } },
    { path: '/registration-complete', component: RegistrationComplete, meta: { title: 'Registration complete', breadcrumb: [] } },
    { path: '/reset-password', component: ResetPassword, meta: { title: 'Reset password', breadcrumb: [] } },
    { path: '/reset-password-complete', component: ResetPasswordComplete, meta: { title: 'Reset password', breadcrumb: [] } },
    { path: '/accept-terms', component: AcceptTerms, meta: { title: 'Terms', breadcrumb: [] } },
    { path: '/password-recovery', component: PasswordRecovery, meta: { title: 'Password recovery', breadcrumb: [] } },
    { path: '/terms', component: Terms, meta: { title: 'Terms', breadcrumb: [] } },
    { path: '/set-new-password', component: SetNewPassword, meta: { requiresAuth: true, title: 'Set new password', breadcrumb: [] } },

    { path: '/', component: Login, meta: { title: 'Login', breadcrumb: [] } },

    { path: '*', component: NotFound, meta: { title: 'Page not found', breadcrumb: [] } },

    { path: '/mainPage', component: MainPage, meta: { requiresAuth: true, title: "Main", breadcrumb: [] } },

    { path: '/manager_page/:tabName', component: ManagerPage, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "informations").translation, breadcrumb: [] } },
    { path: '/information/:informationId', component: InformationComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "informationDetail").translation, breadcrumb: [] } },
    { path: '/symptom/:symptomId', component: SymptomComponnentInfoDetail, props: true, meta: { requiresAuth: true, title: "Detail symptomu", breadcrumb: [] } },
    { path: '/user/:userId', component: UserDetail, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "userDeatil").translation, breadcrumb: [] } },
    { path: '/client_page', component: ClientPage, meta: { requiresAuth: true, title: "Pacient", breadcrumb: [] } },
    { path: '/newQuestion', component: NewQuestionComponnent, meta: { requiresAuth: true, title: "Dotaz", breadcrumb: [] } },
    { path: '/questionArchive', component: QuestionListComponnent, meta: { requiresAuth: true, title: "Archiv", breadcrumb: [] } },
    { path: '/myDiagnoses', component: DiagnosisList, meta: { requiresAuth: true, title: "Pacient", breadcrumb: [] } },
    { path: '/clientProfile', component: ClientProfile, meta: { requiresAuth: true, title: "Pacient - profil", breadcrumb: [] } },
    { path: '/doctor_page/:selectedTab', component: DoctorPage, props: true, meta: { requiresAuth: true, title: "Doktor", breadcrumb: [] } },
    { path: '/patient/:patientId', component: PatientDetail, props: true, meta: { requiresAuth: true, title: "Detail pacienta", breadcrumb: [] } },
    { path: '/medicalGroup/:medicalGroupId', component: DetailOfMedicalGroupComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "detailOfMedicalGroupComponnent").translation, breadcrumb: [] } },
    { path: '/pharma/:pharmaId', component: DetailOfFarmakComponnent, props: true, meta: { requiresAuth: true, title: "Farmaka", breadcrumb: [] } },
    { path: '/non-pharma/:nonPharmaId', component: DetailOfNonPharmaComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "nonPharma").translation, breadcrumb: [] } },
    { path: '/preventive-measure/:preventiveMeasureId', component: DetailOfPreventiveMeasureComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "preventiveMeasures").translation, breadcrumb: [] } },
    { path: '/medical-place/:medicalPlaceId', component: DetailOfMedicalPlaceComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "madicalPlaces").translation, breadcrumb: [] } },
    { path: '/news/:newsId', component: DetailOfNewsComponnent, props: true, meta: { requiresAuth: true, title: translations["cs-CZ"].breadcrumbNames.find(x => x.name === "news").translation, breadcrumb: [] } }

];