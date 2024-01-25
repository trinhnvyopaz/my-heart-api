<template>
    <v-container class="tile">
        <svg class="icon"><use :href="'#' + icon"></use></svg>
        <h3>{{title}}</h3>
        <span class="date">{{timeDist}}</span>
        <div class="centralValue">
            <div v-if="textValue" class="currentValues">
                {{textValue}}
            </div>
            <div v-else>
                <div :class="currentValuesClass">{{currentValues}}<span v-if="unit">&nbsp;{{unit}}</span></div>
                <!-- <div class="diffValues">{{diffValues}}<span v-if="unit">&nbsp;{{unit}}</span></div> -->
            </div>
        </div>

    </v-container>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
    import Formatting from '@utils/formatting';

@Component({
    name: "DashboardTile",
})
export default class DashboardTile extends Vue {
    loading: boolean = false;
    @Prop({ default: "" }) title: string;
    @Prop({ default: () => null }) date: Date;
    @Prop({ default: "" }) icon: string;
    @Prop({ default: () => [] }) values: number[];
    @Prop({ default: () => [] }) valuesDiff: number[];
    @Prop({ default: () => [] }) minValues: number[];
    @Prop({ default: () => [] }) maxValues: number[];
    @Prop({ default: "" }) unit: string;
    @Prop({ default: "" }) textValue: string;

    get currentValues() {
        if (!this.values) return " ";
        return this.values.join('/');
    }
    get diffValues() {
        if (!this.valuesDiff) return " ";
        return this.valuesDiff.map(x => x < 0 ? x : "+" + x).join('/');
    }
    get currentValuesClass() {
        let levels = ['great', 'ok', 'bad'];
        var level = 0;
        for (var i = 0; i < this.values.length; i++) {
            let v = this.values[i];
            let lo = this.minValues[i];
            let hi = this.maxValues[i];
            let mid = (lo + hi) / 2;
            let dev = Math.abs(mid - v);
            if (dev / (mid - lo) > 0.6) level = Math.max(level, 1);
            if (dev / (mid - lo) > 1) level = Math.max(level, 2);
        }
        return "currentValues " + levels[level];
    }

    get timeDist() {
        if (!this.date) return " ";
        return Formatting.formatDateAsDistance(this.date);
    }

}
</script>


<style scoped>
    .tile {
        background-color: var(--white);
        height: 267px;
        border-radius: 20px;
        padding: 20px;
        cursor: pointer;
    }
    .tile:hover {
        background-color: var(--navmenu-color);
    }
    .tile .centralValue {
        display: flex;
        align-items: center; /* Vertical center alignment */
        justify-content: center; /* Horizontal center alignment */
        text-align: center;
        height: 150px;
    }
    .currentValues {
        font-size: 59px;
        font-weight: 800;
    }
    .currentValues span{
        font-size: 49px;
        font-weight: 600;
    }
    .currentValues.bad {
        color: var(--red);
    }
    .currentValues.ok {
        color: var(--orange);
    }
    .tile h3 {
        font-size: 24px;
    }
    .tile .date {
        font-size: 20px;
        color: #47474A !important;
    }
    .diffValues {
        font-weight: 600;
        font-size: 20px;
        color: #7C7C84;
    }
    .tile .icon {
        float: right;
        color: #2d2957;
        width: 32px;
        height: 32px;
    }
</style>
