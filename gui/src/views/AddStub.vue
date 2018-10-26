<template>
  <div class="add-stub">
    <h1>Add stub(s)</h1>

    <p>
      You can add new stubs here. Fill in the stub below in YAML format and click on "Add stub(s)". For examples, visit <a href="https://github.com/dukeofharen/httplaceholder" target="_blank">https://github.com/dukeofharen/httplaceholder</a>.
    </p>
    
    <div class="input-group">
      <codemirror v-model="stub" :options="cmOptions"></codemirror>
    </div>

    <div class="input-group">
      <a class="btn btn-success" v-on:click="addStubs">Add stub(s)</a>
    </div>

  </div>
</template>

<script>
import { shouldAuthenticate, logicAddStub } from "@/data/dataLogic";
import Stub from "@/components/Stub";
import resources from "@/resources";
import toastr from "toastr";
import yaml from "js-yaml";
import { codemirror } from "vue-codemirror";

export default {
  name: "addStub",
  data() {
    return {
      stub: resources.defaultStub,
      cmOptions: {
        tabSize: 4,
        mode: "text/x-yaml",
        lineNumbers: true,
        line: true
      }
    };
  },
  components: {
    Stub,
    codemirror
  },
  created() {
    shouldAuthenticate(result => {
      if (result) {
        this.$router.push({ name: "login" });
      }
    });
  },
  methods: {
    addStubs() {
      let stubsArray;
      let parsedObject = yaml.safeLoad(this.stub);
      if (!Array.isArray(parsedObject)) {
        stubsArray = [parsedObject];
      } else {
        stubsArray = parsedObject;
      }

      for (let index in stubsArray) {
        let stub = stubsArray[index];
        logicAddStub(stub)
          .then(response => {
            toastr.success(resources.stubAddedSuccessfully.format(stub.id));
          })
          .catch(error => {
            if (error.response.status === 409) {
              toastr.error(resources.stubAlreadyAdded.format(stub.id));
            } else {
              toastr.error(resources.stubNotAdded.format(stub.id));
            }
          });
      }
    }
  },
  watch: {}
};
</script>

<style scoped>
.vue-codemirror {
  width: 100%;
  margin: 10px;
}
.CodeMirror {
  width: 100%;
}

.add-stub {
  text-align: left;
}
</style>