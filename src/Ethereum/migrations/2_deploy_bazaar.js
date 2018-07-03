const Bazaar = artifacts.require("./Bazaar.sol");
const BazaarLib = artifacts.require("./BazaarLib.sol");

module.exports = function(deployer) {
    deployer.deploy(BazaarLib);
    deployer.link(BazaarLib, Bazaar);
    deployer.deploy(Bazaar);
};