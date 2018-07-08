pragma solidity ^0.4.23;

import "./BazaarLib.sol";

contract Bazaar {
    using BazaarLib for BazaarLib.Item;

    address public owner;
    uint8 public commission;
    mapping(bytes32 => uint32) public availability;
    mapping(bytes32 => BazaarLib.Item) public detailsOf;
    mapping(address => bytes32[]) public itemsOf;
    bytes32[] public items;

    constructor(uint8 _commission) public {
        owner = msg.sender;
        commission = _commission;
    }

    /**
     * @dev Throws if called by any account other than the owner.
     */
    modifier onlyOwner() {
        require(msg.sender == owner);
        _;
    }

    /**
     * @dev Throws if called by the contract owner account.
     */
    modifier ownerNotAllowed() {
        require(msg.sender != owner, "Owner is not allowed to call this function.");
        _;
    }

    /**
     * @dev Throws if item's availability doesn't equal 0.
     * @param _itemId The item Id (hash).
     */
    modifier isNew(bytes32 _itemId) {
        require(availability[_itemId] == 0, "Item has already been added.");
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
    modifier inStock(bytes32 _itemId, uint32 _count) {
        require(availability[_itemId] >= _count, "Not enough stock.");
        _;
    }

    /**
     * @dev Throws if \p _value is not positive.
     * @param _value The value to check.
     */
    modifier positive(uint256 _value) {
        require(_value > 0, "Value must be greater than 0.");
        _;
    }

    /**
     * @dev Adds an item for sale. If item already exists, only its quantity is increased.
     * @param _itemId The item Id (hash).
     * @param _price The item price.
     * @param _quantity Quantity available.
     */
    function addItem(
        bytes32 _itemId,
        uint256 _price,
        uint32 _quantity
    )
        public
        ownerNotAllowed
        isNew(_itemId)
        positive(_price)
        positive(_quantity)
    {
        BazaarLib.Item memory item;
        item.price = _price;
        item.seller = msg.sender;

        items.push(_itemId);
        detailsOf[_itemId] = item;
        itemsOf[msg.sender].push(_itemId);
        availability[_itemId] = _quantity;

        BazaarLib.emitNewItem(_itemId, _price, _quantity, msg.sender);
    }

    /**
     * @dev Returns all items in the bazaar.
     */
    function getAllItems() public view returns (bytes32[])
    {
        return items;
    }

    /**
     * @dev Returns all items of the seller. Intended to be invoked by the seller.
     */
    function getItems() public view ownerNotAllowed returns (bytes32[])
    {
        return itemsOf[msg.sender];
    }

    /**
     * @dev Used by customers to purchase certain \p _quantity of the item.
     * @param _itemId The item id (hash).
     * @param _quantity Purchased quantity.
     */
    function purchase(bytes32 _itemId, uint32 _quantity)
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

        availability[_itemId] -= _quantity;
        uint256 transferAmount = calculateTransferAmount(msg.value, commission);
        item.seller.transfer(transferAmount);
        BazaarLib.emitPurchase(_itemId, item.price, _quantity, msg.value, msg.sender, item.seller);
    }

    function calculateTransferAmount(
        uint256 _purchaseAmount,
        uint8 _commission
    )
        private
        pure
        returns (uint256 transferAmount)
    {
        return (100 - _commission) * _purchaseAmount / 100;
    }
}