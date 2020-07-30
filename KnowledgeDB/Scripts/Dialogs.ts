let deleteConfirmDialogButtons: NodeListOf<HTMLElement> = document.querySelectorAll(".confirm-delete-dialog") as NodeListOf<HTMLElement>;

for (var i = 0; i < deleteConfirmDialogButtons.length; i++) {
    deleteConfirmDialogButtons[i].addEventListener("click", function (event : MouseEvent) {
        event.preventDefault();
        var result : boolean = confirm(this.dataset.question);

        if (result) {
            var rootForm: HTMLFormElement = document.createElement("form") as HTMLFormElement;
            rootForm.method = "post"
            rootForm.action = this.getAttribute("href");

            var idElement: HTMLInputElement = document.createElement("input") as HTMLInputElement;
            idElement.type = "hidden";
            idElement.name = "ID";
            idElement.value = this.dataset.id;

            rootForm.appendChild(idElement);

            this.parentElement.appendChild(rootForm);
            rootForm.submit();
        }
    });
}