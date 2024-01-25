<template>
    <div>

        <v-card>
            <v-card-title primary-title class="title">
                {{ $locale({i: 'common.symptom'}) }} {{symptom.name}}
            </v-card-title>

            <v-divider></v-divider>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md4>
                                    <span class="bold-span">{{ $locale({i: 'common.name'}) }}</span><br />
                                    <v-text-field v-model="symptom.name"></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <v-textarea v-model="symptom.description"></v-textarea>
                                </v-flex>
                            </v-layout>

                        </v-container>
                    </v-form>
                </div>
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="primary"
                       @click="close()">
                    {{ $locale({i: 'common.back'}) }}
                </v-btn>
                <v-btn color="error"
                       v-on:click="close()">
                    {{ $locale({i: 'common.delete'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="close()">
                    {{ $locale({i: 'common.save'}) }}
                </v-btn>
            </v-card-actions>
        </v-card>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch, Prop } from 'vue-property-decorator';

    import { translations } from '../../../../utils/translations';
    import userStore from '@backend/store/userStore'

    @Component({
        components: {
        }
    })

    export default class SymptomComponnent extends Vue {
        @Prop() symptom;

        close() {
            this.$emit("dialog-closed");
        }

        created() {

        }

    }
</script>

<style>
    .bold-span {
        font-weight: bold;
    }
</style>