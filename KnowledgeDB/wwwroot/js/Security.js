export function uuidv4() {
    return ("" + 1e7 + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c => (parseFloat(c) ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> parseFloat(c) / 4).toString());
}
