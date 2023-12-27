let evalId = 0;

const Sparebeat = window.Sparebeat || {
    hello: function () {
        alert('hello sparebeat');
    },
    load: function (map, bgm) {
        if (typeof map === 'undefined') {
            return false;
        }

        if (typeof bgm === 'undefined') {
            return false;
        }

        const iframe = document.getElementById('client').contentWindow;
        const message = {
            id: 'sparebeat',
            map: map,
            bgm: bgm
        };

        iframe.postMessage(JSON.stringify(message), 'https://sparebeat.com');
    },
    eval: function (script) {
        const iframe = document.getElementById('client').contentWindow;
        const message = {
            key: 'eval',
            id: evalId++,
            script: script
        };

        const promise = new Promise(function (resolve, reject) {
            function responseHandler(event) {
                const payload = JSON.parse(event.data);

                if (payload.key === 'eval#ret' && payload.id === message.id) {
                    window.removeEventListener('message', responseHandler);

                    if (payload.error) {
                        const error = new Error(payload.error.message);
                        error.name = payload.error.name;
                        error.stack = payload.error.stack;

                        reject(error);
                    } else {
                        resolve(payload.result);
                    }
                }
            }

            window.addEventListener('message', responseHandler);
        });

        iframe.postMessage(JSON.stringify(message), 'https://sparebeat.com');

        return promise;
    },
};
