let userForm = document.querySelector('.clear-search-form');
let userClearAnchor = document.querySelector('.clear-anchor');

userClearAnchor.addEventListener('click', function (e) {
    let userForm = document.querySelector('.clear-search-form');
    userForm.submit();
})

userForm.addEventListener('submit', function (e) {
    let userSearchForm = document.querySelector('.search-form');
    userSearchForm.reset();
})

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

        //oFormElement.elements.namedItem("result").value =
        //    'Result: ' + response.status + ' ' + response.statusText;
        location.reload();

    } catch (error) {
        console.error('Error:', error);
    }
}