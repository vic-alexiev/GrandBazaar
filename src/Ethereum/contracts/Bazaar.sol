pragma solidity ^0.4.23;

import "zeppelin-solidity/contracts/ownership/Ownable.sol";
import "zeppelin-solidity/contracts/math/SafeMath.sol";

import "./BazaarLib.sol";

contract Bazaar is Ownable {
    using SafeMath for uint256;
    using BazaarLib for BazaarLib.Item;

    bytes32[] private itemIds;
    mapping(bytes32 => uint256) private availability;
    mapping(bytes32 => BazaarLib.Item) private detailsOf;
    mapping(address => BazaarLib.Item[]) private itemsOf;

    /**
     * @dev Throws if called by the contract owner account.
     */
    modifier ownerNotAllowed() {
        require(msg.sender != owner, "Owner is not allowed to call this function.");
        _;
    }

    /**
     * @dev Throws if item's availability equals 0.
     * @param _itemId The item Id (hash).
     */
    modifier available(bytes32 _itemId) {
        require(availability[_itemId] > 0, "Item out of stock.");
        _;
    }

    /**
     * @dev Throws if item's availability is less than \p _count.
     * @param _itemId The item Id (hash).
     * @param _count The quantity to check against.
     */
    modifier inStock(bytes32 _itemId, uint256 _count) {
        require(availability[_itemId] >= _count, "Not enough stock.");
        _;
    }

    /**
     * @dev Throws if \p _value is empty.
     * @param _value The value to check.
     * @param _name Parameter name (used in the error message).
     */
    modifier notEmpty(bytes32 _value, string _name) {
        require(_value.length > 0, string(abi.encodePacked(_name, " cannot be empty.")));
        _;
    }

    /**
     * @dev Throws if \p _value is not positive.
     * @param _value The value to check.
     * @param _name Parameter name (used in the error message).
     */
    modifier positive(uint256 _value, string _name) {
        require(_value > 0, string(abi.encodePacked(_name, " must be greater than 0.")));
        _;
    }

    /**
     * @dev Adds an item for sale. If item already exists, only its quantity is increased.
     * @param _name The item name.
     * @param _description The item description.
     * @param _price The item price.
     * @param _quantity Quantity available.
     */
    function addItem(
        bytes32 _name,
        bytes32 _description,
        uint256 _price,
        uint256 _quantity
    )
        public
        ownerNotAllowed
        notEmpty(_name, "Name")
        positive(_price, "Price")
        positive(_quantity, "Quantity")
    {
        bytes32 itemId = keccak256(abi.encodePacked(msg.sender, _name, _description, _price));
        if (availability[itemId] > 0) {
            availability[itemId] = availability[itemId].add(_quantity);
        } else {
            BazaarLib.Item memory item;
            item.id = itemId;
            item.name = _name;
            item.price = _price;
            item.seller = msg.sender;

            itemIds.push(itemId);
            detailsOf[itemId] = item;
            itemsOf[msg.sender].push(item);
            availability[itemId] = _quantity;

            BazaarLib.emitNewItem(_name, _price, _quantity, msg.sender);
        }
    }

    /**
     * @dev Returns all items in the bazaar.
     */
    function getAllItems() public view
        returns (bytes32[], bytes32[], uint256[], uint256[])
    {
        return toTupleOfArraysAll();
    }

    /**
     * @dev Returns all items of the seller. Intended to be invoked by the seller.
     */
    function getItems() public view
        returns (bytes32[], bytes32[], uint256[], uint256[])
    {
        return toTupleOfArrays(msg.sender);
    }

    /**
     * @dev Returns all items of the seller specified by \p _seller.
     * @param _seller The seller address.
     */
    function getItems(address _seller) public view
        returns (bytes32[], bytes32[], uint256[], uint256[])
    {
        return toTupleOfArrays(_seller);
    }

    /**
     * @dev Used by customers to purchase certain \p _quantity of the item.
     * @param _itemId The item id (hash).
     * @param _quantity Purchased quantity.
     */
    function purchase(bytes32 _itemId, uint256 _quantity)
        public
        ownerNotAllowed
        available(_itemId)
        inStock(_itemId, _quantity)
        payable
    {
        BazaarLib.Item storage item = detailsOf[_itemId];
        if (item.seller == msg.sender) {
            revert("Sellers are not allowed to purchase their own items.");
        }

        if (msg.value != item.price * _quantity) {
            revert("Amount provided does not match the total cost.");
        }

        availability[_itemId] = availability[_itemId].sub(_quantity);
        item.seller.transfer(msg.value);
        BazaarLib.emitPurchase(item.name, _quantity, msg.value, msg.sender, item.seller);
    }

    function toTupleOfArrays(address _addr) private view
        returns (bytes32[], bytes32[], uint256[], uint256[])
    {
        uint256 itemsCount = itemsOf[_addr].length;
        bytes32[] memory ids = new bytes32[](itemsCount);
        bytes32[] memory names = new bytes32[](itemsCount);
        uint256[] memory prices = new uint256[](itemsCount);
        uint256[] memory quantities = new uint256[](itemsCount);

        for (uint256 i = 0; i < itemsCount; i++) {
            BazaarLib.Item storage item = itemsOf[_addr][i];
            ids[i] = item.id;
            names[i] = item.name;
            prices[i] = item.price;
            quantities[i] = availability[item.id];
        }

        return (ids, names, prices, quantities);
    }

    function toTupleOfArraysAll() private view
        returns (bytes32[], bytes32[], uint256[], uint256[])
    {
        uint256 itemsCount = itemIds.length;
        bytes32[] memory ids = new bytes32[](itemsCount);
        bytes32[] memory names = new bytes32[](itemsCount);
        uint256[] memory prices = new uint256[](itemsCount);
        uint256[] memory quantities = new uint256[](itemsCount);

        for (uint256 i = 0; i < itemsCount; i++) {
            BazaarLib.Item storage item = detailsOf[itemIds[i]];
            ids[i] = item.id;
            names[i] = item.name;
            prices[i] = item.price;
            quantities[i] = availability[item.id];
        }

        return (ids, names, prices, quantities);
    }
}