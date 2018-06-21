// addEventListener support for IE8
function bindEvent(element, eventName, eventHandler) {
    if (element.addEventListener) {
        element.addEventListener(eventName, eventHandler, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + eventName, eventHandler);
    }
}

bindEvent(window, 'load', function(event) { 
    var sendError = document.getElementById('send-error');
    var typeError = document.getElementById('type-error');
    var messageBox = document.getElementById('message-box');
    var sendButton = document.getElementById('send-message');
    var frameNumber = window.frameElement.id.replace(/\D/g,'');

    document.getElementById('header').innerHTML = "Messenger #" + frameNumber;

    // Send a message to the parent window
    var sendMessage = function (message) {
        window.parent.postMessage(JSON.stringify(message), '*');
    };

    // Listen to messages from parent window
    bindEvent(window, 'message', function (e) {
        var item = document.createElement('li');
        var text = document.createElement('span');
        var title = document.createElement('strong');
        var message = JSON.parse(e.data);
        var sender = frameNumber === message.sender.replace(/\D/g,'')
            ? 'me'
            : message.sender;
        
        if (message.logged) {
            title.setAttribute('class', sender)
            text.innerHTML = message.text;
            title.innerHTML = '[' + sender + ']:';

            item.appendChild(title);
            item.appendChild(text);
            document.getElementById('messages').appendChild(item);
            messageBox.value = '';
        } else {
            sendError.style.display = 'list-item';
            setTimeout(function() {
                sendError.style.display = 'none';    
            }, 5000);
        }
    });

    // Send message from iframe
    bindEvent(sendButton, 'click', function (e) {
        var text = messageBox.value;

        if (text) {
            sendMessage({
                "sender": "iFrame" + frameNumber,
                text
            });
        } else {
            typeError.style.display = 'list-item';
        }
    });

    // Send message on click enter key
    bindEvent(messageBox, 'keyup', function(e) {
        e.preventDefault();

        if (e.keyCode === 13) {
            sendButton.click();
        }
    });

    // Show/hide error message on message input changes
    bindEvent(messageBox, 'input', function() {
        if (!this.value) {
            typeError.style.display = 'list-item';
        } else if (typeError.style.display !== 'none') {
            typeError.style.display = 'none';
        }
    });
});