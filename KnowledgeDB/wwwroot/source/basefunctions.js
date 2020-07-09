let deleteConfirmDialogButtons = document.querySelectorAll(".confirm-delete-dialog")

for (var i = 0; i < deleteConfirmDialogButtons.length; i++) {
    deleteConfirmDialogButtons[i].addEventListener("click", function (event) {
        event.preventDefault();
        console.log(this.getAttribute("href"));
        console.log(this.dataset.question);
        var result = confirm(this.dataset.question);

        if (result) {
            var rootForm = document.createElement("form");
            rootForm.method = "post"
            rootForm.action = this.getAttribute("href");

            var idElement = document.createElement("input");
            idElement.type = "hidden";
            idElement.name = "ID";
            idElement.value = this.dataset.id;

            rootForm.appendChild(idElement);

            this.parentElement.appendChild(rootForm);
            rootForm.submit();
        }
    });
}


async function UploadFormData(formData, action, onUploadFinished) {
    try {
        const response = await fetch(action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            if (onUploadFinished != null) {
                onUploadFinished(true);
            }
        }
    } catch (error) {
        console.error('Error', error);
        onUploadFinished(false);
    }
}

function copyToClipboard(text) {
    let body = document.getElementsByTagName("body")[0];

    let tmpInput = document.createElement("Input");
    tmpInput.type = "text";
    tmpInput.value = text;

    tmpInput.style.position = 'fixed';
    tmpInput.style.top = 0;
    tmpInput.style.left = 0;

    // Ensure it has a small width and height. Setting to 1px / 1em
    // doesn't work as this gives a negative w/h on some browsers.
    tmpInput.style.width = '1px';
    tmpInput.style.height = '1px';

    // We don't need padding, reducing the size if it does flash render.
    tmpInput.style.padding = 0;

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