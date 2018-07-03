pragma solidity ^0.4.23;

library BazaarLib {
    struct Item {
        bytes32 id;
        bytes32 name;
        bytes32 description;
        uint256 price;
        uint256 quantity;
    }

    event ItemAdded(
        bytes32 name,
        uint256 price,
        uint256 quantity,
        address seller
    );

    event ItemSold(
        bytes32 name,
        uint256 quantity,
        uint256 total,
        address buyer,
        address seller
    );

    function emitItemAdded(
        bytes32 _name,
        uint256 _price,
        uint256 _quantity,
        address _seller
    ) public
    {
        emit ItemAdded(_name, _price, _quantity, _seller);
    }

    function emitItemSold(
        bytes32 _name,
        uint256 _quantity,
        uint256 _total,
        address _buyer,
        address _seller
    ) public
    {
        emit ItemSold(_name, _quantity, _total, _buyer, _seller);
    }
}