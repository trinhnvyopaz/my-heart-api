import SubscriptionDto from "../subscription/subscriptionDto"

export default class UserDetailDto {

    PIN: string
    DoctorId: number
    IsSubscription: boolean
    UserId: number
    FirstName: string
    LastName: string
    Email: string
    isFamilyAnamnesis: boolean
    isPersonalAnamnesis: boolean
    isPharmacyAnamnesis: boolean
    isAllergyAnamnesis: boolean
    isAbususAnamnesis: boolean
    isSocialAnamnesis: boolean
    isFamily_ICHS: boolean
    isFamily_ValveDefect: boolean
    isFamily_AtrialFibrillation: boolean
    isFamily_SuddenDeath: boolean
    isFamily_Pacemaker: boolean
    isPersonal_DiseaseId : boolean
    isPersonal_NonpharmaticId : number
    isPharmacy_PharmacyId : number
    isPharmacy_BoldStr : number
    isPharmacy_Dose : string
    isAllergy_PharmacyId : number
    isAllergy_Other : string
    isAbusus_Alcohol : boolean
    isAbusus_Exsmoker : boolean
    isAbusus_Smoker : boolean
    isSocial_LivingWithPartner : boolean
    isSocial_Working : boolean
    isSocial_Pension : boolean
    isSocial_PartialDisabilityPension : boolean
    isSocial_DisabilityPension : boolean
    subscriptionId: number
    subscription: SubscriptionDto
    subscriptionValidTo: Date
    subscriptionCancelAtPeriodEnd: boolean
}
