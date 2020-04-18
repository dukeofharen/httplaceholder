// Source: https://stackoverflow.com/questions/8486099/how-do-i-parse-a-url-query-parameters-in-javascript
const parseUrl = url => {
  if (!url) url = location.href;
  const question = url.indexOf("?");
  let hash = url.indexOf("#");
  if (hash === -1 && question === -1) return {};
  if (hash === -1) hash = url.length;
  const query =
    question === -1 || hash === question + 1
      ? url.substring(hash)
      : url.substring(question + 1, hash);
  const result = {};
  query.split("&").forEach(function(part) {
    if (!part) return;
    part = part.split("+").join(" "); // replace every + with space, regexp-free version
    const eq = part.indexOf("=");
    let key = eq > -1 ? part.substr(0, eq) : part;
    const val = eq > -1 ? decodeURIComponent(part.substr(eq + 1)) : "";
    const from = key.indexOf("[");
    if (from === -1) result[decodeURIComponent(key)] = val;
    else {
      const to = key.indexOf("]", from);
      const index = decodeURIComponent(key.substring(from + 1, to));
      key = decodeURIComponent(key.substring(0, from));
      if (!result[key]) result[key] = [];
      if (!index) result[key].push(val);
      else result[key][index] = val;
    }
  });
  return result;
};

export { parseUrl };
