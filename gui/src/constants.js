const authenticateResults = {
    NOT_SET: 'not_set',
    INVALID_CREDENTIALS: 'invalid_credentials',
    OK: 'ok',
    INTERNAL_SERVER_ERROR: 'internal_server_error'
}

const messageTypes = {
    INFO: 'info',
    SUCCESS: 'success',
    WARNING: 'warning',
    ERROR: 'error'
}

const themes = {
    lightTheme: {
        className: 'light-theme',
        codeMirrorTheme: 'solarized light'
    }
}

export {
    authenticateResults,
    messageTypes,
    themes
}