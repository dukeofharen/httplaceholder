import { getField, updateField } from "vuex-map-fields";

const storeTypeEnum = {
  GETTER: 0,
  MUTATION: 1,
  ACTION: 2,
  WATCH: 3
};

const storeMap = [
  // Getters
  {
    type: storeTypeEnum.GETTER,
    content: require("@/store/getters/general")
  },
  {
    type: storeTypeEnum.GETTER,
    content: require("@/store/getters/metadata")
  },
  {
    type: storeTypeEnum.GETTER,
    content: require("@/store/getters/users")
  },

  // Actions
  {
    type: storeTypeEnum.ACTION,
    content: require("@/store/actions/metadata")
  },
  {
    type: storeTypeEnum.ACTION,
    content: require("@/store/actions/requests")
  },
  {
    type: storeTypeEnum.ACTION,
    content: require("@/store/actions/stubs")
  },
  {
    type: storeTypeEnum.ACTION,
    content: require("@/store/actions/tenants")
  },
  {
    type: storeTypeEnum.ACTION,
    content: require("@/store/actions/users")
  },

  // Mutations
  {
    type: storeTypeEnum.MUTATION,
    content: require("@/store/mutations/general")
  },
  {
    type: storeTypeEnum.MUTATION,
    content: require("@/store/mutations/metadata")
  },
  {
    type: storeTypeEnum.MUTATION,
    content: require("@/store/mutations/users")
  }
];

export function constructStore(state) {
  let result = {
    state: state,
    mutations: {},
    actions: {},
    getters: {}
  };
  for (let storeResult of storeMap) {
    let typeKey = "";
    switch (storeResult.type) {
      case storeTypeEnum.GETTER:
        typeKey = "getters";
        break;
      case storeTypeEnum.MUTATION:
        typeKey = "mutations";
        break;
      case storeTypeEnum.ACTION:
        typeKey = "actions";
        break;
    }

    for (let key in storeResult.content) {
      if (storeResult.type === storeTypeEnum.WATCH) {
        storeResult.content[key]();
      } else {
        let value = storeResult.content[key];
        if (typeof value === "function") {
          result[typeKey][key] = value;
        }
      }
    }
  }

  result.getters.getField = getField;
  result.mutations.updateField = updateField;
  return result;
}
