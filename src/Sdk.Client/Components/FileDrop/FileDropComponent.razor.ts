type FileDropTexts = {
    noFileSelected: string;
    onlyOneFileCanBeDropped: string;
    filesSelected: string;
};

export class FileDrop {

    readonly #descriptionWidth: number;
    readonly #dragHoverEventListenerBinding = this.#onDragHover.bind(this);
    readonly #dragLeaveEventListenerBinding = this.#onDragLeave.bind(this);
    readonly #dropEventListenerBinding = this.#onDrop.bind(this);
    readonly #changeEventListenerBinding = this.#onChange.bind(this);

    constructor(readonly dropZoneElement: HTMLElement,
        readonly inputLabel: HTMLElement,
        readonly descriptionLabel: HTMLElement,
        readonly inputFile: HTMLInputElement,
        readonly texts: FileDropTexts) {
        this.#descriptionWidth = descriptionLabel.clientWidth;

        this.dropZoneElement.addEventListener('dragenter', this.#dragHoverEventListenerBinding);
        this.dropZoneElement.addEventListener('dragover', this.#dragHoverEventListenerBinding);
        this.dropZoneElement.addEventListener('dragleave', this.#dragLeaveEventListenerBinding);
        this.dropZoneElement.addEventListener('drop', this.#dropEventListenerBinding);

        this.inputFile.addEventListener('change', this.#changeEventListenerBinding);
        this.inputFile.addEventListener('input', this.#changeEventListenerBinding);
        this.inputFile.addEventListener('cancel', this.#changeEventListenerBinding);
    }

    #onDragHover(event: DragEvent) {
        event.preventDefault();
        this.dropZoneElement.classList.add('hover');
    }

    #onDragLeave(event: DragEvent) {
        event.preventDefault();
        this.dropZoneElement.classList.remove('hover');
    }

    #onDrop(event: DragEvent) {
        event.preventDefault();
        this.dropZoneElement.classList.remove('hover');

        const files = event.dataTransfer?.files;

        // Dragged text or a link carries no files; the current selection stays.
        if (!files || files.length === 0)
            return;

        if (files.length > 1 && !this.inputFile.multiple) {
            this.#showDescription(this.texts.onlyOneFileCanBeDropped);
            return;
        }

        this.inputFile.files = files;
        this.inputFile.dispatchEvent(new Event('change', { bubbles: true }));
    }

    #onChange() {
        const { files } = this.inputFile;

        if (files === null || files.length === 0)
            this.#showSelection(this.texts.noFileSelected);
        else if (files.length === 1)
            this.#showSelection(files[0].name);
        else
            this.#showSelection(this.texts.filesSelected.replace('{0}', () => files.length.toString()));
    }

    #showSelection(text: string) {
        this.inputLabel.title = text;
        this.#showDescription(text);
    }

    #showDescription(text: string) {
        this.descriptionLabel.style.width = this.#descriptionWidth + 'px';
        this.descriptionLabel.title = text;
        this.descriptionLabel.textContent = text;
    }

    public clear() {
        // Setting the value raises no change event, so OnChange is not invoked again.
        this.inputFile.value = '';
        this.#onChange();
    }

    public dispose() {
        this.dropZoneElement.removeEventListener('dragenter', this.#dragHoverEventListenerBinding);
        this.dropZoneElement.removeEventListener('dragover', this.#dragHoverEventListenerBinding);
        this.dropZoneElement.removeEventListener('dragleave', this.#dragLeaveEventListenerBinding);
        this.dropZoneElement.removeEventListener('drop', this.#dropEventListenerBinding);

        this.inputFile.removeEventListener('change', this.#changeEventListenerBinding);
        this.inputFile.removeEventListener('input', this.#changeEventListenerBinding);
        this.inputFile.removeEventListener('cancel', this.#changeEventListenerBinding);
    }
}
