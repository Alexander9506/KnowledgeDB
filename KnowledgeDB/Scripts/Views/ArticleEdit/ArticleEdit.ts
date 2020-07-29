import * as GuiCreation from "../../GuiCreation.js";
import { FileExplorer } from "../../FileExplorer.js";

addButtonListener();
addSubmitListener();

let fileExplorer : FileExplorer = null;

function addButtonListener() : void {
    let saveButton : HTMLButtonElement = document.getElementById("saveButton") as HTMLButtonElement;
    saveButton.onclick = function () {
        var submitButton = document.getElementById("submitButton")
        submitButton.click();
    };

    let toggleFileExplorerButton: HTMLButtonElement = document.getElementById("toggleFileExplorer") as HTMLButtonElement
    toggleFileExplorerButton.onclick = function () {
        toggleFileExplorer();
    };

    let reloadButton: HTMLButtonElement = document.getElementById("reloadFiles") as HTMLButtonElement;
    reloadButton.onclick = function () {
        if (fileExplorer) {
            fileExplorer.refresh();
        }
    }

}

function addSubmitListener() {
    var element = document.getElementById("articleForm");
    if (element.addEventListener) {
        element.addEventListener("submit", function (e) { onSubmit(e); }, false);
    }
}

function onSubmit(e) {
    try {
        AddTagsToForm("articleTags", "articleForm");
    } catch (err) {
        e.preventDefault();
    }
}

function AddTagsToForm(tagElementId, formElementId) {
    var form = document.getElementById(formElementId);
    var tagList = GetTagList(tagElementId)

    var tagsContainer = document.createElement("div");
    for (var i = 0; i < tagList.length; i++) {

        var tagInputElement : HTMLInputElement = document.createElement("input") as HTMLInputElement;

        tagInputElement.name = "RefToTags[" + i + "].ArticelTag.Name";
        tagInputElement.type = "hidden";
        tagInputElement.value = tagList[i];

        tagsContainer.appendChild(tagInputElement);
    }
    form.appendChild(tagsContainer);
    console.log(tagList);
}

//Split and Clean Input of TagElement
function GetTagList(tagElementId) : string[] {
    var tagListElement : HTMLInputElement = document.getElementById(tagElementId) as HTMLInputElement;
    var tagFieldValue : string = tagListElement.value;
    var tagList : string[] = tagFieldValue.split(",").map((item) => item.replace("#", "").trim());
    tagList = tagList.filter(name => name.length > 0);

    return tagList;
}

function toggleFileExplorer() : void {
    //Expand sidebar to fit the FileExplorer
    GuiCreation.toggleSideBarExpansion();
    toggleOffset();

    toggleAddFileExplorer();
}

//TODO: improve! must be cleaner and generally useable
function toggleOffset() {
    let main = document.getElementById("main");

    main.classList.toggle("offset-xl-1")
}



function toggleAddFileExplorer() {
    let fileExplorerElement = document.getElementById("file-explorer");

    if (fileExplorerElement.classList.contains("d-none")) {
        initFileExplorer();
    } else {
        fileExplorer = null;
    }
    fileExplorerElement.classList.toggle("d-none");
}

function initFileExplorer() {
    fileExplorer = new FileExplorer('file-explorer');

    fileExplorer.ImageUrl = (document.getElementById("ImageURL") as HTMLSpanElement).innerHTML;
    fileExplorer.DeleteFileURL = (document.getElementById("DeleteFileURL") as HTMLSpanElement).innerHTML;
    fileExplorer.UploadFileURL = (document.getElementById("UploadURL") as HTMLSpanElement).innerHTML;

    fileExplorer.CreatePreviewImageURL = (file) => {
        if ('URL' in file) {
            return file.URL + "?width=80&height=80&keepRatio=true&imbedInBackground=true";
        } else {
            return ""
        }
    }
    fileExplorer.refresh();
}