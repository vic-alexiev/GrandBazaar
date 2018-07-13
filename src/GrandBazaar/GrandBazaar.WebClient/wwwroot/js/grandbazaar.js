var grandBazaar = (function () {
    var showTxInfo = function (url) {
        $('#infoBox>p').html(`Transaction hash: <a href="${url}" target="_blank">${url}</a>`);
        $('#infoBox').show();
        $('#infoBox>header').click(function () { $('#infoBox').hide(); });
    }

    var showInfo = function (message) {
        $('#infoBox>p').html(message);
        $('#infoBox').show();
        $('#infoBox>header').click(function () { $('#infoBox').hide(); });
    }

    var showError = function (message) {
        $('#errorBox>p').html("Error: " + message);
        $('#errorBox').show();
        $('#errorBox>header').click(function () { $('#errorBox').hide(); });
    }

    // public API
    return {
        showTxInfo: showTxInfo,
        showInfo: showInfo,
        showError: showError
    };
})();
