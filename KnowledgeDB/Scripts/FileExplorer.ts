import { Security } from "./Security";

enum UploadStates {
    UploadState_Added = "added",
    UploadState_OnUpload = "onUpload",
    UploadState_Uploaded = "uploaded"
}


interface IFileFilter {
    FileType?: string;
    Name?: string;
}

class FileFilter implements IFileFilter{
    
    FileType: string;
    Name: string;

    public applyFilter(file: FileContainer) : boolean {
        //TODO:  Implement
        return false;
    }

    public overrideFilter(filter: IFileFilter) {
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
    constructor(public File: object,
        public FileId: string,
        public GuiId: string,
        public UploadState: UploadStates,
        public URL: string = "") {
    }
}

class FileExplorer {

    private ID_fileExplorer: string = 'file-explorer';
    private ID_fileExplorerSearch : string  = 'file-explorer-search' ;
    private ID_fileExplorerAddFile: string = 'file-explorer-add-file';
    private ID_fileExplorerSettings: string = 'file-explorer-settings';
    private ID_fileExplorerCbxOnlyNewFiles: string = 'file-explorer-cbox-only-new-files';
    private ID_fileExplorerCbxPreviewImages: string = 'file-explorer-cbox-preview-images';
    private ID_fileExplorerFiles: string = 'file-explorer-files';

    public ShowPreviewImage: Boolean = false;
    public CreatePreviewImageURL : Function = null;
    public ImageUrl = null;

    private _filter: FileFilter = null;

    //Css
    private CSS_PreviewImage: string = "file-explorer-previewimage";

    private _files: FileContainer[]

    constructor(fileExplorerId: string) {
        this.ID_fileExplorer = fileExplorerId;

        this._filter = new FileFilter();
        this._filter.FileType = "image";

        this.addListener();
    }

    private changeFilter(filter: IFileFilter) {
        this._filter.overrideFilter(filter);
    }

    private addListener() : void {
        const fileAddButton: HTMLInputElement = document.getElementById(this.ID_fileExplorerAddFile) as HTMLInputElement;
        const self : FileExplorer = this;

        //New file added listener
        fileAddButton.onchange = function (e : Event) {
            const target = event.target as HTMLInputElement;

            let newFiles: Array<FileContainer> = [];

            newFiles.push.apply(newFiles, target.files);
            self.addFiles(newFiles);

            //remove file from Input to ensure it can be uploaded again
            target.value = null;
        }

        const searchInput : HTMLInputElement = document.getElementById(this.ID_fileExplorerSearch) as HTMLInputElement;
        if (searchInput != null) {
            searchInput.addEventListener("input", function (e) {
                self.changeFilter({Name: searchInput.value})
                self.refresh();
            });
        }

        const previewImageCbx : HTMLInputElement = document.getElementById(this.ID_fileExplorerCbxPreviewImages) as HTMLInputElement;
        if (previewImageCbx != null) {
            previewImageCbx.addEventListener("change", (e) => {
                const target = e.target as HTMLInputElement;
                self.ShowPreviewImage = target.checked;
                self.refresh();
            });
        }
    }

    private addFiles(files: Array<FileContainer>) {
        const self : FileExplorer = this;
        const transformedFiles:FileContainer[] = files.map((f) => {
            const id = self.generateFileId(f);
            return new FileContainer(f,  id, id, UploadStates.UploadState_Added);
        });
        this._files.push.apply(this._files, transformedFiles);
        this.showFiles(this._files);
    }

    private generateFileId(file) {
        return Security.uuidv4() + file.name;
    }

    private search(search: string = "") {

    }

    private refresh() {
        let files: FileContainer[] = this.filterFiles(this._filter, this._files);
        this.showFiles(files);
    }

    private filterFiles(fileFilter: FileFilter, files : FileContainer[]) : FileContainer[] {
        return files.filter(fc => {
            fileFilter.applyFilter(fc);
        });
    }

    private showFiles(files : FileContainer[]) {

    }
}