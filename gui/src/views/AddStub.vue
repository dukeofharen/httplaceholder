<template>
    <v-row v-shortkey="['ctrl', 's']" @shortkey="addStubs">
        <v-col>
            <h1>Add stub(s)</h1>
            <v-card>
                <v-card-title>You can add new stubs here</v-card-title>
                <v-card-text>
                    <v-row>
                        <v-col>
                            Fill in the stub below in YAML format and click on "Add stub(s)".
                            For examples, visit
                            <a
                                    href="https://github.com/dukeofharen/httplaceholder"
                                    target="_blank"
                            >https://github.com/dukeofharen/httplaceholder</a
                            >.
                        </v-col>
                    </v-row>
                </v-card-text>
            </v-card>
            <v-card class="editor">
                <v-card-text>
                    <v-row>
                        <v-col>
                            You can also select an example from the list below.
                            <br/>
                            <strong>WARNING</strong> The stub in the textbox below will be
                            overwritten!
                            <v-select
                                    :items="stubExamples"
                                    placeholder="Select a stub example..."
                                    v-model="selectedStubExample"
                                    item-text="name"
                                    item-value="key"
                                    @change="stubExampleSelected"
                                    clearable
                            ></v-select>
                        </v-col>
                    </v-row>
                </v-card-text>
                <v-card-actions>
                    <codemirror v-model="input" :options="cmOptions"></codemirror>
                </v-card-actions>
            </v-card>
            <v-btn color="success" @click="addStubs">Add stub(s)</v-btn>
            <h1>Upload stub(s)</h1>
            <v-card>
                <v-card-title>You can upload stubs here</v-card-title>
                <v-card-text
                >Click the button and select a .yml file with stubs from your
                    PC.
                </v-card-text
                >
                <v-card-actions>
                    <input
                            type="file"
                            name="file"
                            ref="stubUpload"
                            @change="loadTextFromFile"
                    />
                    <v-btn color="success" @click="uploadStubs">Upload stubs</v-btn>
                </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
</template>

<script>
    import {codemirror} from "vue-codemirror";
    import {resources} from "@/shared/resources";
    import stubExamples from "@/stub_examples.json";
    import {actionNames} from "@/store/storeConstants";
    import {toastError, toastSuccess} from "@/utils/toastUtil";

    export default {
        name: "addStub",
        data() {
            return {
                input: resources.defaultStub,
                selectedStubExample: {},
                stubExamples: stubExamples,
                cmOptions: {
                    tabSize: 4,
                    mode: "text/x-yaml",
                    lineNumbers: true,
                    line: true
                }
            };
        },
        created() {
            if (this.darkTheme) {
                this.cmOptions.theme = "material-darker";
            }
        },
        components: {
            codemirror
        },
        computed: {
            darkTheme() {
                return this.$store.getters.getDarkTheme;
            }
        },
        methods: {
            async addStubs() {
                await this.addStubsInternal(this.input);
            },
            uploadStubs() {
                this.$refs.stubUpload.click();
            },
            loadTextFromFile(ev) {
                const file = ev.target.files[0];
                const reader = new FileReader();
                reader.onload = e => {
                    this.addStubsInternal(e.target.result);
                };
                reader.readAsText(file);
            },
            stubExampleSelected(key) {
                if (key !== "empty") {
                    let stub = this.stubExamples.find(e => e.key === key);
                    this.selectedStubExample = stub;
                    this.input = stub.stub;
                }
            },
            async addStubsInternal(input) {
                const promises = await this.$store.dispatch(actionNames.addStubs, {input});
                for (let promise of promises) {
                    try {
                        const stub = await promise;
                        toastSuccess(resources.stubAddedSuccessfully.format(stub.id));
                    } catch (e) {
                        if (e.error.response.status === 409) {
                            toastError(resources.stubAlreadyAdded.format(e.stubId))
                        } else {
                            toastError(resources.stubNotAdded.format(e.stubId))
                        }
                    }
                }
            }
        }
    };
</script>

<style scoped>
    .v-card {
        margin-top: 10px;
        margin-bottom: 10px;
    }

    input[type="file"] {
        display: none;
    }
</style>
