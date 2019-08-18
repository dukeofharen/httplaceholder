import Vue from 'vue'
import yaml from 'js-yaml'

Vue.filter('yaml', (value) => {
    return yaml.dump(value)
})