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

Technical Details
-----------------

* .NET Core 2.1
* Entity Framework Core Tools 2.0.1
* SQL Server 2014 or above (for user account storage)
* Nethereum.Web3 2.5.1
* Visual Studio Code for contract development (using the Solidity plugin by Juan Franco)
* Remix IDE for smart contract deployment
* Truffle framework for unit testing
* Have Google Go and IPFS installed
* MetaMask and web3.js

Starting the Project
--------------------

* Clone the project locally and open it in Visual Studio (version >= 2017).
* Restore NuGet packages.
* Set `GrandBazaar.WebClient` project as startup project.
* In Package Manager Console select `GrandBazaar.WebClient` project from the dropdown above and execute `Update-Database`.
* In a command line start the local IPFS daemon by executing `ipfs daemon`.
* Start the project.

Screenshots
-----------

* <a href="https://user-images.githubusercontent.com/3578679/42722497-a1de794e-8755-11e8-8c3c-9962eef89c55.png" target="_parent">Home page</a>

Running the Tests
-----------------

* For ordinary test execution:

  1. Start `ganache-cli` in the command line.
  2. In Visual Studio Code terminal run `truffle test`.

* For code-coverage testing:

  1. In a command line window go to `GrandBazaar\src\Ethereum` directory and run `npm run test:run:server`.
  2. In Visual Studio Code terminal run `npm run test:coverage`.

Smart Contract Address
----------------------

* https://ropsten.etherscan.io/address/0xfecbe56b42eafa213461c70e6babf54750375570

* https://ropsten.etherscan.io/address/0x2a99ab3b2d7de551db599934e39a9f496c99f8b9 (backup :))

License
-------

* MIT
