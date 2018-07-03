const Bazaar = artifacts.require("./Bazaar.sol");

module.exports = function(deployer) {
    deployer.deploy(Bazaar);
};