<template>
  <div class="add-stub">
    <h1>Add stub(s)</h1>

    <p>
      You can add new stubs here. Fill in the stub below in YAML format and click on "Add stub(s)".
    </p>
    
    <div class="input-group">
      <textarea class="form-control" v-model="stub"></textarea>
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

export default {
  name: "addStub",
  data() {
    return {
      stub: ""
    };
  },
  components: {
    Stub
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
      if(!Array.isArray(parsedObject)) {
        stubsArray = [
          parsedObject
        ];
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
            if(error.response.status === 409) {
              toastr.error(resources.stubAlreadyAdded.format(stub.id))
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
.input-group textarea {
  height: 200px;
  margin-bottom: 10px;
}
</style>