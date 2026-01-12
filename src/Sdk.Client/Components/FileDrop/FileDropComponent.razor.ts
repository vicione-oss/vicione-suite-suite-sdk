class FileDrop {

    private readonly _descriptionWidth: number;

    constructor(readonly dropZoneElement: HTMLElement,
        readonly inputLabel: HTMLElement,
        readonly descriptionLabel: HTMLElement,
        readonly noFileUploaded: string,
        readonly inputFile: HTMLInputElement) {
        this._descriptionWidth = descriptionLabel.clientWidth;
    }

    public init() {
        this.dropZoneElement.addEventListener('dragenter', this.onDragHover.bind(this));
        this.dropZoneElement.addEventListener('dragover', this.onDragHover.bind(this));
        this.dropZoneElement.addEventListener('dragleave', this.onDragLeave.bind(this));
        this.dropZoneElement.addEventListener('drop', this.onDrop.bind(this));

        this.inputFile.addEventListener('change', this.onChange.bind(this));
        this.inputFile.addEventListener('input', this.onChange.bind(this));
        this.inputFile.addEventListener('cancel', this.onChange.bind(this));
    }

    public dispose() {
        this.dropZoneElement.removeEventListener('dragenter', this.onDragHover);
        this.dropZoneElement.removeEventListener('dragover', this.onDragHover);
        this.dropZoneElement.removeEventListener('dragleave', this.onDragLeave);
        this.dropZoneElement.removeEventListener('drop', this.onDrop);

        this.inputFile.removeEventListener('change', this.onChange.bind(this));
        this.inputFile.removeEventListener('input', this.onChange.bind(this));
        this.inputFile.removeEventListener('cancel', this.onChange.bind(this));
    }

    private onDragHover(e: DragEvent) {
        e.preventDefault();
        this.dropZoneElement.classList.add('hover');
    }

    private onDragLeave(e: DragEvent) {
        e.preventDefault();
        this.dropZoneElement.classList.remove('hover');
    }

    private onDrop(e: DragEvent) {
        e.preventDefault();
        this.dropZoneElement.classList.remove('hover');

        this.inputFile.files = e.dataTransfer!.files;
        this.inputFile.dispatchEvent(new Event('change', { bubbles: true }));
    }

    private onChange() {
        this.descriptionLabel.style.width = this._descriptionWidth + 'px';

        if (this.inputFile.files === null || this.inputFile.files.length === 0) {
            this.descriptionLabel.title = this.noFileUploaded;
            this.descriptionLabel.textContent = this.noFileUploaded;
            this.inputLabel.title = this.noFileUploaded;
            return;
        }

        this.descriptionLabel.title = this.inputFile.files[0].name;
        this.descriptionLabel.textContent = this.inputFile.files[0].name;
        this.inputLabel.title = this.inputFile.files[0].name;
    }
}

export function init(dropZoneElement: HTMLElement, inputLabel: HTMLElement, descriptionLabel: HTMLElement, noFileUploaded: string, inputFile: HTMLInputElement) {
    const fileDrop = new FileDrop(dropZoneElement, inputLabel, descriptionLabel, noFileUploaded, inputFile);
    fileDrop.init();

    return fileDrop;
}
