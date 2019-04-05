const downloadBlob = (filename, data) => {
    let blob = data;
    if (typeof data === 'string') {
        blob = new Blob([data], { type: 'application/octet-stream' });
    }

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        // IE does not allow direct links to blobs.
        // See also: https://stackoverflow.com/questions/20310688/blob-download-is-not-working-in-ie
        window.navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        // Recommended way of saving blobs through angular. Direct link does not work due to authentication.
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;

        // FireFox needs this line, because else the click won't work.
        document.body.appendChild(link);
        link.click();

        // Delete the link with a delay (because of FireFox)
        // https://stackoverflow.com/questions/30694453/blob-createobjecturl-download-not-working-in-firefox-but-works-when-debugging
        setTimeout(function () {
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        }, 100);
    }
}

export {
    downloadBlob
}