window.saveAsFile = function (fileName, base64String) {
    try {
        var link = document.createElement('a');
        link.download = fileName;
        link.href = "data:application/pdf;base64," + base64String;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (error) {
        console.error('Errore durante il download del file:', error);
    }
} 