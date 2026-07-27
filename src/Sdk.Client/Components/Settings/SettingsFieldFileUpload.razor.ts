export class SettingsFieldFileUpload {

    public resetInput(...inputFiles: Array<HTMLInputElement | undefined>) {
        for (const inputFile of inputFiles) {
            if (!inputFile)
                continue;

            inputFile.value = '';
        }
    }

    public showFilePicker(inputFile: HTMLInputElement) {
        inputFile.value = ''; // Clear selected file to ensure onchange is triggered at all times

        inputFile.showPicker();
    }
}
