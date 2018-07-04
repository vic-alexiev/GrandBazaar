const Bazaar = artifacts.require("./Bazaar.sol");
const BazaarLib = artifacts.require("./BazaarLib.sol");

const COMMISSION = 5; // percentage

module.exports = function(deployer) {
    deployer.deploy(BazaarLib);
    deployer.link(BazaarLib, Bazaar);
    deployer.deploy(Bazaar, COMMISSION);
};