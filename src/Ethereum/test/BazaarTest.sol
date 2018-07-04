pragma solidity ^0.4.23;

import "truffle/Assert.sol";
import "truffle/DeployedAddresses.sol";
import "../contracts/BazaarLib.sol";
import "../contracts/Bazaar.sol";

contract BazaarTest{
    Bazaar bazaar;
    bytes32 itemId = keccak256(abi.encodePacked(block.number));

    function beforeEach() public {
        bazaar = new Bazaar(5);
    }

    function testSettingCommissionDuringCreation() public {
        Assert.equal(bazaar.commission(), uint256(5), "Commission is different than supplied value");
    }

    function testSettingAnOwnerDuringCreation() public {
        Assert.equal(bazaar.owner(), this, "An owner is different than a deployer");
    }

    function testSettingAnOwnerOfDeployedContract() public {
        bazaar = Bazaar(DeployedAddresses.Bazaar());
        Assert.equal(bazaar.owner(), msg.sender, "An owner is different than a deployer");
    }

    function testAddItemByNotAnOwner() public {
        bazaar = Bazaar(DeployedAddresses.Bazaar());

        bool success = callAddItem(itemId, 20 finney, 2);
        Assert.equal(success, true, "Calling addItem() by not an owner fails");
    }

    function testAddItemByNotAnOwnerWithZeroPrice() public {
        bazaar = Bazaar(DeployedAddresses.Bazaar());

        bool success = callAddItem(itemId, 0 finney, 2);
        Assert.equal(success, false, "Calling addItem() by not an owner with 0 price succeeds");
    }

    function testAddItemByNotAnOwnerWithZeroQuantity() public {
        bazaar = Bazaar(DeployedAddresses.Bazaar());

        bool success = callAddItem(itemId, 20 finney, 0);
        Assert.equal(success, false, "Calling addItem() by not an owner with 0 quantity succeeds");
    }

    function testAddItemByAnOwner() public {
        bool success = callAddItem(itemId, 20 finney, 2);
        Assert.equal(success, false, "Calling addItem() by an owner succeeds");
    }

    function callAddItem(
        bytes32 _itemId,
        uint256 _price,
        uint256 _quantity
    )
        private
        returns (bool success)
    {
        return bazaar.call(
            bytes4(keccak256("addItem(bytes32,uint256,uint256)")),
            _itemId,
            _price,
            _quantity
        );
    }
}