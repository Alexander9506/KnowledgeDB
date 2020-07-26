var UploadStates;
(function (UploadStates) {
    UploadStates["UploadState_Added"] = "added";
    UploadStates["UploadState_OnUpload"] = "onUpload";
    UploadStates["UploadState_Uploaded"] = "uploaded";
})(UploadStates || (UploadStates = {}));
class FileExplorer {
    constructor(fileExplorerId) {
        this.ID_fileExplorerSearch = 'file-explorer-search';
        this.ID_fileExplorerAddFile = 'file-explorer-add-file';
        this.ID_fileExplorerSettings = 'file-explorer-settings';
        this.ID_fileExplorerCbxOnlyNewFiles = 'file-explorer-cbox-only-new-files';
        this.ID_fileExplorerCbxPreviewImages = 'file-explorer-cbox-preview-images';
        this.ID_fileExplorerFiles = 'file-explorer-files';
        this.ShowPreviewImage = false;
        this.CreatePreviewImageURL = null;
        this.ImageUrl = null;
        this.Filter = null;
        //Css
        this.CSS_PreviewImage = "file-explorer-previewimage";
        this.Filter = {
            FileType: "image"
        };
        this.addListener();
    }
    addListener() {
        const fileAddButton = document.getElementById(this.ID_fileExplorerAddFile);
        const self = this;
        //New file added listener
        fileAddButton.onchange = function (e) {
            const target = event.target;
            let newFiles = [];
            newFiles.push.apply(newFiles, target.files);
            self.addFiles(newFiles);
            //remove file from Input to ensure it can be uploaded again
            target.value = null;
        };
        const searchInput = document.getElementById(this.ID_fileExplorerSearch);
        if (searchInput != null) {
            searchInput.addEventListener("input", function (e) {
                self.refresh(searchInput.value);
            });
        }
        const previewImageCbx = document.getElementById(this.ID_fileExplorerCbxPreviewImages);
        if (previewImageCbx != null) {
            previewImageCbx.addEventListener("change", (e) => {
                const target = e.target;
                self.ShowPreviewImage = target.checked;
                self.refresh();
            });
        }
    }
    addFiles(files) {
    }
    refresh(filter = "") {
    }
}
//# sourceMappingURL=FileExplorer.js.map