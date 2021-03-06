const Bazaar = artifacts.require("Bazaar");

const FINNEY = 10**15;

contract("Bazaar", accounts => {
    const [firstAccount, secondAccount, thirdAccount] = accounts;
    let bazaar;
    let itemId1 = web3.sha3("itemId1");
    let itemId2 = web3.sha3("itemId2");

    beforeEach(async () => {
        bazaar = await Bazaar.new(5);
    });

    it("sets an owner", async () => {
        assert.equal(await bazaar.owner.call(), firstAccount);
    });

    it("sets the commission", async () => {
        assert.equal(await bazaar.commission.call(), 5);
    });

    it("doesn't allow an owner to add an item", async () => {
        try {
            await bazaar.addItem(
                itemId1,
                20 * FINNEY,
                10,
                { from: firstAccount });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }
    });

    it("allows non-owners to add items: 1 seller adds 1 item", async () => {
        await bazaar.addItem(
            itemId1,
            15 * FINNEY,
            7,
            { from: secondAccount });

        assert.equal(await bazaar.availability.call(itemId1), 7);

        let itemDetails1 = await bazaar.detailsOf.call(itemId1);
        assert.equal(itemDetails1[0], 15 * FINNEY);
        assert.equal(itemDetails1[1], secondAccount);

        let sellerItems = await bazaar.getItems.call({ from: secondAccount });
        assert.equal(sellerItems.length, 1);
        assert.equal(sellerItems[0], itemId1);

        let allItems = await bazaar.getAllItems.call();
        assert.equal(allItems.length, 1);
        assert.equal(allItems[0], itemId1);
    });

    it("doesn't allow a seller to add the same item twice", async () => {
        await bazaar.addItem(
            itemId2,
            25 * FINNEY,
            5,
            { from: thirdAccount });

        assert.equal(await bazaar.availability.call(itemId2), 5);

        try {
            await bazaar.addItem(
                itemId2,
                25 * FINNEY,
                3,
                { from: thirdAccount });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }
    });

    it("doesn't allow a seller to add an item with price 0", async () => {
        try {
            await bazaar.addItem(
                itemId1,
                0 * FINNEY,
                6,
                { from: secondAccount });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }
    });

    it("doesn't allow a seller to add an item with quantity 0", async () => {
        try {
            await bazaar.addItem(
                itemId1,
                20 * FINNEY,
                0,
                { from: secondAccount });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }
    });

    it("allows non-owners to add items: 1 seller adds 2 items", async () => {
        await bazaar.addItem(
            itemId1,
            15 * FINNEY,
            7,
            { from: secondAccount });

        await bazaar.addItem(
            itemId2,
            30 * FINNEY,
            13,
            { from: secondAccount });

        assert.equal(await bazaar.availability.call(itemId1), 7);
        assert.equal(await bazaar.availability.call(itemId2), 13);

        let itemDetails1 = await bazaar.detailsOf.call(itemId1);
        assert.equal(itemDetails1[0], 15 * FINNEY);
        assert.equal(itemDetails1[1], secondAccount);

        let itemDetails2 = await bazaar.detailsOf.call(itemId2);
        assert.equal(itemDetails2[0], 30 * FINNEY);
        assert.equal(itemDetails2[1], secondAccount);

        let sellerItems = await bazaar.getItems.call({ from: secondAccount });
        assert.equal(sellerItems.length, 2);
        assert.equal(sellerItems[0], itemId1);
        assert.equal(sellerItems[1], itemId2);

        let allItems = await bazaar.getAllItems.call();
        assert.equal(allItems.length, 2);
        assert.equal(allItems[0], itemId1);
        assert.equal(allItems[1], itemId2);
    });

    it("allows non-owners to add items: 2 sellers adds 2 items", async () => {
        await bazaar.addItem(
            itemId1,
            15 * FINNEY,
            7,
            { from: secondAccount });

        await bazaar.addItem(
            itemId2,
            30 * FINNEY,
            13,
            { from: thirdAccount });

        assert.equal(await bazaar.availability.call(itemId1), 7);
        assert.equal(await bazaar.availability.call(itemId2), 13);

        let itemDetails1 = await bazaar.detailsOf.call(itemId1);
        assert.equal(itemDetails1[0], 15 * FINNEY);
        assert.equal(itemDetails1[1], secondAccount);

        let itemDetails2 = await bazaar.detailsOf.call(itemId2);
        assert.equal(itemDetails2[0], 30 * FINNEY);
        assert.equal(itemDetails2[1], thirdAccount);

        let sellerItems1 = await bazaar.getItems.call({ from: secondAccount });
        assert.equal(sellerItems1.length, 1);
        assert.equal(sellerItems1[0], itemId1);

        let sellerItems2 = await bazaar.getItems.call({ from: thirdAccount });
        assert.equal(sellerItems2.length, 1);
        assert.equal(sellerItems2[0], itemId2);

        let allItems = await bazaar.getAllItems.call();
        assert.equal(allItems.length, 2);
        assert.equal(allItems[0], itemId1);
        assert.equal(allItems[1], itemId2);
    });

    it("allows non-owners to purchase items: add item and purchase", async () => {
        const sellerInitBalance = web3.eth.getBalance(secondAccount);
        const customerInitBalance = web3.eth.getBalance(thirdAccount);

        await bazaar.addItem(
            itemId1,
            15 * FINNEY,
            7,
            { from: secondAccount });

        assert.equal(await bazaar.availability.call(itemId1), 7);

        await bazaar.purchase(
            itemId1,
            2,
            { from: thirdAccount, value: 2 * 15 * FINNEY });

        const sellerFinalBalance = web3.eth.getBalance(secondAccount);
        const customerFinalBalance = web3.eth.getBalance(thirdAccount);

        assert.equal(await bazaar.availability.call(itemId1), 5);
        assert.ok(sellerFinalBalance.greaterThan(sellerInitBalance));
        assert.ok(customerInitBalance.greaterThan(customerFinalBalance));
    });

    it("doesn't allow customer to purchase item that is not available", async () => {
        const sellerInitBalance = web3.eth.getBalance(secondAccount);
        const customerInitBalance = web3.eth.getBalance(thirdAccount);

        await bazaar.addItem(
            itemId1,
            15 * FINNEY,
            7,
            { from: secondAccount });

        assert.equal(await bazaar.availability.call(itemId1), 7);

        try {
            await bazaar.purchase(
                itemId2,
                2,
                { from: thirdAccount, value: 2 * 15 * FINNEY });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }

        const sellerFinalBalance = web3.eth.getBalance(secondAccount);
        const customerFinalBalance = web3.eth.getBalance(thirdAccount);

        assert.equal(await bazaar.availability.call(itemId1), 7);
        assert.ok(sellerInitBalance.greaterThan(sellerFinalBalance));
        assert.ok(customerInitBalance.greaterThan(customerFinalBalance));
    });

    it("doesn't allow customer to purchase item with insufficient stock", async () => {
        const sellerInitBalance = web3.eth.getBalance(secondAccount);
        const customerInitBalance = web3.eth.getBalance(thirdAccount);

        await bazaar.addItem(
            itemId1,
            30 * FINNEY,
            2,
            { from: secondAccount });

        assert.equal(await bazaar.availability.call(itemId1), 2);

        try {
            await bazaar.purchase(
                itemId1,
                3,
                { from: thirdAccount, value: 3 * 30 * FINNEY });
            assert.fail();
        } catch (err) {
            assert.ok(/revert/.test(err.message));
        }

        const sellerFinalBalance = web3.eth.getBalance(secondAccount);
        const customerFinalBalance = web3.eth.getBalance(thirdAccount);

        assert.equal(await bazaar.availability.call(itemId1), 2);
        assert.ok(sellerInitBalance.greaterThan(sellerFinalBalance));
        assert.ok(customerInitBalance.greaterThan(customerFinalBalance));
    });
});