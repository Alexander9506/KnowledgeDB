export function copyToClipboard(text) {
    let body = document.getElementsByTagName("body")[0];
    let tmpInput = document.createElement("Input");
    tmpInput.type = "text";
    tmpInput.value = text;
    tmpInput.style.position = 'fixed';
    tmpInput.style.top = '0';
    tmpInput.style.left = '0';
    // Ensure it has a small width and height. Setting to 1px / 1em
    // doesn't work as this gives a negative w/h on some browsers.
    tmpInput.style.width = '1px';
    tmpInput.style.height = '1px';
    // We don't need padding, reducing the size if it does flash render.
    tmpInput.style.padding = '0';
    // Clean up any borders.
    tmpInput.style.border = 'none';
    tmpInput.style.outline = 'none';
    tmpInput.style.boxShadow = 'none';
    // Avoid flash of white box if rendered for any reason.
    tmpInput.style.background = 'transparent';
    body.appendChild(tmpInput);
    tmpInput.select();
    document.execCommand("copy");
    body.removeChild(tmpInput);
}
