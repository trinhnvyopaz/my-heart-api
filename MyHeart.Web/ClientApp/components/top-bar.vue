<template>
    <div class="d-flex flex-column flex-md-row">
        <div class="d-flex flex-row  order-0 order-md-1">
            <v-btn v-if="$vuetify.breakpoint.mobile" icon @click="toggleMobileNavMenu" class="hambuger-btn">
                <v-icon>mdi-menu</v-icon>
            </v-btn>
            <v-spacer v-if="$vuetify.breakpoint.mobile"/>
            <div class="d-flex" v-if="IsImpersonating()">
                <v-divider vertical class="my-0 mx-2" />
                <span class="mx-1 mr-2 font-weight-medium">pacient - {{ ImpersonatedUser().firstName }} {{ ImpersonatedUser().lastName }}</span>
                <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                        <v-btn elevation="0"
                            icon @click="stopImpersonating"
                            v-bind="attrs"
                            v-on="on">
                            <font-awesome-icon icon="hand-paper" size="lg"></font-awesome-icon>
                        </v-btn>
                    </template>
                    <span>ukončit impersonaci</span>
                </v-tooltip>
            </div>
            <div class="d-flex" v-else-if="isUserLoggedIn()">
                <v-divider vertical class="my-0 mx-2" />
                <v-btn icon @click="navigateToDetail">
                    <v-icon>mdi-account</v-icon>
                </v-btn>
                <span class="mx-1 font-weight-medium">{{ CurrentUser().firstName }} {{ CurrentUser().lastName }}</span>
                <v-tooltip bottom>
                    <template v-slot:activator="{ on, attrs }">
                        <v-btn icon
                            @click="logOut"
                            v-bind="attrs"
                            v-on="on">
                            <font-awesome-icon icon="sign-out-alt" size="lg"></font-awesome-icon>
                        </v-btn>
                    </template>
                    <span>Odhlásit se</span>
                </v-tooltip>
            </div>
        </div>
        
        <v-app-bar elevation="0" class="order-2 order-md-0">
            <v-tabs v-model="value" center-active >
                <v-tabs-slider></v-tabs-slider>
                <v-tab v-for="item in tabNames" :key="item">
                    {{ item }}
                </v-tab>
            </v-tabs>            
            <div class="middle-title">
                <span style="font-size: 30px;color: var(--blue)">
                    {{middleTitle}}      
                </span>
            </div>   
        </v-app-bar>         
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, ModelSync } from "vue-property-decorator";
    import AuthStore from "@backend/store/authStore.ts";
    import Events from "@models/shared/Events";
    import UserDto, { UserType } from "@models/user/UserDto";
    import EventBus from "@models/EventBus";
    
    @Component({
        name: "TopBar",
        components: {
        }
    })
    export default class TopBar extends Vue {
        isOpen: boolean = false;
        @Prop(Array) readonly tabNames: string[];
        @Prop(Number) readonly model: number;
        @Prop(String) readonly middleTitle: string

        get value() {
            return this.model;
        }
        set value(val) {
            this.$emit('input', val);
        }
        navMenuMini = false;
        showMobileMenu = false

        navigateToDetail(){
            var user =this.CurrentUser();
            if (user?.userTypes && user.userTypes.includes(UserType.Doctor)){
                this.$router.push('/doctor/detail/' + user.id).catch(() => {})
            }else{
                this.$router.push('/user/detail/' + user.id).catch(() => {})
            }
        }

        updateMini(newMini: boolean): void {
            console.log("updateMini")
            console.log(newMini)
            this.navMenuMini = newMini;
        }

        isUserLoggedIn(): boolean {
            return AuthStore.isUserLoggedIn();
        }

        CurrentUser(): UserDto {
            return AuthStore.getCurrentUser();
        }

        IsImpersonating(): boolean {
            return AuthStore.isImpersonating();
        }

        ImpersonatedUser(): UserDto {
            return AuthStore.getImpersonatedUser();
        }

        stopImpersonating() {
            AuthStore.stopImpersonating();
            EventBus.$emit(Events.StoppedImpersonating);
        }

        logOut() {
            AuthStore.clearToken();
            AuthStore.clearCurrentUser();
            EventBus.$emit(Events.UserLoggedOut);
            this.$router.push("/login");
        }

        mounted() {
            var base = this;

            EventBus.$on(Events.UserLoggedIn, () => {
                base.$nextTick(() => {
                    base.$forceUpdate();
                });
            });
            EventBus.$on(Events.UserLoggedOut, () => {
                base.$nextTick(() => {
                    base.$forceUpdate();
                });
            });
            EventBus.$on(Events.StartedImpersonating, () => {
                base.$nextTick(() => {
                    base.$forceUpdate();
                });
            });
            EventBus.$on(Events.StoppedImpersonating, () => {
                base.$nextTick(() => {
                    base.$forceUpdate();
                });
            });
        }

        toggleMobileNavMenu(){
            this.showMobileMenu = !this.showMobileMenu
            EventBus.$emit(Events.ToggleMobileNavMenu, this.showMobileMenu)
        }
    }
</script>

<style scoped>
    .v-app-bar {
        border-bottom: 1px solid #ddd !important;
    }
    .hamburger-btn{
        margin-left: 2px!important;
        margin-right: 10px;
    }
    .v-slide-group__prev v-slide-group__prev--disabled{
        display: none!important;
    }
    .hambuger-btn{
        margin-left: 14px;
    }
    .middle-title {
        position: absolute;
        left: 50%;
        margin-top: 5px;
    }
</style>
