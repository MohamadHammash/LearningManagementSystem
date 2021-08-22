function getAntiForgeryToken() {
    token = $('input[name=__RequestVerificationToken]').val();
    return token;
};

async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': getAntiForgeryToken({})
            },
            body: formData
        });

        oFormElement.elements.namedItem("result").value =
            'Result: ' + response.status + ' ' + response.statusText;
    } catch (error) {
        console.error('Error:', error);
    }
}