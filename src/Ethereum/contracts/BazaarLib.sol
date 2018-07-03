pragma solidity ^0.4.23;

library BazaarLib {
    struct Item {
        bytes32 id;
        bytes32 name;
        uint256 price;
        uint256 quantity;
        address seller;
    }

    event NewItem(
        bytes32 name,
        uint256 price,
        uint256 quantity,
        address seller
    );

    event Purchase(
        bytes32 name,
        uint256 quantity,
        uint256 total,
        address customer,
        address seller
    );

    function emitNewItem(
        bytes32 _name,
        uint256 _price,
        uint256 _quantity,
        address _seller
    ) public
    {
        emit NewItem(_name, _price, _quantity, _seller);
    }

    function emitPurchase(
        bytes32 _name,
        uint256 _quantity,
        uint256 _total,
        address _customer,
        address _seller
    ) public
    {
        emit Purchase(_name, _quantity, _total, _customer, _seller);
    }
}