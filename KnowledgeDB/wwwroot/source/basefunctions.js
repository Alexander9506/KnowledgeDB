var deleteConfirmDialogButtons = document.querySelectorAll(".confirm-delete-dialog")

for (var i = 0; i < deleteConfirmDialogButtons.length; i++) {
    deleteConfirmDialogButtons[i].addEventListener("click", function (e) {
        event.preventDefault();

        var result = confirm(this.getAttribute("data-question"));

        if (result) {
            var rootForm = document.createElement("form");
            rootForm.method = "post"
            rootForm.action = this.getAttribute("href");

            var idElement = document.createElement("input");
            idElement.type = "hidden";
            idElement.name = "ID";
            idElement.value = this.getAttribute("data-id");

            rootForm.appendChild(idElement);

            rootForm.submit();
        }
    });
}