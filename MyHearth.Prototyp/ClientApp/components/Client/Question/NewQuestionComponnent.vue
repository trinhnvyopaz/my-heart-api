<template>
    <div class="margin-15 shadow">

        <v-card>
            <v-card-title primary-title class="title" style="background-color: ghostwhite">
                {{ $locale({i: 'question.new'}) }}
            </v-card-title>

            <v-card-text>
                <div>
                    <v-form>
                        <v-container>
                            <v-layout>
                                <v-flex xs12
                                        md3>
                                    <span class="bold-span">{{ $locale({i: 'common.subject'}) }}</span><br />
                                    <v-text-field v-model="newQuestion.subject"></v-text-field>
                                </v-flex>

                                <v-flex xs12
                                        md3>
                                </v-flex>
                            </v-layout>

                            <v-layout>
                                <v-flex xs12
                                        md12>
                                    <span class="bold-span">{{ $locale({i: 'common.description'}) }}</span><br />
                                    <v-textarea solo v-model="newQuestion.text"></v-textarea>
                                </v-flex>
                            </v-layout>

                        </v-container>
                    </v-form>
                </div>
            </v-card-text>

            <v-divider></v-divider>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="primary"
                       @click="goBack()">
                    {{ $locale({i: 'common.back'}) }}
                </v-btn>
                <v-btn color="success"
                       v-on:click="goBack()">
                    {{ $locale({i: 'common.send'}) }}
                </v-btn>
            </v-card-actions>
        </v-card>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';

    import { translations } from '../../utils/translations';
    import userStore from '@backend/store/userStore';
    import { QuestionModel } from '../../../backend/entities/questionModel';

    @Component({
        components: {
        }
    })

    export default class NewQuestionComponnent extends Vue {
        newQuestion: QuestionModel = {} as any;

        goBack() {
            window.history.back();
        }

        created() {
           
        }

    }
</script>

<style>
   
</style>