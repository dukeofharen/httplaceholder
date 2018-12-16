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
        name: "Light theme",
        className: 'light-theme',
        codeMirrorTheme: 'solarized light'
    },
    darkTheme: {
        name: "Dark theme",
        className: 'dark-theme',
        codeMirrorTheme: 'solarized dark'
    }
}

export {
    authenticateResults,
    messageTypes,
    themes
}