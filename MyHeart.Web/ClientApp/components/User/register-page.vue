<template>
  <div>
      <v-container>
          <v-row align="center"
                 justify="center">
              <v-col cols="12"
                     sm="8"
                     md="4">

                <logo/>

                  <strong class="red--text" v-html="error" v-if="error"></strong>

                  <v-text-field v-model="register.email"
                                color="error"
                                id="email"
                                label="Email" />

                  <v-text-field v-model="register.firstName"
                                color="error"
                                id="firstName"
                                label="First name" />

                  <v-text-field v-model="register.lastName"
                                color="error"
                                id="lastName"
                                label="Last Name" />

                  <v-text-field id="password"
                                v-model="register.password"
                                color="error"
                                type="password"
                                label="Password" />

                  <v-text-field id="password"
                                v-model="register.passwordAgain"
                                color="error"
                                type="password"
                                label="Password again" />
                  <v-row>
                      <v-col cols="0" md="1">
                        </v-col>
                          <v-col cols="5" >
                              <v-btn class="custom empty wide" :to="'/login'">Zpět</v-btn>
                          </v-col>
                          <v-col cols="7" md="5">
                              <v-btn class="custom wide" @click="createUser">Registrovat</v-btn>
                          </v-col>
                  </v-row>
              </v-col>
          </v-row>
      </v-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import RegisterDto from "@models/user/LoginDto";
import UserApi from "@backend/api/user";
import Logo from "../Shared/Logo.vue"

@Component({
  name: "LoginPage",
  components:{
    Logo
  }
})
export default class RegisterPage extends Vue {
  register: RegisterDto = new RegisterDto();
  loading: boolean = false;

  error: string | null = null;


    async createUser() {
        this.loading = true;
        this.error = "";
        var result = await UserApi.registerUser(this.register);
        console.log(result);
        if (result.success) {
            this.$router.push(`register/verify/${result.data.id}`);
        } else {
            for (var key in result.errors) {
                var errors = result.errors[key];

                if (typeof errors === 'string') {
                    this.error += `${errors} `;
                    this.error += "<br>";
                }else {
                    for (var index in errors) {
                        this.error += `${errors[index]} `;
                        this.error += "<br>";
                    }
                }
                
            }
        }

        this.loading = false;
    }

}
</script>

<style lang="scss" scoped></style>
