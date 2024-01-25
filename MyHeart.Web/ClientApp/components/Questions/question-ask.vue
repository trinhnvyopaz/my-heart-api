<template>
    <v-card class="mt-2">
        <v-card-text>
            <div>
                <v-row v-if="hasError">
                    <v-col>
                        <span class="error">{{error}}</span>
                    </v-col>
                </v-row>

                <v-row>
                    <v-col>
                        <v-text-field v-model="question.subject"
                                      :disabled="impersonating"
                                      color="error"
                                      label="Předmět"
                                      dense />
                    </v-col>
                </v-row>

                <v-row>
                    <v-col>
                        <v-textarea :disabled="impersonating" v-model="question.body" style="height: 100%" placeholder="Tělo zprávy" />
                    </v-col>
                </v-row>

                <v-row class="mt-4">
                    <v-spacer></v-spacer>
                    <v-btn @click="send()" class="error" :disabled="loading || impersonating" style="margin-right:0.7%;">odeslat</v-btn>
                </v-row>
            </div>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch} from "vue-property-decorator";
    import QuestionDto, { QuestionStatus } from "../../models/question/QuestionDto";
    import QuestionApi from "@backend/api/question";

    import AuthStore from "@backend/store/authStore";
    import UserDto from "../../models/user/UserDto";
@Component({
    name: "QuestionAsk",
    components: {
    },
})
export default class QuestionAsk extends Vue {
    question: QuestionDto = new QuestionDto();

    isBodyDetailActive: boolean = false;
    loading: boolean = false;
    error: string = "";

    impersonating: boolean = false;

    get hasError() {
        return this.error != "";
    }

    showBodyDetail() {
        this.isBodyDetailActive = !this.isBodyDetailActive;
    }

    async send() {
        this.loading = true;

        this.error = "";

        var user: UserDto = null;
        if (AuthStore.isImpersonating()) {
            user = AuthStore.getImpersonatedUser() as UserDto;
            this.question.userId = user.id;
        }
        var result = await QuestionApi.askQuestion(this.question);

        if (result.success) {
            this.question = new QuestionDto();
            this.question.id = -1;
            this.question.userId = -1;
            this.$emit("askedQuestion");
        } else {
            this.error = result.data;
        }

        this.loading = false;
    }

    mounted() {
        this.question = new QuestionDto();
        this.question.id = -1;
        this.question.userId = -1;
        this.impersonating = AuthStore.isImpersonating() as boolean;
    }
}
</script>

<style scoped>
</style>
