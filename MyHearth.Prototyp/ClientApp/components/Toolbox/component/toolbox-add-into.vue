<template>
    <div>
        <a class="btn btn-success text-light mb-2" :class="{'btn-block' : !short}" v-b-modal="id">
            <font-awesome-icon icon="toolbox" /><span v-if="!short">&nbsp;Add to toolbox</span>
        </a>
        <toolbox-modal :id="id" :tool="tool"/>
    </div>
</template>

<script>

    import ToolboxModal from '@components/toolbox/component/toolbox-modal';
//import FontAwesomeIcon from '@fortawesome/vue-fontawesome';

export default {
    props: ['tool', 'id','short'],
    components: {
        ToolboxModal,
        //FontAwesomeIcon
    }
}
</script>

<style>

</style>
