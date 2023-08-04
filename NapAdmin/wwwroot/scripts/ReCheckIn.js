/* global NAP: false */

"use strict"; function ReCheckUM() {
    var urlRecheckIn = "/Auth/WhoAmI";
    var xmlhttp = new XMLHttpRequest();

    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4

            if (xmlhttp.status === 200) {
                document.location.href = window.rootUrl;
            }

            setTimeout(ReCheckUM, 60000);
        }
    };

    xmlhttp.open("POST", urlRecheckIn, true);
    xmlhttp.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    xmlhttp.send();
}
ReCheckUM();