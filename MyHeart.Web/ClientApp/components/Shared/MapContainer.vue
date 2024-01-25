<template>
    <div id="map" style="min-height: 600px;"></div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from "vue-property-decorator";
    import SeznamMapFactory from "../../backend/tools/SeznamMapFactory";

    @Component({
        name: "MapContainer"
    })
    export default class MapContainer extends Vue {
        @Prop({ default: "" })
        gps: string;

        mounted() {
            this.getMap();
        }

        async getMap() {
            if (this.gps == null || this.gps == "") return;

            const split = this.gps.split(" ");
            if (split.length != 2) return;

            let map: any;
            let mapContainer = document.getElementById("map");
            const mapFactory = new SeznamMapFactory();

            try {
                mapFactory.init(() => {
                    map = mapFactory.helper.createMap(mapContainer, Number.parseFloat(split[0]), Number.parseFloat(split[1]), 17);
                });
            } catch (exception) {
                mapFactory.init(() => {
                    map = mapFactory.helper.createMap(mapContainer, Number.parseFloat(split[0]), Number.parseFloat(split[1]), 17);
                });
            }

            while (map == null) {
                await this.delay(500);
                this.getMap();
            }
            map.addDefaultLayer(window.SMap.DEF_BASE).enable();
            map.addDefaultControls();

            var layer = mapFactory.helper.createMarkerLayer();
            map.addLayer(layer);
            layer.enable();

            var marker = mapFactory.helper.createMarker(Number.parseFloat(split[0]), Number.parseFloat(split[1]));
            layer.addMarker(marker);
        }

        delay(ms: number) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
    }
</script>
