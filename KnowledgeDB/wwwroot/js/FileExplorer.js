import * as Security from "./Security.js";
import * as GuiCreation from "./GuiCreation.js";
import * as ClientSideFunctions from "./ClientSideFunctions.js";
import * as ServerSideFunctions from "./ServerSideFunctions.js";
var UploadStates;
(function (UploadStates) {
    UploadStates["Added"] = "added";
    UploadStates["OnUpload"] = "onUpload";
    UploadStates["Uploaded"] = "uploaded";
})(UploadStates || (UploadStates = {}));
class FileFilter {
    constructor() {
        this.fileType = "";
        this.name = "";
    }
    checkFilter(file) {
        if (!this.filterByFileType(file, this.fileType)) {
            return false;
        }
        if (!this.filterByFileName(file, this.name)) {
            return false;
        }
        return true;
    }
    filterByFileName(file, name) {
        if (name != null && name !== "") {
            let searchSplit = name.toLowerCase().split(" ");
            for (var i = 0; i < searchSplit.length; i++) {
                if (searchSplit[i] !== "" && file.file.name.toLowerCase().includes(searchSplit[i])) {
                    return true;
                }
            }
        }
        else {
            //No Filter
            return true;
        }
        return false;
    }
    filterByFileType(file, type) {
        let filterSurvived = false;
        if (type != null && file != null && file.file != null && file.file.type != null) {
            if (type.length < file.file.type.length) {
                let fileTypeSub = file.file.type.substring(0, type.length);
                if (fileTypeSub.toLowerCase() === type) {
                    filterSurvived = true;
                }
            }
        }
        else {
            //No Filter
            return true;
        }
        return filterSurvived;
    }
    addFilter(filter) {
        //TODO: don't know if this works
        Object.keys(this).forEach(key => {
            let prop = filter[key];
            if (prop != null) {
                this[key] = prop;
            }
        });
    }
}
class FileContainer {
    constructor(file, fileId, guiId, uploadState, URL = "") {
        this.file = file;
        this.fileId = fileId;
        this.guiId = guiId;
        this.uploadState = uploadState;
        this.URL = URL;
    }
}
export class FileExplorer {
    constructor(fileExplorerId) {
        this.ID_fileExplorer = 'file-explorer';
        this.ID_fileExplorerSearch = 'file-explorer-search';
        this.ID_fileExplorerAddFile = 'file-explorer-add-file';
        this.ID_fileExplorerSettings = 'file-explorer-settings';
        this.ID_fileExplorerCbxOnlyNewFiles = 'file-explorer-cbox-only-new-files';
        this.ID_fileExplorerCbxPreviewImages = 'file-explorer-cbox-preview-images';
        this.ID_fileExplorerFiles = 'file-explorer-files';
        this.ShowPreviewImage = false;
        this.CreatePreviewImageURL = null;
        this.ImageUrl = null;
        this.DeleteFileURL = null;
        this.UploadFileURL = null;
        this._filter = null;
        this._files = [];
        //Css
        this.CSS_PreviewImage = "file-explorer-previewimage";
        this.ID_fileExplorer = fileExplorerId;
        this._filter = new FileFilter();
        this._filter.fileType = "image";
        this.addListener();
    }
    addListener() {
        const fileAddButton = document.getElementById(this.ID_fileExplorerAddFile);
        const self = this;
        //New file added listener
        fileAddButton.onchange = function (e) {
            const target = event.target;
            let newFiles = [];
            console.log(JSON.stringify(target.files));
            newFiles.push.apply(newFiles, target.files);
            self.addFiles(newFiles);
            //remove file from Input to ensure it can be uploaded again
            target.value = null;
        };
        const searchInput = document.getElementById(this.ID_fileExplorerSearch);
        if (searchInput != null) {
            searchInput.addEventListener("input", function (e) {
                self.search(searchInput.value);
            });
        }
        const previewImageCbx = document.getElementById(this.ID_fileExplorerCbxPreviewImages);
        if (previewImageCbx != null) {
            previewImageCbx.addEventListener("change", (e) => {
                const target = e.target;
                self.ShowPreviewImage = target.checked;
                self.refreshGUI();
            });
        }
    }
    addFiles(files) {
        const self = this;
        const transformedFiles = files.map((f) => {
            const id = self.generateFileId(f);
            return new FileContainer(f, id, id, UploadStates.Added);
        });
        this._files.push.apply(this._files, transformedFiles);
        this.showFiles(this._files);
    }
    generateFileId(file) {
        return Security.uuidv4() + file.name;
    }
    search(search = "") {
        let searchFilter = new FileFilter();
        searchFilter.name = search;
        this._filter.addFilter(searchFilter);
        this.refreshGUI();
    }
    refresh() {
        this.loadServerFiles(() => {
            this.refreshGUI();
        });
    }
    refreshGUI() {
        let files = this.filterFiles(this._filter, this._files);
        this.showFiles(files);
    }
    filterFiles(fileFilter, files) {
        let filteredFiles = files.filter(fc => {
            return fileFilter.checkFilter(fc);
        });
        return filteredFiles;
    }
    showFiles(files) {
        const fileListView = document.getElementById(this.ID_fileExplorerFiles);
        if (fileListView == null) {
            return;
        }
        //TODO: make faster!!!
        //Delete all Contant in the List 
        fileListView.innerHTML = '';
        for (var i = 0; i < files.length; i++) {
            let newFileCard = this.createFileViewCard(files[i]);
            fileListView.appendChild(newFileCard);
        }
    }
    createFileViewCard(file) {
        let newRow = document.createElement('li');
        newRow.dataset.fileId = file.fileId;
        newRow.className = 'justify-content-between list-group-item d-flex justify-content-between';
        let colActions = document.createElement('div');
        let colName = document.createElement('div');
        if (this.ShowPreviewImage === true) {
            let previewImage = this.createPreviewImage(file);
            colName.appendChild(previewImage);
        }
        colName.appendChild(this.createName(file));
        //Add CopyUrl Button
        if (file.URL != null && file.URL !== "") {
            colActions.appendChild(this.createCopyURLButton(file));
        }
        //Add upload button
        if (file.uploadState === UploadStates.Added) {
            colActions.appendChild(this.createUploadButton(file));
        }
        //Add remove button
        colActions.appendChild(this.createRemoveButton(file));
        newRow.appendChild(colName);
        newRow.appendChild(colActions);
        return newRow;
    }
    createCopyURLButton(file) {
        const buttonCopyUrl = GuiCreation.createImageButton("far fa-copy", "btn btn-outline-info m-1", "Copy to Clipboard");
        buttonCopyUrl.onclick = function () {
            ClientSideFunctions.copyToClipboard(file.URL);
        };
        return buttonCopyUrl;
    }
    createPreviewImage(file) {
        let previewImage = document.createElement('img');
        previewImage.className = this.CSS_PreviewImage;
        if (this.CreatePreviewImageURL != null) {
            previewImage.src = this.CreatePreviewImageURL(file);
        }
        else {
            previewImage.src = "#";
            console.error('Can not create PreviewImageURL: CreatePreviewImageURL == null');
        }
        return previewImage;
    }
    createName(file) {
        let col;
        if (file.URL != null && file.URL !== "") {
            //file name as link if URL is provided
            let colAnchor = document.createElement('a');
            colAnchor.href = file.URL;
            col = colAnchor;
        }
        else {
            //only file name if no URL
            col = document.createElement('span');
        }
        col.textContent = file.file.name;
        return col;
    }
    createRemoveButton(file) {
        let self = this;
        let removeButton = GuiCreation.createImageButton("fas fa-trash", "btn btn-outline-danger m-1", "Remove");
        removeButton.onclick = function () {
            if (file.fileId !== "") {
                self.deleteFileOnServer(file, (f) => self.deleteFileLocal(f));
            }
            else {
                self.deleteFileLocal(file);
            }
        };
        return removeButton;
    }
    deleteFileLocal(file) {
        this._files = this._files.filter(f => f.fileId !== file.fileId);
        this.refreshGUI();
    }
    async deleteFileOnServer(file, onFileDelete) {
        if (file == null || this.DeleteFileURL == null) {
            return;
        }
        let formData = new FormData();
        formData.append("id", file.fileId);
        try {
            const response = await fetch(this.DeleteFileURL, {
                method: "POST",
                body: formData
            });
            if (response.ok) {
                if (onFileDelete != null) {
                    onFileDelete(file);
                }
            }
        }
        catch (error) {
            console.error('Error', error);
        }
    }
    createUploadButton(file) {
        const self = this;
        let uploadButton = GuiCreation.createImageButton("fas fa-upload", "btn btn-outline-info m-1", "Upload");
        uploadButton.onclick = function () {
            const newImage = GuiCreation.replaceButtonWithImage(this, "fa fa-spinner fa-spin m-3");
            file.uploadState = UploadStates.OnUpload;
            if (self.UploadFileURL != null) {
                let formData = self.createUploadFormData([file]);
                ServerSideFunctions.uploadFormData(formData, self.UploadFileURL, function (success) {
                    if (success) {
                        GuiCreation.changeImage(newImage, "fa fa-check m-3");
                        //ChangeUpload State
                        file.uploadState = UploadStates.Uploaded;
                    }
                    else {
                        file.uploadState = UploadStates.Added;
                        GuiCreation.changeImage(newImage, "fa fa-exclamation-triangle m-3");
                    }
                    //switchout check with copy url Button
                    setTimeout(function () {
                        self.refresh();
                    }, 1000);
                });
            }
            else {
                console.error("No Upload URL defined");
            }
        };
        return uploadButton;
    }
    createUploadFormData(files) {
        let formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            //needed to supply multiple Objects from same type to asp.net controller
            let fileCont = files[i];
            formData.append("Files[" + i + "].FormFile", fileCont.file);
            let guiid = fileCont.guiId;
            if (guiid == null) {
                guiid = "";
            }
            //needed to supply multiple Objects from same type to asp.net controller
            formData.append("Files[" + i + "].GUIID", guiid);
        }
        for (var pair of formData.entries()) {
            console.log('creatUploadFormData: ' + pair[0] + ', ' + pair[1]);
        }
        return formData;
    }
    async loadServerFiles(filesLoadedCallback) {
        const self = this;
        if (this.ImageUrl != null) {
            try {
                const response = await fetch(this.ImageUrl, {
                    method: 'Post',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    //Only Show Images
                    body: JSON.stringify(self._filter)
                });
                if (response.ok) {
                    let rawJson = response.json();
                    rawJson.then(function (json) {
                        const serverFiles = json;
                        const transformedFiles = serverFiles.map(function (serverFileModel) {
                            let container = {
                                file: {
                                    name: serverFileModel.displayName,
                                    type: serverFileModel.fileType,
                                },
                                fileId: serverFileModel.id,
                                guiId: serverFileModel.id,
                                uploadState: UploadStates.Uploaded,
                                URL: serverFileModel.fileUrl
                            };
                            return container;
                        });
                        //remove all local files from the "server"
                        self._files = self._files.filter(f => f.uploadState === UploadStates.Added || f.uploadState === UploadStates.OnUpload);
                        //add new loaded server files
                        self._files.push.apply(self._files, transformedFiles);
                        if (filesLoadedCallback != null) {
                            filesLoadedCallback();
                        }
                    });
                }
            }
            catch (error) {
                console.error('Error', error);
            }
        }
    }
}
