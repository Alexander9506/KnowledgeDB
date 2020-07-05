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


async function UploadFiles(files, action, onUploadFinished) {
    let formData = new FormData();
    for (var i = 0; i < files.length; i++) {
        formData.append("files", files[i]);
    }

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
