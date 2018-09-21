const setItem = (key, value) => {
    let serializedValue = JSON.stringify(value);
    sessionStorage[key] = serializedValue;
};

const getItem = (key) => {
    let value = sessionStorage[key];
    if(!value) {
        return null;
    }
    
    let deserializedValue = JSON.parse(value);
    return deserializedValue;
};

export {
    setItem,
    getItem
}