﻿import * as Security from "./Security.js";
import * as GuiCreation from "./GuiCreation.js";
import * as ClientSideFunctions from "./ClientSideFunctions.js";
import * as ServerSideFunctions from "./ServerSideFunctions.js";

enum UploadStates {
    Added = "added",
    OnUpload = "onUpload",
    Uploaded = "uploaded"
}

interface IFileFilter {
    fileType: string;
    name: string;
}

type File = {
    name: string;
    type: string;
}

class FileFilter implements IFileFilter{
    fileType: string = "";
    name: string = "";

    public checkFilter(file: FileContainer): boolean {
        if (!this.filterByFileType(file, this.fileType)){
            return false;
        }

        if (!this.filterByFileName(file, this.name)) {
            return false;
        }

        return true;
    }

    private filterByFileName(file: FileContainer, name: string): boolean {
        if (name != null && name !== "") {
            let searchSplit: string[] = name.toLowerCase().split(" ");

            for (var i = 0; i < searchSplit.length; i++) {
                if (searchSplit[i] !== "" && file.file.name.toLowerCase().includes(searchSplit[i])) {
                    return true;
                }
            }
        } else {
            //No Filter
            return true;
        }
        return false;
    }

    private filterByFileType(file: FileContainer, type: string): boolean{
        let filterSurvived: boolean = false;
        if (type != null && file != null && file.file != null && file.file.type != null) {
            if (type.length < file.file.type.length) {
                let fileTypeSub: string = file.file.type.substring(0, type.length);
                if (fileTypeSub.toLowerCase() === type) {
                    filterSurvived = true;
                }
            }
        } else {
            //No Filter
            return true;
        }
        return filterSurvived;
    }

    public addFilter(filter: IFileFilter) {
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
    constructor(
        public file: File,
        public fileId: string,
        public guiId: string,
        public uploadState: UploadStates,
        public URL: string = "") {
    }
}

export class FileExplorer {
    private ID_fileExplorer: string = 'file-explorer';
    private ID_fileExplorerSearch : string  = 'file-explorer-search' ;
    private ID_fileExplorerAddFile: string = 'file-explorer-add-file';
    private ID_fileExplorerSettings: string = 'file-explorer-settings';
    private ID_fileExplorerCbxOnlyNewFiles: string = 'file-explorer-cbox-only-new-files';
    private ID_fileExplorerCbxPreviewImages: string = 'file-explorer-cbox-preview-images';
    private ID_fileExplorerFiles: string = 'file-explorer-files';

    public ShowPreviewImage: Boolean = false;
    public CreatePreviewImageURL : Function = null;
    public ImageUrl : string = null;
    public DeleteFileURL : string = null;
    public UploadFileURL : string = null;

    private _filter: FileFilter = null;
    private _files: FileContainer[] = [];

    //Css
    private CSS_PreviewImage: string = "file-explorer-previewimage";


    constructor(fileExplorerId: string) {
        this.ID_fileExplorer = fileExplorerId;

        this._filter = new FileFilter();
        this._filter.fileType = "image";

        this.addListener();
    }

    private addListener() : void {
        const fileAddButton: HTMLInputElement = document.getElementById(this.ID_fileExplorerAddFile) as HTMLInputElement;
        const self : FileExplorer = this;

        //New file added listener
        fileAddButton.onchange = function (e: Event) {
            const target = event.target as HTMLInputElement;

            let newFiles: Array<File> = [];

            console.log(JSON.stringify(target.files));

            newFiles.push.apply(newFiles, target.files);
            self.addFiles(newFiles);

            //remove file from Input to ensure it can be uploaded again
            target.value = null;
        }

        const searchInput : HTMLInputElement = document.getElementById(this.ID_fileExplorerSearch) as HTMLInputElement;
        if (searchInput != null) {
            searchInput.addEventListener("input", function (e) {
                self.search(searchInput.value);
            });
        }

        const previewImageCbx : HTMLInputElement = document.getElementById(this.ID_fileExplorerCbxPreviewImages) as HTMLInputElement;
        if (previewImageCbx != null) {
            previewImageCbx.addEventListener("change", (e) => {
                const target = e.target as HTMLInputElement;
                self.ShowPreviewImage = target.checked;
                self.refreshGUI();
            });
        }
    }

    private addFiles(files: Array<File>) {
        const self : FileExplorer = this;
        const transformedFiles:FileContainer[] = files.map((f) => {
            const id = self.generateFileId(f);
            return new FileContainer(f,  id, id, UploadStates.Added);
        });
        this._files.push.apply(this._files, transformedFiles);
        this.showFiles(this._files);
    }

    private generateFileId(file) {
        return Security.uuidv4() + file.name;
    }

    private search(search: string = "") {
        let searchFilter : FileFilter = new FileFilter();
        searchFilter.name = search;

        this._filter.addFilter(searchFilter);
        this.refreshGUI();
    }


    public refresh() {
        this.loadServerFiles(() => {
            this.refreshGUI()
        });
    }

    private refreshGUI() {
        let files: FileContainer[] = this.filterFiles(this._filter, this._files);
        this.showFiles(files);
    }

    private filterFiles(fileFilter: FileFilter, files : FileContainer[]) : FileContainer[] {
        let filteredFiles : FileContainer[]= files.filter(fc => {
            return fileFilter.checkFilter(fc);
        });
        return filteredFiles;
    }

    private showFiles(files : FileContainer[]) {
        const fileListView : HTMLElement = document.getElementById(this.ID_fileExplorerFiles);
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

    private createFileViewCard(file : FileContainer) : HTMLElement {
        let newRow : HTMLElement = document.createElement('li');
        newRow.dataset.fileId = file.fileId;
        newRow.className = 'justify-content-between list-group-item d-flex justify-content-between';

        let colActions: HTMLDivElement = document.createElement('div');
        let colName: HTMLDivElement= document.createElement('div');

        if (this.ShowPreviewImage === true) {
            let previewImage: HTMLImageElement = this.createPreviewImage(file);
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

    private createCopyURLButton(file : FileContainer): HTMLButtonElement {
        const buttonCopyUrl = GuiCreation.createImageButton("far fa-copy", "btn btn-outline-info m-1", "Copy to Clipboard")
        buttonCopyUrl.onclick = function () {
            ClientSideFunctions.copyToClipboard(file.URL);
        };
        return buttonCopyUrl;
    }

    private createPreviewImage(file: FileContainer): HTMLImageElement {
        let previewImage : HTMLImageElement = document.createElement('img');
        previewImage.className = this.CSS_PreviewImage;
        if (this.CreatePreviewImageURL != null) {
            previewImage.src = this.CreatePreviewImageURL(file);
        } else {
            previewImage.src = "#"
            console.error('Can not create PreviewImageURL: CreatePreviewImageURL == null')
        }

        return previewImage;
    }

    private createName(file: FileContainer): HTMLElement {
        let col : HTMLElement;
        if (file.URL != null && file.URL !== "") {
            //file name as link if URL is provided
            let colAnchor: HTMLAnchorElement = document.createElement('a');
            colAnchor.href = file.URL;
            col = colAnchor;
        } else {
            //only file name if no URL
            col = document.createElement('span');
        }
        col.textContent = file.file.name;

        return col;
    }

    private createRemoveButton(file : FileContainer) : HTMLButtonElement {
        let self : FileExplorer = this;
        let removeButton : HTMLButtonElement = GuiCreation.createImageButton("fas fa-trash", "btn btn-outline-danger m-1", "Remove")
        removeButton.onclick = function () {
            if (file.fileId !== "") {
                self.deleteFileOnServer(file, (f) => self.deleteFileLocal(f));
            } else {
                self.deleteFileLocal(file);
            }
        }
        return removeButton;
    }

    private deleteFileLocal(file : FileContainer) : void {
        this._files = this._files.filter(f => f.fileId !== file.fileId);
        this.refreshGUI();
    }

    private async deleteFileOnServer(file : FileContainer, onFileDelete) {
        if (file == null || this.DeleteFileURL == null) {
            return;
        }

        let formData = new FormData();
        formData.append("id", file.fileId)
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
        } catch (error) {
            console.error('Error', error);
        }
    }


    private createUploadButton(file : FileContainer) : HTMLElement {
        const self : FileExplorer = this;
        let uploadButton = GuiCreation.createImageButton("fas fa-upload", "btn btn-outline-info m-1", "Upload")
        uploadButton.onclick = function () {
            const newImage = GuiCreation.replaceButtonWithImage(this, "fa fa-spinner fa-spin m-3")
            file.uploadState = UploadStates.OnUpload;

            if (self.UploadFileURL != null) {
                let formData = self.createUploadFormData([file]);
                ServerSideFunctions.uploadFormData(formData, self.UploadFileURL, function (success) {
                    if (success) {
                        GuiCreation.changeImage(newImage, "fa fa-check m-3");

                        //ChangeUpload State
                        file.uploadState = UploadStates.Uploaded;
                    } else {
                        file.uploadState = UploadStates.Added;
                        GuiCreation.changeImage(newImage, "fa fa-exclamation-triangle m-3");
                    }

                    //switchout check with copy url Button
                    setTimeout(
                        function () {
                            self.refresh()
                        }, 1000);
                });
            } else {
                console.error("No Upload URL defined");
            }
        }
        return uploadButton;
    }

    private createUploadFormData(files : FileContainer[]): FormData{
        let formData: FormData = new FormData();
        
        for (var i = 0; i < files.length; i++) {
            //needed to supply multiple Objects from same type to asp.net controller
            let fileCont: FileContainer = files[i];
            
            formData.append("Files[" + i + "].FormFile", (fileCont.file as unknown) as Blob);

            let guiid: string = fileCont.guiId;
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

    private async loadServerFiles(filesLoadedCallback? : Function) {
        const self : FileExplorer = this;
        if (this.ImageUrl != null) {
            try {
                const response : Response = await fetch(this.ImageUrl, {
                    method: 'Post',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    //Only Show Images
                    body: JSON.stringify(self._filter)
                })

                if (response.ok) {
                    let rawJson = response.json();
                    rawJson.then(function (json) {
                        const serverFiles = json;
                        const transformedFiles : FileContainer[] = serverFiles.map(function (serverFileModel) : FileContainer {
                            let container: FileContainer = {
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
                    })
                }
            } catch (error) {
                console.error('Error', error);
            }
        }
    }
}