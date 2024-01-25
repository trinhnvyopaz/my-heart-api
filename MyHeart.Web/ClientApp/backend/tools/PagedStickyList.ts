import BondDetailDto from "../../models/professionInfo/helpClass/bondDetailDto";
import MathTools from './MathTools';
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";
import DiseaseApi from "@backend/api/disease";
import SymptomApi from "@backend/api/symptom";
import SymptomQuestionApi from "@backend/api/symptomQuestions";
import NonPharmacyApi from "@backend/api/nonpharmacy";
import MedicamentGroupApi from "@backend/api/medicamentGroup";
import PreventiveMeasuresApi from "@backend/api/preventiveMeasures";
import MedicalFacilitiesApi from "@backend/api/medicalFacilitie";
import PharmacyApi from "@backend/api/pharmacy";
import AtcApi from "@backend/api/atc";

/*
 * we'll keep list of selected and list to pull new page into
 * on page change we'll check if it is in the sticky selected portion and if not or partially we'll pull a page from the server
 * we'll then check if any selected are present and if they are and shouldn't be we'll remove them and pull more data if needed
 * */
export default class PagedStickyList {
    selected: BondDetailDto[] = [];
    selectedNative: any[] = [];
    api: string;
    sticky: boolean;
    pageLength: number;

    itemsFromAPI: number = 0;
    displayList: BondDetailDto[] = [];
    nativeList: any[] = [];
    request: PagedResourceRequest = { page: 0, pageSize: 10, filter: "", selected: [] };

    constructor(api: string, sticky: boolean = false, pageLen: number = 10) {
        this.api = api;
        this.sticky = sticky;
        this.pageLength = pageLen;
        this.selected = [];
        this.selectedNative = [];
    }

    get pageCount() {
        return Math.ceil(this.itemsFromAPI / this.pageLength);
    }

    set filter(f: string) {
        console.log("filter is: " + this.filter + " , f is: " + f);

        if (f == null) {
            f = "";
        }

        if (this.filter != f.toLowerCase()) {
            this.request.filter = f.toLowerCase();
            this.loadPage();
        }
    }

    get filter() {
        return this.request.filter;
    }

    set page(p: number) {
        if (this.page !== p) {
            this.request.page = p;
            this.request.pageSize = this.pageLength;
            this.loadPage();
        }
    }

    get page() {
        return this.request.page;
    }

    async loadPage(newItem = null) {
        var result;

        if (this.sticky) {
            this.request.selected = this.selected.filter(x => x.name.toLowerCase().includes(this.request.filter.toLowerCase())).map(x => x.id);
        }

        switch (this.api) {
            case "disease": {
                result = await DiseaseApi.getPagedResource(this.request);
                break;
            }
            case "symptom": {
                result = await SymptomApi.getPagedResource(this.request);
                break;
            }
            case "symptomQuestion": {
                result = await SymptomQuestionApi.getPagedResource(this.request)
            }
            case "nonpharmacy": {
                result = await NonPharmacyApi.getPagedResource(this.request);
                break;
            }
            case "medicamentgroup": {
                result = await MedicamentGroupApi.getPagedResource(this.request);
                break;
            }
            case "preventive": {
                result = await PreventiveMeasuresApi.getPagedResource(this.request);
                break;
            }
            case "medicalFacilities": {
                result = await MedicalFacilitiesApi.getPagedResource(this.request);
                break;
            }
            case "pharmacy": {
                result = await PharmacyApi.getPagedResource(this.request);
                break;
            }
            case "atc": {
                result = await AtcApi.getPagedResource(this.request);
                break;
            }
        }


        if (result !== null && result.success) {
            this.itemsFromAPI = result.data.totalCount;
            await this.processResult(result.data.data);

            console.log(newItem)

            if(newItem != null){
                var displayItemIndex = this.displayList.findIndex(d => d.id == newItem.id)
                var nativeItemIndex = this.nativeList.findIndex(d => d.id == newItem.id)

                if(nativeItemIndex != -1){
                    this.nativeList.splice(nativeItemIndex, 1)
                }else{
                    this.nativeList.splice(this.nativeList.length - 1, 1)
                }

                if(displayItemIndex != -1){
                    this.displayList.splice(displayItemIndex, 1)
                }else{
                    this.displayList.splice(this.displayList.length - 1, 1)
                }
                
                var newBondDetail = new BondDetailDto
                newBondDetail.id = newItem.id;
                newBondDetail.name = newItem.name,
                newBondDetail.isSelected = true,
                newBondDetail.bondStr = 0
                
                this.nativeList.splice(0,0, newBondDetail)
                this.displayList.splice(0, 0, newBondDetail)
            }
        }
    }

    async processResult(res: []) {
        this.displayList = [];
        this.nativeList = [];
        var result = res;

        const hasFilter = this.filter != "";

        if (this.sticky && !hasFilter && Math.ceil(this.selected.length / this.pageLength) >= this.page) {
            const startIndex = (this.page - 1) * this.pageLength;
            this.displayList = this.selected.slice(startIndex, startIndex + this.pageLength);
            this.nativeList = this.selectedNative.slice(startIndex, startIndex + this.pageLength);
        }

        for (var key in result) {
            //if (this.nativeList.length >= this.request.pageSize) break;

            const singleBond: BondDetailDto = new BondDetailDto;

            singleBond.id = result[key].id;
            singleBond.isSelected = this.selected.find(x => x.id == result[key].id) != null;
            singleBond.bondStr = 0;

            if (this.api == "atc") {
                singleBond.name = result[key].atcCode + ' - ' + result[key].name;
                this.nativeList.push({ id: result[key].id, name: result[key].atcCode + ' - ' + result[key].name });
            } else {
                singleBond.name = result[key].name;
                this.nativeList.push(result[key]);
            }

            this.displayList.push(singleBond);
        }

        this.sort();
    }

    sort() {
        this.displayList.sort((a, b) => {
            if (this.filter != "") {
                return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
            } else {
                if (a.isSelected && !b.isSelected) {
                    return -1;
                } else if (!a.isSelected && b.isSelected) {
                    return 1;
                } else {
                    if (a.bondStr < b.bondStr) {
                        return 1;
                    } else if (a.bondStr > b.bondStr) {
                        return -1;
                    } else {
                        return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
                    }
                }
            }
        });
    }
}
