<template>
    <div>
        <v-container fluid>
            <v-card class=" elevation-3">
                <v-card-title>Dotazy pacienta</v-card-title>
                <v-card-text>
                    <localized-data-table dense :headers="headers" :items="questions" :loading="loading" @click:row="rowClicked">
                    </localized-data-table>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn @click="addDialog">
                        <v-icon>mdi-frequently-asked-questions</v-icon>
                        Přidat
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-container>

    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from "vue-property-decorator";


    import moment from "moment";

    import QuestionDto from "@models/question/QuestionDto";

    import UserApi from "@backend/api/user";

    @Component({
        name: "UserQuestionsListPage",
        components: {
        }
    })
    export default class UserQuestionsListPage extends Vue {
        id: number = 0;
        loadingUser: boolean | string = false;
        questions: QuestionDto[] = [];

        headers = [
            { text: "Id", value: "id" },
            { text: "Předmět", value: "subject" },
            { text: "Datum vytvoření", value: "creationDate" },
            { text: "Detail dotazu", value: "action", sortable: false }
        ];

        mounted() {
            if (this.$route.params.id) {
                this.id = parseInt(this.$route.params.id);
                this.loadQuestions();
            }
        }

        rowClicked(item, args) {
            this.goto(item.id);
        }

        goto(id) {
            this.$router.push("/client/question/" + id);
        }

        async loadQuestions() {
            this.loadingUser = "error";

            var result = await UserApi.getUserQuestions(this.id);

            if (result.success) {
                this.questions = result.data;
            }

            this.loadingUser = false;
        }

    }
</script>

<style scoped>
    .card {
        margin-top: 3rem;
        min-height: 7.5rem;
        max-height: 15rem;
        max-width: 40rem;
        min-width: 20rem;
    }
</style>
