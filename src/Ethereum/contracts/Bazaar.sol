pragma solidity ^0.4.23;

import "zeppelin-solidity/contracts/ownership/Ownable.sol";
import "zeppelin-solidity/contracts/math/SafeMath.sol";

import "./BazaarLib.sol";

contract Bazaar is Ownable {
    using SafeMath for uint256;
    using BazaarLib for BazaarLib.Item;

    mapping(bytes32 => uint256) private availability;
    mapping(bytes32 => address) private sellerOf;
    mapping(address => Item[]) private itemsOf;

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
    modifier notEmpty(string _value, string _name) {
        require(!isEmpty(_value), string(abi.encodePacked(_name, " cannot be empty.")));
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
     * @dev Adds an item to seller's items.
     * @param _name The item name.
     * @param _description The item description.
     * @param _price The item price (per piece).
     * @param _quantity Quantity (pieces).
     */
    function addItem(
        string _name,
        string _description,
        uint256 _price,
        uint256 _quantity
    )
        public
        ownerNotAllowed
        notEmpty(_name, "Name")
        positive(_price, "Price")
        positive(_quantity, "Quantity")
    {
        bytes32 itemId = keccak256(msg.sender, _name, _description, _price);
        if (availability[itemId] > 0) {
            uint256 index = getItemIndex(msg.sender, itemId);
            itemsOf[msg.sender][index].quantity = itemsOf[msg.sender][index].quantity.add(_quantity);
        } else {
            Item memory item = Item({
                id: itemId,
                name: _name,
                description: _description,
                price: _price,
                quantity: _quantity
            });
            sellerOf[itemId] = msg.sender;
            itemsOf[msg.sender].push(item);
        }
        availability[itemId] = availability[itemId].add(_quantity);
        emit ItemAdd(_name, _price, _quantity, msg.sender);
    }

    /**
     * @dev Returns all items of the seller. Intended to be invoked by the seller.
     */
    function getItems() public view returns(Item[] items){
        return itemsOf[msg.sender];
    }

    /**
     * @dev Returns all items of the seller specified by \p _seller.
     * @param _seller The seller address.
     */
    function getItems(address _seller) public view returns(Item[] items){
        return itemsOf[_seller];
    }

    /**
     * @dev Buys \p _quantity pieces of the item.
     * @param _itemId The item id (hash).
     * @param _quantity Pieces to buy.
     */
    function buyItem(bytes32 _itemId, uint256 _quantity)
        public
        ownerNotAllowed
        available(_itemId)
        inStock(_itemId, _quantity)
        payable
    {
        address memory seller = sellerOf[_itemId];
        if (seller == msg.sender) {
            revert("Sellers are not allowed to buy their own items.");
        }

        uint256 memory index = getItemIndex(seller, _itemId);
        if (msg.value != itemsOf[seller][index].price * _quantity) {
            revert("Amount provided does not match the total cost.");
        }

        availability[_itemId] = availability[_itemId].subtract(_quantity);
        itemsOf[seller][index].quantity = itemsOf[seller][index].quantity.subtract(_quantity);

        seller.transfer(msg.value);
        emit ItemBuy(itemsOf[seller][index].name, _quantity, msg.value, msg.sender, seller);
    }

    function getItemIndex(address _addr, bytes32 _itemId) private view returns(uint256) {
        uint256 i = 0;
        while (itemsOf[_addr][i].Id != _itemId) {
            i++;
        }
        return i;
    }

    function isEmpty(string _value) private pure returns (bool) {
        bytes memory asByteArray = bytes(_value);
        return asByteArray.length == 0;
    }
}