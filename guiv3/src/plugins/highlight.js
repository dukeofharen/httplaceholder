import hljs from "highlight.js/lib/core";
import yaml from "highlight.js/lib/languages/yaml";
import "./highlight/dark-theme.scss";
import "./highlight/light-theme.scss";

hljs.registerLanguage("yaml", yaml);
