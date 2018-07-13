// Write your JavaScript code.
$(document).ready(function () {
    // Checking if Web3 has been injected by the browser (Mist/MetaMask)
    if (typeof web3 !== 'undefined') {
        // Use Mist/MetaMask's provider
        window.web3js = new Web3(web3.currentProvider);
    } else {
        grandBazaar.showError('No web3? You should consider trying MetaMask!');
        // fallback - use your fallback strategy (local node / hosted node + in-dapp id mgmt / fail)
        window.web3js = new Web3(new Web3.providers.HttpProvider("http://localhost:8545"));
    }

    // Now you can start your app & access web3 freely:
    extractCurrentAccount();

    function extractCurrentAccount() {
        window.web3js.eth.getAccounts(function (error, result) {
            if (result && result[0]) {
                console.log('Current Web3 account: ' + result[0]);
            } else {
                grandBazaar.showError('Please, log in MetaMask!');
            }
            setCookie('web3-account', result[0]);
        });
    }

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }
});
