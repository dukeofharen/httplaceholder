// Source: https://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format
// TODO remove this and replace all with sprintf-js
const prototype = String.prototype as any;
if (!prototype.format) {
  prototype.format = function (...args: any[]) {
    return this.replace(/{(\d+)}/g, function (match: any, number: any) {
      return typeof args[number] != "undefined" ? args[number] : match;
    });
  };
}

export default {};
