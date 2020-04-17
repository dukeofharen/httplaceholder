<template>
    <v-row>
        <v-col>
            <h1>Stubs</h1>
            <v-row>
                <v-col class="buttons">
                    <v-btn title="Refresh" @click="initialize" color="success"
                    >Refresh
                    </v-btn
                    >
                    <v-btn
                            title="Delete all stubs"
                            @click="deleteAllDialog = true"
                            color="error"
                    >Delete all stubs
                    </v-btn
                    >
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <v-text-field
                            v-model="searchTerm"
                            placeholder="Filter on stub ID or tenant..."
                            clearable
                    ></v-text-field>
                    <v-select
                            :items="tenantNames"
                            placeholder="Select stub tenant / category name for the stubs you would like to see the stubs for..."
                            v-model="selectedTenantName"
                            clearable
                    ></v-select>
                </v-col>
                <v-expansion-panels>
                    <Stub
                            v-bind:fullStub="stub"
                            v-for="stub in filteredStubs"
                            :key="stub.id"
                    ></Stub>
                </v-expansion-panels>
            </v-row>
        </v-col>
        <v-dialog v-model="deleteAllDialog" max-width="290">
            <v-card>
                <v-card-title class="headline">Delete all stubs?</v-card-title>
                <v-card-text>The stubs can't be recovered.</v-card-text>
                <v-card-actions>
                    <div class="flex-grow-1"></div>
                    <v-btn color="green darken-1" text @click="deleteAllDialog = false"
                    >No
                    </v-btn
                    >
                    <v-btn color="green darken-1" text @click="deleteAllStubs">Yes</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script>
    import Stub from "@/components/Stub";
    import {actionNames} from "@/store/storeConstants";
    import {toastSuccess} from "@/utils/toastUtil";
    import {resources} from "@/shared/resources";

    export default {
        name: "stubs",
        data() {
            return {
                stubs: [],
                tenantNames: [],
                selectedTenantName: "",
                searchTerm: "",
                deleteAllDialog: false
            };
        },
        components: {
            Stub
        },
        async created() {
            await this.initialize();
        },
        methods: {
            async initialize() {
                const getStubsPromise = this.$store.dispatch(actionNames.getStubs);
                const getTenantNamesPromise = this.$store.dispatch(actionNames.getTenantNames);
                this.stubs = await getStubsPromise;
                this.tenantNames = await getTenantNamesPromise;
            },
            handleUrlSearch() {
                let term = this.$route.query.searchTerm;
                if (term) {
                    this.searchTerm = term;
                }

                let tenant = this.$route.query.stubTenant;
                if (tenant) {
                    this.selectedTenantName = tenant;
                }
            },
            async deleteAllStubs() {
                this.deleteAllDialog = false;
                await this.$store.dispatch(actionNames.deleteAllStubs);
                toastSuccess(resources.stubsDeletedSuccessfully);
                await this.initialize();
            }
        },
        computed: {
            filteredStubs() {
                const compare = (a, b) => {
                    if (a.stub.id < b.stub.id) return -1;
                    if (a.stub.id > b.stub.id) return 1;
                    return 0;
                };

                let stubs = this.stubs;
                if (this.searchTerm) {
                    stubs = stubs.filter(s => s.stub.id.includes(this.searchTerm) ||
                        (s.stub.tenant && s.stub.tenant.includes(this.searchTerm)))
                }

                if (this.selectedTenantName) {
                    stubs = stubs.filter(s => s.stub.tenant === this.selectedTenantName);
                }

                return stubs.sort(compare);
            }
        },
        watch: {
            $route() {
                this.handleUrlSearch();
            }
        }
    };
</script>

<style scoped>
    .buttons > button {
        margin-right: 10px;
        margin-top: 10px;
    }
</style>
