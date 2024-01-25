<template>
    <b-modal :id="id" @ok="handleOk" :ok-title="okTitle" ref="toolModalRef">
        <span slot="modal-title">{{tool}}        
        </span>
        <b-form>
            <b-form-group label="Name of your tool"
                          label-for="toolName">

                <b-form-input id="toolName"
                              type="text"
                              v-model="toolName"
                              vocab=""
                              placeholder="Enter name"></b-form-input>
                <label class="label label-warning" v-show="validationToolName">{{validationToolName}}</label>
            </b-form-group>
            <b-form-group label="Tool buy date"
                          label-for="toolBuyDate">

                <b-form-input id="toolBuyDate"
                              type="date"
                              v-model="toolBuyDate"
                              vocab=""
                              placeholder="Enter buy date"></b-form-input>
                <label class="label label-warning" v-show="validationToolBuyDate">{{validationToolBuyDate}}</label>
            </b-form-group>

            <b-form-group label="Your notes"
                          label-for="notes">

                <b-form-textarea id="notes"
                                 type="text"
                                 v-model="notes"
                                 rows="3"
                                 placeholder="Enter your notes">
                </b-form-textarea>
            </b-form-group>

        </b-form>
    </b-modal>
</template>

<script>

import toolboxApi from '@backend/api/toolbox'

export default {
    props: ['tool', 'id', 'toolboxProduct'],       
    data: function(){
        return{
            toolName: this.tool,
            toolBuyDate: (this.toolboxProduct ? this.toolboxProduct.toolBuyDate : ""),
            notes: (this.toolboxProduct ? this.toolboxProduct.description : ""),
            validationToolName: '',
            validationToolBuyDate: ''
        }
    },
    methods:{   
        handleOk: function (evt) {
            evt.preventDefault();
            this.addToToolbox();
        },
        validate: function () {

            var isValid = true;

            this.validationToolName = null;

            if (this.toolName == null || !this.toolName.length) {
                this.validationToolName = this.$locale({
                    i: 'errors.missingProductName'
                });                
                isValid = false;
            }            
            return isValid;

        },
        emitGlobalUpdateEvent() {
            this.clickCount++;            
            this.$eventBus.$emit('toolbox-product-updated', true);
        },
        addToToolbox: function () {
            
            if (!this.validate()) {
                return;
            }

            let toolboxModel = {
                productId: this.id,
                productName: this.toolName,
                description: this.notes,
                toolBuyDate: (this.toolBuyDate ? this.toolBuyDate : null)
            };

           
            if (this.toolboxProduct) {
                if (parseInt(this.toolboxProduct.id) > 0) {
                    toolboxModel.id = this.toolboxProduct.id;
                }
                else {
                    alert("Can not save product details");
                    return;
                }
            }
             

            if (this.toolboxProduct) {
                toolboxApi.UpdateProductInToolbox(toolboxModel).then(r => {
                    if (r.status == 200 || r.status == 201 || r.status == 202) {
                        this.$refs.toolModalRef.hide();
                        this.emitGlobalUpdateEvent();    // Send event
                    }
                    else {
                        alert(this.$locale({ i: 'toolbox.errorSaveToToolbox' }));
                    }
                });
            }
            else {
                toolboxApi.CreateProductInToolbox(toolboxModel)
                .then(r => {
                    if (r.status == 200 || r.status == 201 || r.status == 202) {
                        alert(this.$locale({ i: 'toolbox.savedToToolbox' }));
                        this.$router.push({ path: '/toolbox' });
                    }
                    else {
                        alert(this.$locale({ i: 'toolbox.errorSaveToToolbox' }));
                    }
                });
            }                

        }
    },
    computed: {
        okTitle() {
            if (this.toolboxProduct) {
                return "Save";
            }
            else {
                return "Add";
            }
            
        }
    },
    components: {
        
    }
}
</script>

<style>

</style>
