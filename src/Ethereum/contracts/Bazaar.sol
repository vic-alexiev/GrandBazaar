pragma solidity ^0.4.23;

import "zeppelin-solidity/contracts/ownership/Ownable.sol";
import "zeppelin-solidity/contracts/math/SafeMath.sol";

import "./BazaarLib.sol";

contract Bazaar is Ownable {
    using SafeMath for uint256;
    using BazaarLib for BazaarLib.Item;

    mapping(bytes32 => uint256) private availability;
    mapping(bytes32 => address) private sellerOf;
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
     * @dev Adds an item to seller's items.
     * @param _name The item name.
     * @param _description The item description.
     * @param _price The item price (per piece).
     * @param _quantity Quantity (pieces).
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
            uint256 index = getItemIndex(msg.sender, itemId);
            BazaarLib.Item storage storedItem = itemsOf[msg.sender][index];
            storedItem.quantity = storedItem.quantity.add(_quantity);
        } else {
            BazaarLib.Item memory item = BazaarLib.Item({
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
        BazaarLib.emitItemAdded(_name, _price, _quantity, msg.sender);
    }

    /**
     * @dev Returns all items of the seller. Intended to be invoked by the seller.
     */
    function getItems() public view
        returns (bytes32[], bytes32[], bytes32[], uint256[], uint256[])
    {
        return toTupleOfArrays(msg.sender);
    }

    /**
     * @dev Returns all items of the seller specified by \p _seller.
     * @param _seller The seller address.
     */
    function getItems(address _seller) public view
        returns (bytes32[], bytes32[], bytes32[], uint256[], uint256[])
    {
        return toTupleOfArrays(_seller);
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
        address seller = sellerOf[_itemId];
        if (seller == msg.sender) {
            revert("Sellers are not allowed to buy their own items.");
        }

        uint256 index = getItemIndex(seller, _itemId);
        BazaarLib.Item storage item = itemsOf[seller][index];

        if (msg.value != item.price * _quantity) {
            revert("Amount provided does not match the total cost.");
        }

        availability[_itemId] = availability[_itemId].sub(_quantity);
        item.quantity = item.quantity.sub(_quantity);

        seller.transfer(msg.value);
        BazaarLib.emitItemSold(item.name, _quantity, msg.value, msg.sender, seller);
    }

    function getItemIndex(address _addr, bytes32 _itemId) private view returns(uint256) {
        uint256 i = 0;
        while (itemsOf[_addr][i].id != _itemId) {
            i++;
        }
        return i;
    }

    function toTupleOfArrays(address _addr) private view
        returns (bytes32[], bytes32[], bytes32[], uint256[], uint256[])
    {
        uint256 itemsCount = itemsOf[_addr].length;
        bytes32[] memory ids = new bytes32[](itemsCount);
        bytes32[] memory names = new bytes32[](itemsCount);
        bytes32[] memory descriptions = new bytes32[](itemsCount);
        uint256[] memory prices = new uint256[](itemsCount);
        uint256[] memory quantities = new uint256[](itemsCount);

        for (uint256 i = 0; i < itemsCount; i++) {
            BazaarLib.Item storage item = itemsOf[_addr][i];
            ids[i] = item.id;
            names[i] = item.name;
            descriptions[i] = item.description;
            prices[i] = item.price;
            quantities[i] = item.quantity;
        }
        return (ids, names, descriptions, prices, quantities);
    }
}