window.feedUpload = {
    selectedFiles: [],
    dotNetRef: null,

    init: function (dotNetRef) {
        this.dotNetRef = dotNetRef;
    },

    filesSelected: function (input) {
        var newFiles = Array.from(input.files);
        for(let i=0; i<newFiles.length; i++){
            this.selectedFiles.push(newFiles[i]);
        }
        input.value = ''; // Reset input to allow selecting the same file again
        this.notifyBlazor();
    },

    removeFile: function (index) {
        if(index >= 0 && index < this.selectedFiles.length) {
            this.selectedFiles.splice(index, 1);
            this.notifyBlazor();
        }
    },

    notifyBlazor: function () {
        var fileInfos = this.selectedFiles.map(f => {
            var url = '';
            if (f.type.startsWith('image/') || f.type.startsWith('video/')) {
                url = URL.createObjectURL(f);
            }
            return { name: f.name, size: f.size, type: f.type, previewUrl: url };
        });
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnFilesSelected', fileInfos);
        }
    },

    clearFiles: function () {
        this.selectedFiles = [];
        var inputs = document.querySelectorAll('input[type="file"]');
        inputs.forEach(input => {
            input.value = '';
        });
    },

    uploadFiles: async function () {
        if (this.selectedFiles.length === 0) return [];
        
        var formData = new FormData();
        for (var i = 0; i < this.selectedFiles.length; i++) {
            formData.append('files', this.selectedFiles[i]);
        }

        try {
            var response = await fetch('/api/feed/upload', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                var errorText = await response.text();
                throw new Error(errorText || 'Upload failed');
            }

            var result = await response.json();
            return result;
        } catch (error) {
            throw new Error(error.message || 'Upload failed');
        }
    }
};
