<template>
    <div>
        <breadcrumbs title="Dotazy Pacienta"
                     :routes="[
        { name: 'Administrace', to: '/admin' },
        { name: 'Dotazy Pacienta' }
      ]" />
        <v-card v-for="comment in comments" :key="comment.id" class="card" :style="messageStyle(comment.userTypeId)">
            <v-card-title>{{ comment.userName }}</v-card-title>
            <v-card-text>{{ comment.text }}</v-card-text>
        </v-card>
        <!--<v-bottom-navigation absolute
                             color="whitesmoke"
                             horizontal>

            <v-text-field color="error"
                          label="Napište zprávu..."
                          class="bottom-bar-txt" />

            <v-btn color="error">
                <v-icon dark>mdi-send</v-icon>
            </v-btn>
        </v-bottom-navigation>-->
    </div>
</template>

<script lang="ts">

import { Component, Vue, Watch } from "vue-property-decorator";
import QuestionCommentDto from "@models/user/QuestionCommentDto";
import UserApi from "@backend/api/user";
    import Breadcrumbs from "@components/Shared/breadcrumbs.vue";


@Component({
    name: "UserQuestionsCommentsPage",
    components: {
        Breadcrumbs
  }
})
export default class UserQuestionsCommentsPage extends Vue {
  id: number = 0;
  loadingUser: boolean | string = false;
    comments: QuestionCommentDto[] = [];


    messageStyle(userTypeId: number) {
        if (userTypeId == 1) {
            return 'background: lightcoral !important; left:70%;'
        }
        else {
            return 'background: white !important; left: 2%;'
        }
    }

  mounted() {
    if (this.$route.params.id) {
        this.id = parseInt(this.$route.params.id);
        this.loadComments();
    }
  }

  async loadComments() {
    this.loadingUser = "error";

    var result = await UserApi.getQuestionComments(this.id);

    if (result.success) {
        this.comments = result.data;
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
        background-color: white !important;
    }
    .bottom-bar-btn{
        margin: 0.0rem;
    }
    .bottom-bar-txt {
        margin-left: 2.5rem;
        margin-right: 2.5rem;
        margin-bottom: 1rem;
    }

</style>
