export class NavTileStandardContent {
    #headlineResizeObserver: ResizeObserver | undefined;
    #isDisposed = false;

    constructor(readonly sublineElement: HTMLElement, readonly headlineElement: HTMLElement,
        readonly contentElement: HTMLElement, readonly iconElement: HTMLElement,
        readonly dotNetHelper: DotNet.DotNetObject) {}

    public async initSubline() {
        if (this.#isDisposed)
            return;

        if (!(this.headlineElement instanceof Element)) {
            try {
                await this.dotNetHelper.invokeMethodAsync('SublineInitialized');
            } catch (e) {
                console.error('Could not invoke SublineInitialized, component likely disposed:', e);
            }

            return;
        }

        this.#headlineResizeObserver = new ResizeObserver(this.#observerCallback.bind(this));
        this.#headlineResizeObserver.observe(this.headlineElement);
    }

    public setSublineVerticalOffsetToIcon(sublineElement: HTMLElement, iconElement: HTMLElement) {
        const sublineY = sublineElement.offsetTop;
        const iconY = iconElement.offsetTop;

        sublineElement.style.setProperty('--vertical-offset-to-icon', `${iconY - sublineY}px`);
    }

    public dispose() {
        this.#isDisposed = true;

        if (this.#headlineResizeObserver !== undefined) {
            this.#headlineResizeObserver.disconnect();
            this.#headlineResizeObserver = undefined;
        }
    }

    async #observerCallback() {
        if (this.#isDisposed) {
            await this.dotNetHelper.invokeMethodAsync('SublineInitialized');
            return;
        }

        this.#setSublineMaximumLineCount(this.sublineElement, this.headlineElement, this.contentElement);
        this.setSublineVerticalOffsetToIcon(this.sublineElement, this.iconElement);

        try {
            await this.dotNetHelper.invokeMethodAsync('SublineInitialized');
        } catch (e) {
            console.error('Could not invoke SublineInitialized in observerCallback:', e);
        }
    }

    #setSublineMaximumLineCount(sublineElement: HTMLElement, headlineElement: HTMLElement, contentElement: HTMLElement) {
        const singleLineHeadlineOffsetHeight = this.#getSingleLineHeadlineOffsetHeight(headlineElement, contentElement);
        const currentHeadlineOffsetHeight = headlineElement.offsetHeight;

        if (currentHeadlineOffsetHeight > singleLineHeadlineOffsetHeight)
            sublineElement.style.removeProperty('--maximum-line-count');
        else
            sublineElement.style.setProperty('--maximum-line-count', '5');
    }

    #getSingleLineHeadlineOffsetHeight(headlineElement: HTMLElement, contentElement: HTMLElement) {
        const headlineElementClone = headlineElement.cloneNode(true);

        if (!(headlineElementClone instanceof HTMLElement))
            throw new Error('Expected cloned node to be an HTMLElement');

        headlineElementClone.innerText = 'Ag';
        headlineElementClone.style.setProperty('position', 'absolute'); // Absolute positioning to avoid realignment of grid elements which would trigger the resize observer

        contentElement.insertBefore(headlineElementClone, headlineElement.nextSibling);

        const result = headlineElementClone.offsetHeight;

        headlineElementClone.remove();

        return result;
    }
}
