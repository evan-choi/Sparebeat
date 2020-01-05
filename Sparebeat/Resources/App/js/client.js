var Sparebeat = Sparebeat || {
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

        var iframe = document.getElementById('client').contentWindow;
        var message = {
            id: 'sparebeat',
            map: map,
            bgm: bgm
        };

        iframe.postMessage(JSON.stringify(message), 'https://sparebeat.com');
    }
};