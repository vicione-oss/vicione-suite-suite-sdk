import '/_content/ViciOne.Suite.Sdk.Client/js/event-target-mixins.js';

export class FileDropZone {
    #inputFile: HTMLInputElement | undefined = undefined;

    readonly #dragStartEventListenerBinding: ((event: DragEvent) => void) | undefined = undefined;
    readonly #dragEnterEventListenerBinding: ((event: DragEvent) => void) | undefined = undefined;
    readonly #dragOverEventListenerBinding: ((event: DragEvent) => void) | undefined = undefined;
    readonly #dragLeaveEventListenerBinding: ((event: DragEvent) => void) | undefined = undefined;
    readonly #dropEventListenerBinding: ((event: DragEvent) => void) | undefined = undefined;
    constructor(readonly dropZone: HTMLElement, readonly dotNetObject: DotNet.DotNetObject, inputFile: HTMLInputElement | undefined) {
        this.#inputFile = inputFile;

        this.#dragStartEventListenerBinding = this.#dragStart.bind(this);
        this.dropZone.addEventListener('dragstart', this.#dragStartEventListenerBinding);

        this.#dragEnterEventListenerBinding = (event: DragEvent) => {
            void this.#dragEnter(event);
        };

        this.dropZone.addEventListener('dragenter', this.#dragEnterEventListenerBinding);

        this.#dragOverEventListenerBinding = this.#dragOver.bind(this);
        this.dropZone.addEventListener('dragover', this.#dragOverEventListenerBinding);

        this.#dragLeaveEventListenerBinding = (event: DragEvent) => {
            void this.#dragLeave(event);
        };

        this.dropZone.addEventListener('dragleave', this.#dragLeaveEventListenerBinding);

        this.#dropEventListenerBinding = (event: DragEvent) => {
            void this.#drop(event);
        };

        this.dropZone.addEventListener('drop', this.#dropEventListenerBinding);
    }

    #dragStart(event: DragEvent) {
        // The event "dragstart" is not fired for file drags from the OS, see https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API/File_drag_and_drop.
        // Hence we simply cancel the event and therefore disallow a drop of the currently dragged item(s)
        event.preventDefault();
    }

    async #dragEnter(event: DragEvent) {
        await this.#handleDragEnter(event);
    }

    #dragOver(event: DragEvent) {
        // https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API/File_drag_and_drop#prevent_the_browsers_default_drag_behavior
        event.preventDefault();
    }

    #isSingleFileDrag(dataTransfer: DataTransfer | undefined) {
        if (dataTransfer?.items.length === 1) {
            const dataTransferItem = dataTransfer.items[0];

            return dataTransferItem?.kind === 'file';
        }

        return false;
    }

    async #handleDragEnter(event: DragEvent) {
        if (event.currentTarget !== this.dropZone)
            return;

        if (!event.dataTransfer)
            return;

        const isSingleFileDrag = this.#isSingleFileDrag(event.dataTransfer);

        if (isSingleFileDrag) {
            // https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API/File_drag_and_drop#prevent_the_browsers_default_drag_behavior
            event.preventDefault();

            event.dataTransfer.dropEffect = 'copy';

            await this.dotNetObject.invokeMethodAsync('DragEnter');

        } else {
            event.dataTransfer.dropEffect = 'none';
        }
    }

    async #dragLeave(event: DragEvent) {
        const from = event.target;
        const to = event.relatedTarget;
        const { currentTarget } = event;

        if (from?.isNestedHtmlElementOf(this.dropZone)) {
            if (to === this.dropZone)
                return;

            if (to && !to.isNestedHtmlElementOf(this.dropZone)) {
                await this.#executeDragLeave();

                return;
            }
        }

        if (currentTarget !== this.dropZone)
            return;

        if (to?.isNestedHtmlElementOf(this.dropZone))
            return;

        if (from?.isNestedHtmlElementOf(this.dropZone))
            return;

        await this.#executeDragLeave();
    }

    async #executeDragLeave() {
        await this.dotNetObject.invokeMethodAsync('DragLeave');
    }

    async #drop(event: DragEvent) {
        event.preventDefault();

        if (this.#inputFile) {
            this.#inputFile.files = event.dataTransfer!.files;
            this.#inputFile.dispatchEvent(new Event('change', { bubbles: true }));
        }

        await this.dotNetObject.invokeMethodAsync('Drop');
    }

    public setInputFile(inputFile: HTMLInputElement | undefined) {
        this.#inputFile = inputFile;
    }

    public dispose() {
        if (this.#dropEventListenerBinding)
            this.dropZone.removeEventListener('drop', this.#dropEventListenerBinding);

        if (this.#dragStartEventListenerBinding)
            this.dropZone.removeEventListener('dragstart', this.#dragStartEventListenerBinding);

        if (this.#dragLeaveEventListenerBinding)
            this.dropZone.removeEventListener('dragleave', this.#dragLeaveEventListenerBinding);

        if (this.#dragOverEventListenerBinding)
            this.dropZone.removeEventListener('dragover', this.#dragOverEventListenerBinding);

        if (this.#dragEnterEventListenerBinding)
            this.dropZone.removeEventListener('dragenter', this.#dragEnterEventListenerBinding);
    }
}

