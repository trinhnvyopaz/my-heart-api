<template>
    
    <div>

        <fieldset>
            <legend>Unregister tool</legend>
            <div class="toolbox-actions-grid">
                <div class="form-group">
                    <a class="text-light btn btn-danger" @click="deleteTool()">Delete tool</a>
                </div>
            </div>
        </fieldset>

    </div>

</template>

<script>

    //import FontAwesomeIcon from '@fortawesome/vue-fontawesome';
    import toolboxApi from '@backend/api/toolbox'

    export default {
        props: ['toolboxId'],
        components: {
            //FontAwesomeIcon
        },        
        methods: {
            deleteTool() {

                if (confirm(this.$locale({ i: 'toolbox.confirmDeleteProductFromToolbox' }))) {
                    toolboxApi.RemoveProductFromToolbox(this.toolboxId).then(r => {

                        if (r.status == 200 || r.status == 201 || r.status == 202) {
                            alert(this.$locale({ i: 'toolbox.removedFromToolbox' }));
                            this.$router.push({ path: '/toolbox' });
                        }
                        else {
                            alert(this.$locale({ i: 'toolbox.errorRemovedFromToolbox' }));
                        }

                    });
                }
            }

        }
    };
</script>

<style>
    .toolbox-actions-grid
    {
        padding: 0 1em;
    }
</style>
