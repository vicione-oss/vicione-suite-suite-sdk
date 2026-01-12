class NavTileStandardContent {
    private headlineResizeObserver: ResizeObserver | undefined;

    constructor(readonly sublineElement: HTMLElement, readonly headlineElement: HTMLElement,
        readonly contentElement: HTMLElement, readonly iconElement: HTMLElement,
        readonly dotNetHelper: DotNet.DotNetObject) {
    }

    public initSubline() {
        this.headlineResizeObserver = new ResizeObserver(this.observerCallback.bind(this));

        this.headlineResizeObserver.observe(this.headlineElement);
    }

    public setSublineVerticalOffsetToIcon(sublineElement: HTMLElement, iconElement: HTMLElement) {
        const sublineY = sublineElement.offsetTop;
        const iconY = iconElement.offsetTop;

        sublineElement.style.setProperty('--vertical-offset-to-icon', `${iconY - sublineY}px`);
    }

    public dispose() {
        if (this.headlineResizeObserver !== undefined)
            this.headlineResizeObserver.disconnect();

    }

    private async observerCallback() {
        this.setSublineMaximumLineCount(this.sublineElement, this.headlineElement, this.contentElement);
        this.setSublineVerticalOffsetToIcon(this.sublineElement, this.iconElement);

        await this.dotNetHelper.invokeMethodAsync('SublineInitialized');
    }

    private setSublineMaximumLineCount(sublineElement: HTMLElement, headlineElement: HTMLElement, contentElement: HTMLElement) {
        const singleLineHeadlineOffsetHeight = this.getSingleLineHeadlineOffsetHeight(headlineElement, contentElement);
        const currentHeadlineOffsetHeight = headlineElement.offsetHeight;

        if (currentHeadlineOffsetHeight > singleLineHeadlineOffsetHeight)
            sublineElement.style.removeProperty('--maximum-line-count');
        else
            sublineElement.style.setProperty('--maximum-line-count', '5');
    }

    private getSingleLineHeadlineOffsetHeight(headlineElement: HTMLElement, contentElement: HTMLElement) {
        const headlineElementClone = headlineElement.cloneNode(true) as HTMLElement;
        headlineElementClone.innerText = 'Ag';
        headlineElementClone.style.setProperty('position', 'absolute'); // Absolute positioning to avoid realignment of grid elements which would trigger the resize observer

        contentElement.insertBefore(headlineElementClone, headlineElement.nextSibling);

        const result = headlineElementClone.offsetHeight;

        headlineElementClone.remove();

        return result;
    }
}

export function init(sublineElement: HTMLElement, headlineElement: HTMLElement, contentElement: HTMLElement, iconElement: HTMLElement, dotNetHelper: DotNet.DotNetObject) {
    const navTileStandardContent = new NavTileStandardContent(sublineElement, headlineElement, contentElement, iconElement, dotNetHelper);

    navTileStandardContent.initSubline();

    return navTileStandardContent;
}
