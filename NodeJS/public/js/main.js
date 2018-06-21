// addEventListener support for IE8
function bindEvent(element, eventName, eventHandler) {
    if (element.addEventListener) {
        element.addEventListener(eventName, eventHandler, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + eventName, eventHandler);
    }
}

bindEvent(window, 'load', function(event) { 

    var iframeId = 0;
    var button = document.getElementById('add-client');

    // add iframe to the parent window
    bindEvent(button, 'click', function(event) {
        var src = '/messenger.html'
        var iframe = document.createElement('iframe');

        iframe.setAttribute('src', src);
        iframe.setAttribute('id', 'frame-' + (++iframeId));
        iframe.style.width = 300 + 'px';
        iframe.style.height = 400 + 'px';

        sendMessage({ "sender": "system", "text": "iframe" + iframeId + " joined the conversation" });
        document.body.appendChild(iframe);
    });

    // Send a message to the all iframes if it successfully added to db
    var sendMessage = function(message) {
        var http = new XMLHttpRequest();
        var data = JSON.stringify(message);

        http.open('POST', "/api/log", true);
        http.setRequestHeader('Content-type', 'application/json');
        http.onreadystatechange = function() {
            if (http.readyState == XMLHttpRequest.DONE && http.status == 200) {
                message.logged = http.response === 'true';

                var sender = message.sender.replace(/\D/g,'');
                var frames = document.getElementsByTagName('iframe');

                for (var i = 0; i < frames.length; i++) {
                    if (message.logged || sender === frames[i].id.replace(/\D/g,'')) {
                        frames[i].contentWindow.postMessage(JSON.stringify(message), '*');

                        if (!message.logged) {
                            break;
                        }
                   }
                }
            }
        }
        http.send(data);
    };

    // Listen to message from iframes
    bindEvent(window, 'message', function (e) {
        sendMessage(JSON.parse(e.data));
    });

    // Show add button only after the document is loaded
    var plusIcon = document.createElement('span');
    var plusWrapper = document.createElement('div');

    plusIcon.classList.add('plus-icon');
    plusWrapper.appendChild(plusIcon);
    button.innerHTML = '';
    button.appendChild(plusWrapper);
});
