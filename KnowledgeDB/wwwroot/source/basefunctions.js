var deleteConfirmDialogButtons = document.querySelectorAll(".confirm-delete-dialog")

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


async function UplodeFiles(inputId, action) {
    var inputElement = document.getElementById(inputId);
    if (inputElement) {
        var formData = new FormData();
        for (var i = 0; i < inputElement.files.length; i++) {
            formData.append("files", inputElement.files[i]);
        }

        try {
            const response = await fetch(action, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                console.log('Fertig');
            }
        } catch (error) {
            console.error('Error', error);
        }
    }

}

async function UploadFile() {
    var tmpForm = document.getElementById('form');
    var action = tmpForm.action;

    const formData = new FormData(tmpForm);

    try {
        const response = await fetch(action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            console.log('Fertig');
        }
    } catch (error) {
        console.error('Error', error);
    }
}