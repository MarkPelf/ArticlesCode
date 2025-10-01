//util1.js

/**
* sends a request to the specified url from a form. this will change the window location.
* @param {string} path the path to send the post request to
* @param {object} params the parameters to add to the url
* @param {string} [method=post] the method to use on the form
*
* Warning: This solution is limited and does not handle arrays or nested objects inside of a form.
*/

function postToUrl1(path, params, method = 'post') {

    // The rest of this code assumes you are not using a library.
    // It can be made less verbose if you use one.
    const form = document.createElement('form');
    form.method = method;
    form.action = path;

    for (const key in params) {
        if (params.hasOwnProperty(key)) {
            const hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = key;
            hiddenField.value = params[key];

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}

/**
* sends a request to the specified url from a form. this will change the window location.
* @param {string} path the path to send the post request to
* @param {object} params the parameters to add to the url
* @param {string} [method=post] the method to use on the form
*
* with support for arrays
*/

function postToUrl12(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    var addField = function (key, value) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", key);
        hiddenField.setAttribute("value", value);

        form.appendChild(hiddenField);
    };

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            if (params[key] instanceof Array) {
                for (var i = 0; i < params[key].length; i++) {
                    addField(key, params[key][i])
                }
            }
            else {
                addField(key, params[key]);
            }
        }
    }

    document.body.appendChild(form);
    form.submit();
}

//function is used when we need to extract a list of a given property
//extract values from an array of objects based on a specific key
function PluckUtil1(array, key) {
    return array.map(function (obj) {
        return obj[key];
    });
}
