pragma solidity ^0.4.23;

library BazaarLib {
    struct Item {
        uint256 price;
        address seller;
    }

    event NewItem(
        bytes32 itemId,
        uint256 price,
        uint32 quantity,
        address seller
    );

    event Purchase(
        bytes32 itemId,
        uint256 price,
        uint32 quantity,
        uint256 total,
        address customer,
        address seller
    );

    function emitNewItem(
        bytes32 _itemId,
        uint256 _price,
        uint32 _quantity,
        address _seller
    ) public
    {
        emit NewItem(_itemId, _price, _quantity, _seller);
    }

    function emitPurchase(
        bytes32 _itemId,
        uint256 _price,
        uint32 _quantity,
        uint256 _total,
        address _customer,
        address _seller
    ) public
    {
        emit Purchase(_itemId, _price, _quantity, _total, _customer, _seller);
    }
}