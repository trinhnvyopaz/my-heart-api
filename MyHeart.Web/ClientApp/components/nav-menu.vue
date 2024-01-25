<template>
    <div>
        <v-card class="nav-card">
            <v-navigation-drawer :mini-variant="showMini" mini-variant-width="106" v-model="enabled"
                :permanent="!isNavMenuTemp" :temporary="isNavMenuTemp" app v-if="showNav()" class="nav-bar">

                <v-list v-if="currentUser != null && enabled">
                    <v-list-item class="myHeartItem">
                        <v-list-item-title @click="toggleNavigation" class="myHeartText">
                            <span v-if="!showMini">My Heart</span>
                            <v-img v-else :src="logo" />
                        </v-list-item-title>
                        <v-btn icon @click.stop="toggleNavigation">
                            <v-icon>mdi-chevron-left</v-icon>
                        </v-btn>
                    </v-list-item>
                    <v-divider></v-divider>
                    <template v-if="navItems.length > 1">
                        <v-list-group tag="span" v-for="group in navItems" :key="group.displayName">
                            <template v-slot:activator>
                                <v-tooltip bottom>
                                    <template v-slot:activator="{ on }">
                                        <v-list-item-icon v-on="on">
                                            <svg>
                                                <use :href="'#' + group.icon"></use>
                                            </svg>
                                        </v-list-item-icon>
                                        <v-list-item-content v-on="on">
                                            <v-list-item-title>{{ group.displayName }}</v-list-item-title>
                                        </v-list-item-content>
                                    </template>
                                    <span>{{ group.displayName }}</span>
                                </v-tooltip>
                            </template>
                            <router-link :to="item.url" v-for="item in group.items" :key="item.title">
                                <v-tooltip bottom>
                                    <template v-slot:activator="{ on }">
                                        <v-list-item>
                                            <v-list-item-icon v-on="on" class="sub-nav-item-icon">
                                                <svg>
                                                    <use :href="'#' + item.icon"></use>
                                                </svg>
                                            </v-list-item-icon>
                                            <v-list-item-content v-on="on">
                                                <v-list-item-title>{{ item.title }}</v-list-item-title>
                                            </v-list-item-content>
                                        </v-list-item>
                                    </template>
                                    <span>{{ item.title }}</span>
                                </v-tooltip>
                            </router-link>
                        </v-list-group>
                    </template>
                    <template v-else>
                        <router-link :to="item.url" v-for="item in getMenuByRole" :key="item.title">
                            <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                    <v-list-item v-on="on">
                                        <v-list-item-icon class="sub-nav-item-icon">
                                            <svg>
                                                <use :href="'#' + item.icon"></use>
                                            </svg>
                                        </v-list-item-icon>
                                        <v-list-item-content>
                                            <v-list-item-title>{{ item.title }}</v-list-item-title>
                                        </v-list-item-content>
                                    </v-list-item>
                                </template>
                                <span>{{ item.title }}</span>
                            </v-tooltip>
                        </router-link>
                    </template>
                </v-list>
            </v-navigation-drawer>
        </v-card>
    </div>
</template>





<script lang="ts">
import Vue from "vue";
import { Component, Prop, Watch } from "vue-property-decorator";
import AuthStore from "@backend/store/authStore.ts";
import Events from "@models/shared/Events";
import EventBus from "@models/EventBus";
import UserDto, { UserType } from "../models/user/UserDto";
import Logo from "@resources/icon_symbol.png"

export type MENU_ITEM_TYPE = {
    title: string;
    url: string;
    icon: string;
}
export type MENU_TYPE = {
    displayName: string;
    url: string;
    icon: string;
    items: MENU_ITEM_TYPE[];
}
const roleAdmin = [UserType.SuperAdmin, UserType.Admin];
@Component({
    name: "NavMenu",
    components: {
    }
})
export default class NavMenu extends Vue {
    @Prop({ default: false })
    mini;

    logo = Logo

    enabled: boolean = true;

    currentUser: UserDto = null;
    impersonatedUser: UserDto = null;

    userTypeEnum: any = UserType;

    navItems: MENU_TYPE[] = [];

    showMini: boolean = false
    isSmallScreen: boolean = false;
    isNavMenuTemp: boolean = false;
    get getMenuByRole(): MENU_ITEM_TYPE[] {
        return this.navItems.map(item => item.items).flat();
    }
    get isAdminRole(): boolean {
        return this.currentUser?.userTypes?.split(",").some(item => roleAdmin.includes(+item)) ?? false;
    }
    @Watch("mini", { deep: true, immediate: true })
    onPropertyChanged(newValue: boolean, oldValue: boolean) {
        this.showMini = newValue
    }

    isUserLoggedIn(): boolean {
        return AuthStore.isUserLoggedIn();
    }

    showNav(): boolean {
        return AuthStore.isUserLoggedIn() && this.$route.path != '/login';
    }
    fillData() {
            this.navItems = [
                {
                    displayName: "Administrace",
                    url: "/admin/users",
                    icon: "icon_lekari",
                    items: [
                        {
                            title: "Pacienti",
                            url: "/admin/users",
                            icon: "icon_pacienti"
                        },
                        {
                            title: "Lékaři",
                            url: "/admin/doctors",
                            icon: "icon_lekari"
                        },
                        {
                            title: "Odborné informace",
                            url: "/admin/profinfo",
                            icon: "icon_informace"
                        },
                        {
                            title: "Statistika",
                            url: "/admin/statistics",
                            icon: "icon_statistiky"
                        },
                        {
                            title: "Veškeré dotazy",
                            url: "/admin/questions",
                            icon: "icon_dotazy2"
                        },
                        {
                            title: "Veškeré auto rozhovory",
                            url: "/admin/questionaire",
                            icon: "icon_auto_rozhovor"
                        },
                    ],
                },
                {
                    displayName: "Lékař",
                    icon: "icon_lekari",
                    url: "/admin/users",
                    items: [
                        {
                            title: "Pacienti",
                            url: "/admin/users",
                            icon: "icon_pacienti"
                        },
                        {
                            title: "Lékaři",
                            url: "/admin/doctors",
                            icon: "icon_lekari"
                        },
                        {
                            title: "Odborné informace",
                            url: "/admin/profinfo",
                            icon: "icon_informace"
                        },
                        {
                            title: "Veškeré dotazy",
                            url: "/admin/questions",
                            icon: "icon_dotazy2"
                        },
                        //{
                        //    title: "Lékařské zprávy ke zhodnocení",
                        //    url: "/admin/reports",
                        //    icon: "icon_zdravotni_zpravy"
                        //},
                        {
                            title: "Veškeré auto rozhovory",
                            url: "/admin/questionaire",
                            icon: "icon_auto_rozhovor"
                        },
                        {
                            title: "Můj profil",
                            url: "/doctor/detail/" + this.getDetailId(),
                            icon: ""
                        },
                        //{
                        //    title: "Moje recenze",
                        //    url: "/doctor/rating"
                        //}
                    ]
                },
                {
                    displayName: "Pacient",
                    url: "/client/dash",
                    icon: "icon_pacienti",
                    items: [
                        {
                            title: "Zdravotní stav",
                            url: "/client/dash",
                            icon: "icon_zdravotni_stav"
                        },
                        {
                            title: "Onemocnění",
                            url: "/client/diseases",
                            icon: "icon_onemocneni"
                        },
                        {
                            title: "Léčba",
                            url: "/client/therapy",
                            icon: "icon_lecba"
                        },
                        {
                            title: "Provedené výkony",
                            url: "/client/nonpharmatic-therapy",
                            icon: "icon_provedene_vykony"
                        },
                        {
                            title: "Zdravotní zprávy",
                            url: "/client/reports",
                            icon: "icon_zdravotni_zpravy"
                        },
                        {
                            title: "Dotazy",
                            url: "/client/questions",
                            icon: "icon_dotazy"
                        },
                        {
                            title: "Novinky v léčbě",
                            url: "/client/news",
                            icon: "icon_novinky"
                        },
                        //{
                        //    title: "Recenze",
                        //    url: "/client/rating",
                        //    icon: "icon_zdravotni_stav"
                        //},
                        {
                            title: "Auto rozhovor",
                            url: "/client/questionaire",
                            icon: "icon_auto_rozhovor"
                        },
                        {
                            title: "Můj profil",
                            url: "/user/detail/" + this.getDetailId(),
                            icon: ""
                        }
                    ]
                },
                {
                    displayName: "Lékařská zpráva",
                    icon: "icon_lekari",
                    url: "/admin/reports",
                    items: [
                        {
                            title: "Lékařské zprávy ke zhodnocení",
                            url: "/admin/reports",
                            icon: "icon_zdravotni_zpravy"
                        }
                    ]
                },
            ];
        }

    filterNavItem() {
        this.fillData();
        if (!this.isAdminRole) {
            this.navItems = this.navItems.filter((item, index) => this.currentUser?.userTypes?.includes((index + 1).toString()));
        }
    }
    getDetailId() {
        if (this.currentUser == null) {
            return "";
        }

        if (this.impersonatedUser != null) {
            return this.impersonatedUser.id;
        }

        return this.currentUser.id;
    }

    mounted() {
        var base = this;
        this.userTypeEnum = UserType;
        this.currentUser = AuthStore.getCurrentUser() as UserDto;

        this.filterNavItem();

        EventBus.$on(Events.UserLoggedIn, () => {
            base.$nextTick(() => {
                this.currentUser = AuthStore.getCurrentUser() as UserDto;
                this.impersonatedUser = AuthStore.getImpersonatedUser() as UserDto;
                this.filterNavItem();
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
                this.impersonatedUser = AuthStore.getImpersonatedUser() as UserDto;
                this.filterNavItem();
                base.$forceUpdate();
            });
        });
        EventBus.$on(Events.StoppedImpersonating, () => {
            base.$nextTick(() => {
                this.impersonatedUser = null;
                this.filterNavItem();
                base.$forceUpdate();
            });
        });
        EventBus.$on('open-nav-menu', this.enable);

        EventBus.$on(Events.ToggleMobileNavMenu, () => {
            this.enabled = !this.enabled
        })

        this.onResize();

        window.addEventListener('resize', this.onResize)
    }

    toggleNavMenu() {
        this.enabled = !this.enabled;
    }

    onResize() {

        var smallScreen = window.innerWidth <= 600

        if (smallScreen && this.isSmallScreen == false) {
            this.isNavMenuTemp = true;
            this.enabled = false;
        } else if (!smallScreen && this.isSmallScreen == true) {
            this.enabled = true;
            this.isNavMenuTemp = false;
        }

        this.isSmallScreen = smallScreen
        console.log(this.isNavMenuTemp)
    }

    enable() {
        if (!this.enabled) {
            this.$emit("update-mini", true);
        }
        this.enabled = true;
    }

    goTo(link: string) {
        if (!link.startsWith("/client/question/")) {
            EventBus.$emit(Events.Navigated);
        }
        this.$router.push(link).catch(err => { });
    }

    swipe(direction: string): void {
        if (direction === 'Left') {
            this.$emit("update-mini", true);
        } else if (direction === 'Right') {
            this.$emit("update-mini", false);
        }
    }
    toggleNavigation() {
        if (this.isNavMenuTemp) {
            this.enabled = !this.enabled

        } else {
            this.showMini = !this.showMini
        }
    }
}
</script>

<style scoped>
.hambuger-btn {
    margin-left: 14px;
}

.sub-nav-item-icon {
    margin-left: 15px !important;
}

.nav-bar {
    background-color: var(--navmenu-color) !important;
    color: var(--gray4) !important;
}

.v-navigation-drawer--mini-variant {
    width: 96px !important;
}

.v-list {
    padding: 0px !important;
}

.v-list-item__content {
    font-weight: 600 !important;
}

.nav-bar .v-list-item:hover:not(.myHeartItem) {
    background-color: #ddd !important;
}

.myHeartItem {
    height: 64px;
}

.router-link-active .v-list-item {
    background-color: var(--bg-blue) !important;
    border-left: 5px solid var(--blue);
    cursor: default !important;
}

.router-link-active .v-list-item__content {
    font-weight: 800 !important;
}

.router-link-active .v-list-item * {
    color: var(--blue) !important;
}

.myHeartText {
    color: var(--red) !important;
    font-weight: 300 !important;
    font-size: 30px;
    margin: 14px 10px !important;
}

svg {
    width: 24px;
    height: 24px;
    fill: currentColor !important;
}

.nav-bar .v-divider {
    margin-bottom: 40px !important;
}

hr {
    position: relative;
    top: -1px;
}

a {
    text-decoration: none;
}

.nav-card {
    background-color: #F7F7F7;
}

.v-sheet {
    background-color: var(--bg-color) !important;
}
</style>
