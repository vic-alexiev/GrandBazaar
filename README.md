GrandBazaar
===========

What is Grand Bazaar?
---------------------
*Byzantium* hard fork...  
*Constantinople* release is due to come...  
*Istanbul* code-coverage testing framework...  
*Kapalicarsi* anyone?  
*GrandBazaar* is an ASP.NET Core MVC project which implements a decentralized marketplace.

* Visitors (users not logged in the web app) can view all the items listed for selling as well as each item's details. Each item can have one or more images. Visitors are not allowed to purchase items.

* Visitors can register as customers and purchase items.
  
  1. The `Bazaar` Ethereum smart contract keeps track of all the items that are on sale, their seller address, price and stock amount. When an item is purchased, the payment is sent to the contract which transfers it from the customer's to the seller's account.
  
  2. The smart contract retains a configurable percentage of each transaction. This commission is set when the contract is deployed. For instance, setting the commission to 5 (%) means the seller gets 95 % of the transaction amount.

* Sellers register and list items for sale in the Ethereum blockchain.

  1. Each item's stock amount, seller address and price are stored in Ethereum (the `Bazaar` contract).

  2. The other properties, such as name, description, images (Base64-encoded) are stored in the IPFS file system as a JSON document.

  3. Sellers can add items by providing a keystore file and a password so that their Ethereum account can be retrieved. This account is used when signing the transaction.
 
  4. Sellers can view only the items that they have created. They have to be logged in MetaMask so that their account address can be sent to the contract. The contract returns only the items for that seller based on the address provided.
  
* Customers have an account which is used when purchasing items (for signing the transaction). Again a keystore file and a password are required. This way there is no need to store customers' account private keys server-side.
