function fallbackCopyTextToClipboard(text: string): void {
  const textArea = document.createElement("textarea");
  textArea.value = text;

  // Avoid scrolling to bottom
  textArea.style.top = "0";
  textArea.style.left = "0";
  textArea.style.position = "fixed";

  document.body.appendChild(textArea);
  textArea.focus();
  textArea.select();

  document.execCommand("copy");
  document.body.removeChild(textArea);
}

export function copyTextToClipboard(text: string): Promise<void> {
  if (!navigator.clipboard) {
    return new Promise((resolve, reject) => {
      try {
        fallbackCopyTextToClipboard(text);
        resolve();
      } catch (e) {
        reject(e);
      }
    });
  }

  return navigator.clipboard.writeText(text);
}
