export class FileDrop {

    readonly #descriptionWidth: number;
    readonly #dragHoverEventListenerBinding = this.#onDragHover.bind(this);
    readonly #dragLeaveEventListenerBinding = this.#onDragLeave.bind(this);
    readonly #dropEventListenerBinding = this.#onDrop.bind(this);
    readonly #changeEventListenerBinding = this.#onChange.bind(this);

    constructor(readonly dropZoneElement: HTMLElement,
        readonly inputLabel: HTMLElement,
        readonly descriptionLabel: HTMLElement,
        readonly noFileUploaded: string,
        readonly inputFile: HTMLInputElement) {
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

        this.inputFile.files = event.dataTransfer!.files;
        this.inputFile.dispatchEvent(new Event('change', { bubbles: true }));
    }

    #onChange() {
        this.descriptionLabel.style.width = this.#descriptionWidth + 'px';

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
