"use strict";


class FileExplorer {
    constructor(fileExplorerId) {
        this.fileExplorerId = fileExplorerId;
        this.fileList = [];

        // Element IDs
        this.ID_fileExplorerSearch = 'file-explorer-search';
        this.ID_fileExplorerAddFile = 'file-explorer-add-file';
        this.ID_fileExplorerSettings = 'file-explorer-settings';
        this.ID_fileExplorerCbxOnlyNewFiles = 'file-explorer-cbox-only-new-files';
        this.ID_fileExplorerCbxPreviewImages = 'file-explorer-cbox-preview-images';
        this.ID_fileExplorerFiles = 'file-explorer-files';

        //File Upload States
        this.UploadState_Added = "added";
        this.UploadState_OnUpload = "onUpload";
        this.UploadState_Uploaded = "uploaded";

        //Css
        this.CSS_PreviewImage = "file-explorer-previewimage";

        //cbx
        this.ShowPreviewImage = false;

        //Create URL for previewImage
        this.CreatePreviewImageURL = null;

        this.ImageUrl = null;
        this.Filter = {
            FileType: "image"
        }
        this.addListener();
    }

    addListener() {
        const fileAddButton = document.getElementById(this.ID_fileExplorerAddFile);
        const self = this;

        //New file added listener
        fileAddButton.onchange = function () {
            let newFiles = [];

            newFiles.push.apply(newFiles, this.files);
            self.addFiles(newFiles);

            //remove file from Input to ensure it can be uploaded again
            this.value = null;

        }

        const search = document.getElementById(this.ID_fileExplorerSearch);
        if (search != null) {
            search.addEventListener("input", function (e) {
                self.refresh(search.value);
            });
        }

        const previewImageCbx = document.getElementById(this.ID_fileExplorerCbxPreviewImages);
        if (previewImageCbx != null) {
            previewImageCbx.addEventListener("change", (e) => {
                self.ShowPreviewImage = e.target.checked;
                self.refresh();
            });
        }
    }

    getSearchString() {
        const search = document.getElementById(this.ID_fileExplorerSearch);
        if (search != null) {
            return search.value;
        }
        return null;
    }

    addFiles(files) {
        const self = this;
        const transformedFiles = files.map(function (f) {
            const id = self.generateFileId(f)
            return { file: f, fileId: id, guiId: id, uploadState: self.UploadState_Added };
        });
        this.fileList.push.apply(this.fileList, transformedFiles);
        this.refreshFileView(this.fileList);
    }

    generateFileId(file) {
        return uuidv4() + file.name;
    }

    getFileById(fileId) {
        for (let i = 0; i < this.fileList.length; i++) {
            let file = this.fileList[i];
            if (file.id === fileId) {
                return file;
            }
        }
    }

    refresh(searchString = null) {
        let files = null;
        if (searchString != null) {
            files = this.filterFileListBySearch(this.fileList, this.getSearchString());
        } else {
            files = this.fileList;
        }

        this.refreshFileView(files);
    }

    refreshFileView(files) {
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
        if ('URL' in file) {
            colActions.appendChild(this.createCopyURLButton(file));
        }

        //Add upload button
        if (file.uploadState === this.UploadState_Added) {
            colActions.appendChild(this.createUploadButton(file));
        }

        //Add remove button
        colActions.appendChild(this.createRemoveButton(file));

        newRow.appendChild(colName);
        newRow.appendChild(colActions);

        return newRow;
    }

    createPreviewImage(file) {
        let previewImage = document.createElement('img');
        previewImage.className = this.CSS_PreviewImage;
        if (this.CreatePreviewImageURL != null) {
            previewImage.src = this.CreatePreviewImageURL(file);
        } else {
            previewImage.src = "#"
            console.error('Can not create PreviewImageURL: CreatePreviewImageURL == null')
        }

        return previewImage;
    }

    createName(file) {
        let colName;
        if ('URL' in file) {
            //file name as link if URL is provided
            colName = document.createElement('a');
            colName.href = file.URL;
        } else {
            //only file name if no URL
            colName = document.createElement('span');
        }
        colName.textContent = file.file.name;

        return colName;
    }

    createRemoveButton(file) {
        let self = this;
        let removeButton = createImageButton("fas fa-trash", "btn btn-outline-danger m-1", "Remove")
        removeButton.onclick = function () {
            if (file.fileId > 0) {
                self.deleteFileOnServer(file, (f) => self.deleteFileLocal(f));
            } else {
                self.deleteFileLocal(file);
            }
        }
        return removeButton;
    }

    createUploadButton(file) {
        const self = this;
        let uploadButton = createImageButton("fas fa-upload", "btn btn-outline-info m-1", "Upload")
        uploadButton.onclick = function () {
            const newImage = replaceButtonWithImage(this, "fa fa-spinner fa-spin m-3")
            file.uploadState = self.UploadState_OnUpload;

            if (self.UploadUrl != null) {
                let formData = self.createUploadFormData([file]);
                UploadFormData(formData, self.UploadUrl, function (success) {
                    ChangeImage(newImage, "fa fa-check m-3");

                    //ChangeUpload State
                    file.uploadState = self.UploadState_Uploaded;

                    //switchout check with copy url Button
                    setTimeout(
                        function () {
                            self.loadServerFiles();
                        }, 1000);
                });
            } else {
                console.error("No Upload URL defined");
            }
        }
        return uploadButton;
    }

    createCopyURLButton(file) {
        const buttonCopyUrl = createImageButton("far fa-copy", "btn btn-outline-info m-1", "Copy to Clipboard")
        buttonCopyUrl.onclick = function () {
            copyToClipboard(file.URL);
        };
        return buttonCopyUrl;
    }

    deleteFileLocal(file) {
        this.fileList = this.fileList.filter(f => f.fileId !== file.fileId);

        let filteredBySearch = this.filterFileListBySearch(this.fileList, this.getSearchString());
        this.refreshFileView(filteredBySearch);
    }

    filterFileListBySearch(fileList, search) {
        if (fileList != null && search != null) {
            let searchSplit = search.toLowerCase().split(" ");
            let filteredList = fileList.filter(f => {
                //check if any searchstring matches
                for (var i = 0; i < searchSplit.length; i++) {
                    if (f.file.name.toLowerCase().includes(searchSplit[i])) {
                        return true;
                    }
                }
                return false;
            });
            return filteredList;
        } else {
            return fileList;
        }
    }

    async deleteFileOnServer(file, onFileDelete) {
        if (file == null || this.DeleteFileUrl == null) {
            return;
        }

        let formData = new FormData();
        formData.append("id", file.fileId)
        try {
            const response = await fetch(this.DeleteFileUrl, {
                method: "POST",
                body: formData
            });
            if (response.ok) {
                if (onFileDelete != null) {
                    onFileDelete(file);
                }
            }
        } catch (error) {
            console.error('Error', error);
        }
    }

    async loadServerFiles() {
        const self = this;
        if (this.ImageUrl != null) {
            try {
                const response = await fetch(this.ImageUrl, {
                    method: 'Post',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    //Only Show Images
                    body: JSON.stringify(self.filter)
                })

                if (response.ok) {
                    let rawJson = response.json();
                    rawJson.then(function (json) {
                        const serverFiles = json;
                        const transformedFiles = serverFiles.map(function (fc) {
                            return {
                                file: { name: fc.displayName }, fileId: fc.id, guiId: fc.id, uploadState: self.UploadState_Uploaded, URL: fc.fileUrl
                            };
                        });
                        //remove all local files from the "server"
                        self.fileList = self.fileList.filter(f => f.uploadState === self.UploadState_Added || f.uploadState === self.UploadState_OnUpload);
                        //add new loaded server files
                        self.fileList.push.apply(self.fileList, transformedFiles);

                        let filteredBySearch = self.filterFileListBySearch(self.fileList, self.getSearchString());
                        self.refreshFileView(filteredBySearch);
                    })
                }
            } catch (error) {
                console.error('Error', error);
            }
        }
    }

    createUploadFormData(files) {
        const formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            //needed to supply multiple Objects from same type to asp.net controller
            formData.append("Files[" + i + "].FormFile", files[i].file);

            let guiid = files[i].guiId;
            if (guiid == null) {
                guiid = "";
            }
            //needed to supply multiple Objects from same type to asp.net controller
            formData.append("Files[" + i + "].GUIID", guiid);
        }
        return formData;
    }
}