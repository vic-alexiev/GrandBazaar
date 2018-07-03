pragma solidity ^0.4.23;

library BazaarLib {
    struct Item {
        bytes32 id;
        string name;
        string description;
        uint256 price;
        uint256 quantity;
    }

    event ItemAdd(
        string name,
        uint256 price,
        uint256 quantity,
        address seller
    );

    event ItemBuy(
        string name,
        uint256 quantity,
        uint256 total,
        address buyer,
        address seller
    );
}