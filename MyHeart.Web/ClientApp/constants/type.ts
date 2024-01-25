export type typeOption = "messageType" | "messageDate" | "personalData" | "healthStatus" | "knownData" | "newlyDiscoveredData" | "aNewTreatment" | "discontinuedTreatment" | "other";
export type typeKey = "MessageType" | "MessageDate" | "Workplace" | "Name" | "Surname" | "PersonalIdentificationNumber" | "Address" | "Height" | "Weight" | "BloodPressure" | "Pulse" | "LDL" | "KnownIllness" | "AWellKnownPharmacy" | "KnownPerformances" | "NewDiseases" | "NewPharmacy" | "NewPerformances" | "DiscontinuedTreatment" | "Other";
export const keyMessageType = "messageType";
export const keyOther = "other";
export const keyMessageDate = "messageDate";

export const title = {
    [keyMessageType]: "Typ zprávy",
    "messageDate": "Datum zprávy",
    "personalData": "Identifikační údaje",
    "healthStatus": "Zdravotní stav",
    "knownData": "Známá data",
    // "knownTreatment": "Známá léčba",
    "newlyDiscoveredData": "Nově zjištěná data",
    // "aNewTreatment": "Nová léčba",
    // "discontinuedTreatment": "Vysazená léčba",
    [keyOther]: "Ostatní",
};

export const headers = {
    "MessageType": "Typ zprávy",
    "MessageDate": "Datum zprávy",
    "Workplace": "Pracoviště",
    "Name": "Jméno",
    "Surname": "Příjmení",
    "PersonalIdentificationNumber": "Rodné číslo",
    "Address": "Adresa",
    "Email": "Email",
    "InsuranceCompany": "Pojišťovna",
    "AssignedDoctor": "Přidělený lékař",
    "Height": "Výška",
    "Weight": "Váha",
    "BloodPressure": "Tlak krve",
    "Pulse": "Tepová frekvence",
    "LDL": "LDL",
    "KnownIllness": "Známá onemocnění",
    "AWellKnownPharmacy": "Známá farmaka",
    "KnownPerformances": "Známé výkony",
    "Other": "Ostatní",
    "NewPharmacy": "Nová farmaka",
    "NewPerformances": "Nové výkony",
    "NewDiseases": "Nová onemocnění",
    "DiscontinuedTreatment": "Vysazená léčba",

};

export const typeNames = {
    1: "messageType",
    2: "messageDate",
    3: "personalData",
    4: "healthStatus",
    5: "knownData",
    6: "newlyDiscoveredData",
    7: "other",
};
export const typeKeys = {
    "messageType": 1,
    "messageDate": 2,
    "personalData": 3,
    "healthStatus": 4,
    "knownData": 5,
    "newlyDiscoveredData": 6,
    "other": 7,
};
export const keyNames = {
    1: "MessageType",
    2: "MessageDate",
    3: "Workplace",
    4: "Name",
    5: "Surname",
    6: "PersonalIdentificationNumber",
    7: "Address",
    8: "Height",
    9: "Weight",
    10: "BloodPressure",
    11: "Pulse",
    12: "LDL",
    13: "KnownIllness",
    14: "AWellKnownPharmacy",
    15: "KnownPerformances",
    // New Data
    16: "NewDiseases",
    17: "NewPharmacy",
    18: "NewPerformances",
    19: "DiscontinuedTreatment",
    20: "Other",
};
